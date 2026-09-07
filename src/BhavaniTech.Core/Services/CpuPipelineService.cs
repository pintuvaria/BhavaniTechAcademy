using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record InstructionStep(
        int Cycle,
        string Instruction,
        string Stage, // IF, ID, EX, MEM, WB, or STALL
        string Note
    );

    public record PipelineSimulationResult(
        string ProgramName,
        int TotalCycles,
        int InstructionCount,
        double Cpi, // Cycles Per Instruction
        int StallsCount,
        int ForwardingEventsCount,
        List<string> CycleHeaders,
        List<List<string>> GridMatrix, // rows: instructions, cols: cycles
        List<string> LogMessages
    );

    public static class CpuPipelineService
    {
        public static PipelineSimulationResult SimulateProgram(string programType, bool enableForwarding = true)
        {
            List<string> instructions;
            string progName;

            switch (programType.ToLowerInvariant())
            {
                case "forwarding_raw":
                    progName = "Data Hazard (RAW) - Resolved with ALU Forwarding";
                    instructions = new List<string>
                    {
                        "ADD R1, R2, R3   ; R1 = R2 + R3",
                        "SUB R4, R1, R5   ; R4 = R1 - R5 (RAW on R1)",
                        "AND R6, R1, R7   ; R6 = R1 & R7 (RAW on R1)",
                        "OR  R8, R9, R10  ; Independent instruction"
                    };
                    break;

                case "load_use":
                    progName = "Load-Use Data Hazard - Requires 1-Cycle Stall";
                    instructions = new List<string>
                    {
                        "LW  R1, 0(R2)    ; Load R1 from Memory",
                        "ADD R4, R1, R3   ; Load-Use Hazard! Cannot forward from EX",
                        "SUB R5, R6, R7   ; Independent instruction",
                        "SW  R5, 4(R2)    ; Store R5 to Memory"
                    };
                    break;

                case "branch_hazard":
                    progName = "Control Hazard (Branch Taken) - Pipeline Flush";
                    instructions = new List<string>
                    {
                        "BEQ R1, R2, TARGET ; Branch if R1 == R2 (Taken)",
                        "ADD R3, R4, R5     ; [FLUSHED] Fetched speculatively",
                        "SUB R6, R7, R8     ; [FLUSHED] Fetched speculatively",
                        "TARGET: XOR R9, R9 ; Target instruction executed"
                    };
                    break;

                case "ideal":
                default:
                    progName = "Ideal Independent Sequence (No Hazards, CPI = 1.0)";
                    instructions = new List<string>
                    {
                        "ADD R1, R2, R3   ; R1 = R2 + R3",
                        "SUB R4, R5, R6   ; R4 = R5 - R6",
                        "AND R7, R8, R9   ; R7 = R8 & R9",
                        "OR  R10, R11, R12; R10 = R11 | R12"
                    };
                    break;
            }

            var logs = new List<string>();
            int stallCount = 0;
            int fwdCount = 0;

            int numInstructions = instructions.Count;
            // Build the cycle grid
            // Typical 5 stages: IF, ID, EX, MEM, WB
            var grid = new List<List<string>>();

            if (programType.ToLowerInvariant() == "ideal")
            {
                int totalCycles = numInstructions + 4; // 8 cycles for 4 instructions
                for (int i = 0; i < numInstructions; i++)
                {
                    var row = new List<string>();
                    for (int c = 1; c <= totalCycles; c++)
                    {
                        int stageIdx = c - i;
                        string stage = stageIdx switch
                        {
                            1 => "IF",
                            2 => "ID",
                            3 => "EX",
                            4 => "MEM",
                            5 => "WB",
                            _ => "-"
                        };
                        row.Add(stage);
                    }
                    grid.Add(row);
                }
                logs.Add("[IDEAL] No dependencies detected. Each instruction advances 1 stage per clock cycle.");
                logs.Add("[METRIC] Throughput: 1 instruction completed per cycle in steady state. CPI = 1.0 (approx).");
                return FormatResult(progName, totalCycles, numInstructions, stallCount, fwdCount, grid, logs);
            }
            else if (programType.ToLowerInvariant() == "forwarding_raw")
            {
                if (enableForwarding)
                {
                    fwdCount = 2;
                    int totalCycles = numInstructions + 4;
                    for (int i = 0; i < numInstructions; i++)
                    {
                        var row = new List<string>();
                        for (int c = 1; c <= totalCycles; c++)
                        {
                            int stageIdx = c - i;
                            string stage = stageIdx switch
                            {
                                1 => "IF",
                                2 => "ID",
                                3 => "EX",
                                4 => "MEM",
                                5 => "WB",
                                _ => "-"
                            };
                            row.Add(stage);
                        }
                        grid.Add(row);
                    }
                    logs.Add("[FORWARDING ENABLED] Instruction 2 (SUB) needs R1 in cycle 4 (EX).");
                    logs.Add("[BYPASS PATH] Instruction 1 (ADD) forwards R1 ALU result from EX/MEM buffer directly to EX input of SUB at Cycle 4!");
                    logs.Add("[FORWARDING ENABLED] Instruction 3 (AND) receives R1 forwarded from MEM/WB buffer at Cycle 5!");
                    logs.Add("[RESULT] Zero stalls required. Pipeline flows at maximum clock speed.");
                    return FormatResult(progName, totalCycles, numInstructions, stallCount, fwdCount, grid, logs);
                }
                else
                {
                    // Without forwarding: 2 stall bubbles needed
                    stallCount = 2;
                    int totalCycles = numInstructions + 4 + 2;
                    // Instr 0 starts cycle 1
                    // Instr 1 fetches cycle 2, decodes cycle 3, stalls cycle 4, stalls cycle 5, EX cycle 6...
                    var r0 = new List<string> { "IF", "ID", "EX", "MEM", "WB", "-", "-", "-", "-", "-" };
                    var r1 = new List<string> { "-", "IF", "ID", "STALL", "STALL", "EX", "MEM", "WB", "-", "-" };
                    var r2 = new List<string> { "-", "-", "-", "IF", "ID", "ID", "EX", "MEM", "WB", "-" };
                    var r3 = new List<string> { "-", "-", "-", "-", "IF", "IF", "ID", "EX", "MEM", "WB" };
                    grid.Add(r0);
                    grid.Add(r1);
                    grid.Add(r2);
                    grid.Add(r3);
                    logs.Add("[NO FORWARDING] SUB must wait for ADD to write back R1 to register file in cycle 5 (WB).");
                    logs.Add("[STALL INSERTED] 2 hardware bubbles (NOPs) injected into pipeline, degrading CPI.");
                    return FormatResult(progName, totalCycles, numInstructions, stallCount, fwdCount, grid, logs);
                }
            }
            else if (programType.ToLowerInvariant() == "load_use")
            {
                // LW data only available after MEM stage (Cycle 4)
                // ADD needs it in EX stage. Even with forwarding, 1 cycle stall is mandatory!
                stallCount = 1;
                fwdCount = 1;
                int totalCycles = numInstructions + 4 + 1; // 9 cycles
                var r0 = new List<string> { "IF", "ID", "EX", "MEM", "WB", "-", "-", "-", "-" };
                var r1 = new List<string> { "-", "IF", "ID", "STALL", "EX", "MEM", "WB", "-", "-" };
                var r2 = new List<string> { "-", "-", "IF", "IF", "ID", "EX", "MEM", "WB", "-" };
                var r3 = new List<string> { "-", "-", "-", "-", "IF", "ID", "EX", "MEM", "WB" };
                grid.Add(r0);
                grid.Add(r1);
                grid.Add(r2);
                grid.Add(r3);
                logs.Add("[LOAD-USE HAZARD] LW result is not available until MEM stage completes (End of Cycle 4).");
                logs.Add("[STALL] Hardware Hazard Detection Unit stalls ADD for 1 cycle (Cycle 4) inserting a NOP bubble.");
                logs.Add("[FORWARDING] In Cycle 5, data is forwarded from MEM/WB register directly into ADD's EX stage.");
                return FormatResult(progName, totalCycles, numInstructions, stallCount, fwdCount, grid, logs);
            }
            else // branch_hazard
            {
                stallCount = 2;
                int totalCycles = 8;
                var r0 = new List<string> { "IF", "ID", "EX", "MEM", "WB", "-", "-", "-" };
                var r1 = new List<string> { "-", "IF", "ID", "FLUSH", "-", "-", "-", "-" };
                var r2 = new List<string> { "-", "-", "IF", "FLUSH", "-", "-", "-", "-" };
                var r3 = new List<string> { "-", "-", "-", "IF", "ID", "EX", "MEM", "WB" };
                grid.Add(r0);
                grid.Add(r1);
                grid.Add(r2);
                grid.Add(r3);
                logs.Add("[CONTROL HAZARD] Branch outcome determined in EX stage at Cycle 3.");
                logs.Add("[PIPELINE FLUSH] Two speculatively fetched instructions in IF and ID stages are cancelled (NOPed).");
                logs.Add("[BRANCH PENALTY] 2 clock cycles penalty incurred before branch target starts execution in Cycle 4.");
                return FormatResult(progName, totalCycles, numInstructions, stallCount, fwdCount, grid, logs);
            }
        }

        private static PipelineSimulationResult FormatResult(
            string progName,
            int totalCycles,
            int instructionCount,
            int stalls,
            int forwardings,
            List<List<string>> grid,
            List<string> logs)
        {
            var headers = new List<string>();
            for (int c = 1; c <= totalCycles; c++)
            {
                headers.Add($"C{c}");
            }

            double cpi = Math.Round((double)totalCycles / instructionCount, 2);

            return new PipelineSimulationResult(
                ProgramName: progName,
                TotalCycles: totalCycles,
                InstructionCount: instructionCount,
                Cpi: cpi,
                StallsCount: stalls,
                ForwardingEventsCount: forwardings,
                CycleHeaders: headers,
                GridMatrix: grid,
                LogMessages: logs
            );
        }
    }
}
