using System;
using System.Collections.Generic;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record AsmInstruction(string Address, string HexBytes, string Mnemonic, string Operands, string Explanation);

    public record DisassemblyPattern(
        string PatternName,
        string HighLevelCode,
        List<AsmInstruction> Instructions,
        string ArchitecturalExplanation
    );

    public static class ReverseEngineeringService
    {
        public static List<DisassemblyPattern> GetStandardPatterns()
        {
            return new List<DisassemblyPattern>
            {
                new(
                    "For-Loop Iteration",
                    "int sum = 0;\nfor (int i = 0; i < 10; i++) {\n    sum += i;\n}",
                    new List<AsmInstruction>
                    {
                        new("0x00401120", "c7 45 f8 00 00 00 00", "mov", "dword ptr [rbp-8], 0", "Initialize 'sum' = 0 on local stack frame"),
                        new("0x00401127", "c7 45 fc 00 00 00 00", "mov", "dword ptr [rbp-4], 0", "Initialize loop counter 'i' = 0"),
                        new("0x0040112e", "eb 0a",                   "jmp", "0x0040113a",           "Unconditional jump forward to loop condition check"),
                        new("0x00401130", "8b 45 fc",                "mov", "eax, dword ptr [rbp-4]", "Loop Body: Load 'i' into EAX accumulator"),
                        new("0x00401133", "01 45 f8",                "add", "dword ptr [rbp-8], eax", "Loop Body: sum = sum + EAX"),
                        new("0x00401136", "83 45 fc 01",             "add", "dword ptr [rbp-4], 1", "Step: Increment counter i++"),
                        new("0x0040113a", "83 7d fc 09",             "cmp", "dword ptr [rbp-4], 9", "Condition: Compare i with 9"),
                        new("0x0040113e", "7e f0",                   "jle", "0x00401130",           "Branch: Jump back to loop body if i <= 9")
                    },
                    "Loop Microarchitecture:\nModern CPUs translate 'for' loops into a bottom-tested conditional branch. The branch predictor remembers the backward jump 0x0040113e as TAKEN until the 10th iteration, minimizing pipeline stall bubbles."
                ),

                new(
                    "Conditional If-Else Branching",
                    "if (accessCode == 1337) {\n    grantRoot();\n} else {\n    reject();\n}",
                    new List<AsmInstruction>
                    {
                        new("0x00401150", "81 7d fc 39 05 00 00", "cmp", "dword ptr [rbp-4], 0x539", "Compare accessCode against literal 1337 (0x539 hex)"),
                        new("0x00401157", "75 0c",                "jne", "0x00401165",               "Jump to Else block if Zero Flag (ZF) == 0 (Not Equal)"),
                        new("0x00401159", "e8 42 00 00 00",       "call", "grantRoot",                 "Then Block: Invoke grantRoot() subroutine"),
                        new("0x0040115e", "eb 0a",                "jmp",  "0x0040116a",               "Jump over Else block to function continuation"),
                        new("0x00401160", "90",                   "nop",  "",                         "Alignment padding byte"),
                        new("0x00401165", "e8 76 00 00 00",       "call", "reject",                    "Else Block: Invoke reject() subroutine"),
                        new("0x0040116a", "90",                   "nop",  "",                         "Function continuation point")
                    },
                    "Branch Inversion Exploit Tip:\nIn classic binary reverse engineering and software cracking, attackers patch the single byte '75' (JNE) at 0x00401157 into '74' (JE) or '90 90' (NOP NOP), forcing the processor to execute grantRoot() regardless of password input!"
                ),

                new(
                    "Function Stack Frame Prologue & Epilogue (x86_64 ABI)",
                    "int computeArea(int width, int height) {\n    return width * height;\n}",
                    new List<AsmInstruction>
                    {
                        new("0x00401180", "55",                   "push", "rbp",                      "PROLOGUE: Save caller base pointer on stack"),
                        new("0x00401181", "48 89 e5",             "mov",  "rbp, rsp",                 "PROLOGUE: Establish new stack frame base"),
                        new("0x00401184", "89 7d fc",             "mov",  "dword ptr [rbp-4], edi",   "Save 1st argument (EDI = width) on local stack"),
                        new("0x00401187", "89 75 f8",             "mov",  "dword ptr [rbp-8], esi",   "Save 2nd argument (ESI = height) on local stack"),
                        new("0x0040118a", "8b 45 fc",             "mov",  "eax, dword ptr [rbp-4]",   "Load width into EAX"),
                        new("0x0040118d", "0f af 45 f8",          "imul", "eax, dword ptr [rbp-8]",   "Signed multiplication: EAX = width * height"),
                        new("0x00401191", "5d",                   "pop",  "rbp",                      "EPILOGUE: Restore caller stack frame base pointer"),
                        new("0x00401192", "c3",                   "ret",  "",                         "EPILOGUE: Pop return address into RIP, return to caller")
                    },
                    "Calling Convention Insight (System V AMD64 ABI):\nArguments 1 to 6 are passed in CPU registers (RDI, RSI, RDX, RCX, R8, R9) rather than pushing to stack memory, speeding up function invocations by over 400%."
                ),

                new(
                    "Buffer Overflow Stack Smashing Vulnerability",
                    "void vulnerable() {\n    char buffer[16];\n    gets(buffer); // Dangerous!\n}",
                    new List<AsmInstruction>
                    {
                        new("0x00401200", "55",                   "push", "rbp",                      "Save Caller RBP on stack [RSP]"),
                        new("0x00401201", "48 89 e5",             "mov",  "rbp, rsp",                 "RBP points to Base of Stack Frame"),
                        new("0x00401204", "48 83 ec 10",          "sub",  "rsp, 16",                  "Allocate 16 bytes for char buffer[16]"),
                        new("0x00401208", "48 8d 45 f0",          "lea",  "rax, [rbp-16]",            "Calculate address of buffer"),
                        new("0x0040120c", "48 89 c7",             "mov",  "rdi, rax",                 "Pass buffer address to gets() in RDI"),
                        new("0x0040120f", "e8 3c 00 00 00",       "call", "gets",                     "DANGER: gets() does not check boundary limits!"),
                        new("0x00401214", "c9",                   "leave","",                         "Restores RSP and RBP"),
                        new("0x00401215", "c3",                   "ret",  "",                         "Pops [RSP] into RIP (Instruction Pointer)")
                    },
                    "Buffer Layout & Return Pointer Hijacking:\n" +
                    "Stack Memory Growth (Low to High Address):\n" +
                    "[RBP-16]: buffer[0..15] (16 bytes allocated)\n" +
                    "[RBP+0] : Saved RBP (8 bytes)\n" +
                    "[RBP+8] : Saved RIP Return Address (8 bytes)\n\n" +
                    "If input > 24 bytes (16 buffer + 8 saved RBP), the next 8 bytes overwrite the Saved RIP!\n" +
                    "When 'ret' executes, the CPU jumps to whatever memory address the attacker injected."
                )
            };
        }
    }
}
