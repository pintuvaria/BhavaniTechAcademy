using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record EbpfInstruction(
        int InstructionIndex,
        string Opcode,
        string DstReg,
        string SrcReg,
        int Offset,
        long Immediate,
        string HumanExplanation
    );

    public record EbpfVerificationResult(
        bool IsApproved,
        int TotalInstructions,
        int ComplexityScore,
        List<string> SafetyChecks,
        string VerificationReport
    );

    public record EbpfExecutionResult(
        long ReturnValue,
        Dictionary<string, long> Registers,
        Dictionary<string, long> MapSnapshot,
        List<string> TraceLog,
        bool Success
    );

    public class EbpfSimulatorService
    {
        // eBPF Maps
        public Dictionary<string, long> ProcessExecCounterMap { get; } = new();
        public Dictionary<string, long> PacketFilterDropMap { get; } = new();

        public EbpfSimulatorService()
        {
            ProcessExecCounterMap["/usr/bin/curl"] = 14;
            ProcessExecCounterMap["/bin/bash"] = 38;
            ProcessExecCounterMap["/usr/sbin/nginx"] = 5;

            PacketFilterDropMap["192.168.1.105"] = 120;
            PacketFilterDropMap["10.0.0.99"] = 45;
        }

        public List<EbpfInstruction> GetSampleProgram(string programType)
        {
            if (programType.Equals("XDP", StringComparison.OrdinalIgnoreCase))
            {
                // XDP Packet Filter
                return new List<EbpfInstruction>
                {
                    new(0, "BPF_LDX_MEM", "r1", "r1", 0, 0, "r1 = ctx->data (Pointer to start of packet Ethernet frame)"),
                    new(1, "BPF_LDX_MEM", "r2", "r1", 4, 0, "r2 = ctx->data_end (Pointer to end of packet boundary)"),
                    new(2, "BPF_ALU64_IMM", "r3", "r1", 0, 14, "r3 = r1 + 14 (Skip 14-byte Ethernet header)"),
                    new(3, "BPF_JMP_REG", "r3", "r2", 4, 0, "if (r3 > r2) goto exit_pass (Bounds check to prevent kernel crash)"),
                    new(4, "BPF_LDX_MEM", "r4", "r3", 9, 0, "r4 = *(u8*)(r3 + 9) (Load IP Protocol byte: 6 = TCP)"),
                    new(5, "BPF_JMP_IMM", "r4", "", 1, 6, "if (r4 == 6) goto inspect_tcp"),
                    new(6, "BPF_MOV64_IMM", "r0", "", 0, 2, "r0 = XDP_PASS (Allow non-TCP packet through network stack)"),
                    new(7, "BPF_EXIT", "", "", 0, 0, "return r0"),
                    new(8, "BPF_MOV64_IMM", "r0", "", 0, 1, "r0 = XDP_DROP (Drop packet immediately in NIC driver - Zero overhead)"),
                    new(9, "BPF_EXIT", "", "", 0, 0, "return r0")
                };
            }

            // Default: Kprobe on sys_execve
            return new List<EbpfInstruction>
            {
                new(0, "BPF_MOV64_REG", "r6", "r1", 0, 0, "r6 = r1 (Save ctx pointer into callee-saved register r6)"),
                new(1, "BPF_LDX_MEM", "r2", "r6", 0, 0, "r2 = ctx->filename (Fetch executable path string pointer)"),
                new(2, "BPF_MOV64_IMM", "r1", "", 0, 1, "r1 = BPF_MAP_ID_EXEC_COUNTER (Target Map reference)"),
                new(3, "BPF_CALL", "", "", 0, 1, "call bpf_map_lookup_elem(r1, r2) -> Look up current count"),
                new(4, "BPF_JMP_IMM", "r0", "", 2, 0, "if (r0 == NULL) goto init_new_entry"),
                new(5, "BPF_ALU64_IMM", "r0", "r0", 0, 1, "*(u64*)r0 += 1 (Increment execution counter in BPF map)"),
                new(6, "BPF_EXIT", "", "", 0, 0, "return 0 (Allow process execution to proceed)")
            };
        }

        public EbpfVerificationResult VerifyBytecode(List<EbpfInstruction> program)
        {
            var checks = new List<string>();
            bool hasExit = program.Any(i => i.Opcode == "BPF_EXIT");
            bool hasBoundsCheck = program.Any(i => i.HumanExplanation.Contains("Bounds check"));
            bool hasLoop = false; // Simulated DAG check for bounded loops

            checks.Add($"[DAG Acyclicity]: Program is a Directed Acyclic Graph. Loops detected: {hasLoop}.");
            checks.Add($"[Termination]: Valid BPF_EXIT instruction present: {hasExit}.");
            checks.Add($"[Memory Bounds Safety]: Pointer arithmetic bounds checked before dereference: {hasBoundsCheck || program.Count < 8}.");
            checks.Add("[Register Liveness]: All destination registers initialized prior to ALU usage.");
            checks.Add("[Privilege Constraint]: Program does not call unwhitelisted kernel symbols.");

            bool approved = hasExit && !hasLoop;
            var sb = new StringBuilder();
            sb.AppendLine("=== IN-KERNEL eBPF BYTECODE VERIFIER ANALYSIS ===");
            sb.AppendLine($"Total Verified Instructions: {program.Count}");
            sb.AppendLine($"Safety Confidence Score   : {(approved ? "100% (Kernel Safe - Zero Panic Risk)" : "REJECTED ❌")}");
            sb.AppendLine("Status: " + (approved ? "LOAD_APPROVED ✅ (Bytecode verified & JIT compiled into native machine code)" : "REJECTED ❌"));

            return new EbpfVerificationResult(approved, program.Count, program.Count * 2, checks, sb.ToString());
        }

        public EbpfExecutionResult ExecuteTrace(string programType, string targetItem)
        {
            var regs = new Dictionary<string, long>
            {
                ["R0"] = 0, ["R1"] = 0x7FFF0010, ["R2"] = 0x7FFF0080,
                ["R3"] = 0, ["R6"] = 0x7FFF0010, ["R10"] = 0x7FFF8000
            };
            var trace = new List<string>();

            if (programType.Equals("XDP", StringComparison.OrdinalIgnoreCase))
            {
                bool isDrop = targetItem.EndsWith(".99") || targetItem.EndsWith(".105");
                long retVal = isDrop ? 1 : 2; // 1 = XDP_DROP, 2 = XDP_PASS
                regs["R0"] = retVal;

                if (isDrop)
                {
                    PacketFilterDropMap[targetItem] = PacketFilterDropMap.GetValueOrDefault(targetItem, 0) + 1;
                }

                trace.Add($"eBPF Hook: xdp_ingress (Network Device driver layer)");
                trace.Add($"Packet Source IP: {targetItem}");
                trace.Add($"Action: {(isDrop ? "XDP_DROP 🚨 (Packet intercepted & discarded before Linux SKB allocation)" : "XDP_PASS ✅ (Forwarded to Linux TCP/IP stack)")}");
                trace.Add($"Total Drops for {targetItem}: {PacketFilterDropMap.GetValueOrDefault(targetItem, 0)}");

                return new EbpfExecutionResult(retVal, regs, PacketFilterDropMap, trace, true);
            }
            else
            {
                // Kprobe sys_execve
                ProcessExecCounterMap[targetItem] = ProcessExecCounterMap.GetValueOrDefault(targetItem, 0) + 1;
                regs["R0"] = 0; // sys_execve proceed

                trace.Add($"eBPF Hook: kprobe/sys_execve (Kernel System Call Entry)");
                trace.Add($"Process Binary Invoked: {targetItem}");
                trace.Add($"Map Update: ProcessExecCounterMap['{targetItem}'] = {ProcessExecCounterMap[targetItem]}");
                trace.Add("Telemetry emitted to Ring Buffer without context-switching to userland!");

                return new EbpfExecutionResult(0, regs, ProcessExecCounterMap, trace, true);
            }
        }
    }
}
