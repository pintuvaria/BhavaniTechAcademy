using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BhavaniTech.Core.Models;
using Microsoft.Data.Sqlite;

namespace BhavaniTech.Core.Services
{
    public partial class PracticalExamService
    {
        public static void EnsurePracticalExamsSeeded(SqliteConnection conn)
        {
            using var countCmd = conn.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM PracticalExams";
            long count = (long)(countCmd.ExecuteScalar() ?? 0);

            if (count >= 108) return;

            using var tx = conn.BeginTransaction();
            using var delCmd = conn.CreateCommand();
            delCmd.Transaction = tx;
            delCmd.CommandText = "DELETE FROM PracticalExams";
            delCmd.ExecuteNonQuery();

            var exams = GetAllExams();
            foreach (var ex in exams)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO PracticalExams 
                    (LessonId, Title, Scenario, TaskInstructions, StarterCode, ExpectedKeywords, EvaluationType, Hint, MaxScore, XpReward)
                    VALUES (@lId, @title, @scen, @inst, @code, @keys, @type, @hint, @max, @xp)";
                cmd.Parameters.AddWithValue("@lId", ex.LessonId);
                cmd.Parameters.AddWithValue("@title", ex.Title);
                cmd.Parameters.AddWithValue("@scen", ex.Scenario);
                cmd.Parameters.AddWithValue("@inst", ex.TaskInstructions);
                cmd.Parameters.AddWithValue("@code", ex.StarterCode);
                cmd.Parameters.AddWithValue("@keys", ex.ExpectedKeywords);
                cmd.Parameters.AddWithValue("@type", ex.EvaluationType);
                cmd.Parameters.AddWithValue("@hint", ex.Hint);
                cmd.Parameters.AddWithValue("@max", ex.MaxScore);
                cmd.Parameters.AddWithValue("@xp", ex.XpReward);
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        public static PracticalExamResult Evaluate(string lessonId, string submission)
        {
            var exam = GetAllExams().FirstOrDefault(e => e.LessonId.Equals(lessonId, StringComparison.OrdinalIgnoreCase));
            if (exam == null)
            {
                return new PracticalExamResult(false, 0, 100, 0, "No practical exam registered for this lesson.", new List<string> { "Error: Unknown lesson ID." });
            }

            var logs = new List<string>();
            logs.Add($"=== BHAVANI PRACTICAL EXAM ASSESSMENT: {exam.Title} ===");
            logs.Add($"Evaluation Mode: {exam.EvaluationType}");
            logs.Add($"Submission Length: {submission?.Length ?? 0} characters\n");

            if (string.IsNullOrWhiteSpace(submission))
            {
                logs.Add("[FAIL ❌] Submission is empty. Please provide your solution.");
                return new PracticalExamResult(false, 0, exam.MaxScore, 0, "Submission was empty.", logs);
            }

            string clean = submission.Trim();
            bool pass = false;
            string feedback = "";

            switch (exam.EvaluationType)
            {
                case "CodeExecution":
                case "OutputRegex":
                    pass = EvaluateCodeOrKeywords(clean, exam, logs);
                    break;

                case "CommandSimulation":
                    pass = EvaluateCommand(clean, exam, logs);
                    break;

                case "Calculation":
                    pass = EvaluateCalculation(clean, exam, logs);
                    break;

                case "SqlValidation":
                    pass = EvaluateSql(clean, exam, logs);
                    break;

                default:
                    pass = EvaluateCodeOrKeywords(clean, exam, logs);
                    break;
            }

            int finalScore = pass ? exam.MaxScore : 0;
            int xpAwarded = pass ? exam.XpReward : 0;
            feedback = pass ? "EXCELLENT WORK! All practical criteria passed." : "Needs revision. Review the requirements and hint.";

            logs.Add($"\n=== FINAL PRACTICAL SCORE: {finalScore} / {exam.MaxScore} ({ (pass ? "PASSED ✅" : "FAILED ❌") }) ===");
            if (pass) logs.Add($"⭐ Reward: +{xpAwarded} XP credited to your profile!");

            return new PracticalExamResult(pass, finalScore, exam.MaxScore, xpAwarded, feedback, logs);
        }

        private static bool EvaluateCodeOrKeywords(string code, PracticalExam exam, List<string> logs)
        {
            var required = exam.ExpectedKeywords.Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(k => k.Trim()).ToList();
            int passedCount = 0;

            for (int i = 0; i < required.Count; i++)
            {
                string kw = required[i];
                bool found = false;

                if (kw.StartsWith("regex:", StringComparison.OrdinalIgnoreCase))
                {
                    string pattern = kw.Substring(6);
                    found = Regex.IsMatch(code, pattern, RegexOptions.IgnoreCase);
                }
                else
                {
                    found = code.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                if (found)
                {
                    logs.Add($"[Test {i + 1}] Check requirement '{kw}': PASSED ✅");
                    passedCount++;
                }
                else
                {
                    logs.Add($"[Test {i + 1}] Check requirement '{kw}': FAILED ❌ (Missing expected pattern/keyword)");
                }
            }

            return passedCount == required.Count;
        }

        private static bool EvaluateCommand(string cmd, PracticalExam exam, List<string> logs)
        {
            var required = exam.ExpectedKeywords.Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(k => k.Trim()).ToList();
            int passedCount = 0;

            for (int i = 0; i < required.Count; i++)
            {
                string kw = required[i];
                bool matched = cmd.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0;
                if (matched)
                {
                    logs.Add($"[Test {i + 1}] CLI Syntax check '{kw}': PASSED ✅");
                    passedCount++;
                }
                else
                {
                    logs.Add($"[Test {i + 1}] CLI Syntax check '{kw}': FAILED ❌");
                }
            }

            return passedCount == required.Count;
        }

        private static bool EvaluateCalculation(string submission, PracticalExam exam, List<string> logs)
        {
            var parts = exam.ExpectedKeywords.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int passed = 0;

            foreach (var target in parts)
            {
                string t = target.Trim();
                if (submission.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    logs.Add($"[Math Test] Calculated Target '{t}': MATCHED ✅");
                    passed++;
                }
                else
                {
                    logs.Add($"[Math Test] Calculated Target '{t}': NOT FOUND in submission ❌");
                }
            }

            return passed == parts.Length;
        }

        private static bool EvaluateSql(string query, PracticalExam exam, List<string> logs)
        {
            var parts = exam.ExpectedKeywords.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int passed = 0;

            foreach (var kw in parts)
            {
                string k = kw.Trim();
                if (query.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    logs.Add($"[SQL Syntax] Clause '{k}': VERIFIED ✅");
                    passed++;
                }
                else
                {
                    logs.Add($"[SQL Syntax] Clause '{k}': MISSING ❌");
                }
            }

            try
            {
                using var testConn = new SqliteConnection("Data Source=:memory:");
                testConn.Open();
                using var setupCmd = testConn.CreateCommand();
                setupCmd.CommandText = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Username TEXT, Role TEXT); INSERT INTO Users VALUES (1, 'admin', 'SuperAdmin'), (2, 'guest', 'Guest');";
                setupCmd.ExecuteNonQuery();

                using var runCmd = testConn.CreateCommand();
                runCmd.CommandText = query;
                runCmd.ExecuteNonQuery();
                logs.Add("[SQL Execution Sandbox] Query parsed & executed without syntax error ✅");
            }
            catch (Exception ex)
            {
                logs.Add($"[SQL Execution Sandbox] Warning: {ex.Message}");
            }

            return passed == parts.Length;
        }

        public static List<PracticalExam> GetAllExams()
        {
            var list = new List<PracticalExam>
            {
                // =========================================================================
                // 1. COMPUTER FUNDAMENTALS (CS101)
                // =========================================================================
                new(1, "L_CS_1", "Practical 1.1: Binary Representation & Bit Counting",
                    "A hardware sensor communicates data in raw bytes. You need to write a function that converts an 8-bit byte into its binary string representation and counts the number of set bits (1s).",
                    "1. Complete the function `ToBinaryAndCountBits(byte value)`\n2. Return both the 8-character binary string and total 1 bits.\n3. Verify test value 42 (00101010, 3 set bits).",
                    "// C# Practical Implementation\npublic static (string binary, int setBits) ToBinaryAndCountBits(byte value)\n{\n    string binary = Convert.ToString(value, 2).PadLeft(8, '0');\n    int count = 0;\n    foreach (char c in binary) if (c == '1') count++;\n    return (binary, count);\n}",
                    "Convert.ToString;PadLeft;count", "CodeExecution", "Use Convert.ToString(value, 2) and count occurrences of '1'.", 100, 100),

                new(2, "L_CS_2", "Practical 1.2: CPU ALU Instruction Decoder",
                    "Build a mini software ALU instruction decoder executing basic arithmetic operations based on opcodes.",
                    "1. Implement `ExecuteAluOp(string opcode, int regA, int regB)`.\n2. Handle opcodes: 'ADD', 'SUB', 'AND', 'XOR'.\n3. Return the calculated integer result.",
                    "public static int ExecuteAluOp(string opcode, int regA, int regB)\n{\n    return opcode switch\n    {\n        \"ADD\" => regA + regB,\n        \"SUB\" => regA - regB,\n        \"AND\" => regA & regB,\n        \"XOR\" => regA ^ regB,\n        _ => 0\n    };\n}",
                    "ADD;SUB;AND;XOR", "CodeExecution", "Use a switch statement matching 'ADD', 'SUB', 'AND', 'XOR'.", 100, 100),

                new(3, "L_CS_3", "Practical 1.3: Virtual Memory Page Table Translation",
                    "Simulate an MMU translating a 16-bit Virtual Address into a Physical RAM Address given a 4KB (4096 bytes) page size.",
                    "1. Extract Page Number = VirtualAddress / 4096.\n2. Extract Offset = VirtualAddress % 4096.\n3. Physical Address = (FrameNumber * 4096) + Offset.",
                    "public static int TranslateAddress(int virtualAddress, int frameNumber)\n{\n    int pageSize = 4096;\n    int offset = virtualAddress % pageSize;\n    return (frameNumber * pageSize) + offset;\n}",
                    "pageSize;4096;offset;frameNumber", "CodeExecution", "Modulo 4096 gives the intra-page byte offset.", 100, 100),

                new(4, "L_CS_4", "Practical 1.4: System Call Dispatch Table",
                    "Create an OS kernel syscall table handling SYS_READ (0), SYS_WRITE (1), and SYS_EXIT (60).",
                    "1. Create `DispatchSyscall(int syscallId, string argument)`.\n2. Dispatch appropriate handler based on ID.\n3. Return execution status string.",
                    "public static string DispatchSyscall(int syscallId, string argument)\n{\n    switch (syscallId)\n    {\n        case 0: return $\"SYS_READ: {argument}\";\n        case 1: return $\"SYS_WRITE: {argument}\";\n        case 60: return $\"SYS_EXIT: code {argument}\";\n        default: return \"EINVAL: Unknown Syscall\";\n    }\n}",
                    "SYS_READ;SYS_WRITE;SYS_EXIT", "CodeExecution", "Map syscall numbers 0, 1, and 60.", 100, 100),

                new(5, "L_CS_5", "Practical 1.5: Speculative Execution Barrier Simulation",
                    "Implement a memory access routine that uses speculative fence instructions (LFENCE) to mitigate branch predictor leaks.",
                    "1. Implement `SafeCacheRead(int index, int[] array, bool useFence)`.\n2. When useFence is true, simulate speculative serialization.",
                    "public static int SafeCacheRead(int index, int[] array, bool useFence)\n{\n    if (index >= 0 && index < array.Length)\n    {\n        if (useFence) { /* LFENCE hardware barrier */ }\n        return array[index];\n    }\n    return -1;\n}",
                    "LFENCE;array.Length", "CodeExecution", "Ensure boundary checks and LFENCE serialization.", 100, 100),

                new(6, "L_CS_6", "Practical 1.6: Qubit Superposition State Vector Calculation",
                    "Calculate the probability amplitude |alpha|^2 and |beta|^2 of a quantum qubit state |psi> = alpha|0> + beta|1>.",
                    "1. Given alpha = 1/sqrt(2) and beta = 1/sqrt(2).\n2. Calculate probability of measuring 0 and 1 (both 0.5).\n3. Assert sum of probabilities equals 1.0.",
                    "double alpha = 1.0 / Math.Sqrt(2);\ndouble beta = 1.0 / Math.Sqrt(2);\ndouble p0 = Math.Pow(alpha, 2);\ndouble p1 = Math.Pow(beta, 2);\ndouble total = p0 + p1; // Must equal 1.0",
                    "Math.Sqrt;Math.Pow;total", "CodeExecution", "Probabilities are the squares of the amplitudes.", 100, 100),

                // =========================================================================
                // 2. POLYGLOT PROGRAMMING (PROG101)
                // =========================================================================
                new(7, "L_PRG_1", "Practical 2.1: Strong Static Typing Validator",
                    "Write a typed data record model holding student information with validation.",
                    "1. Create `StudentRecord(int id, string name, double gpa)`.\n2. Validate GPA is between 0.0 and 4.0.",
                    "public record StudentRecord(int Id, string Name, double Gpa)\n{\n    public bool IsValid() => Gpa >= 0.0 && Gpa <= 4.0 && !string.IsNullOrWhiteSpace(Name);\n}",
                    "record;Gpa;IsValid", "CodeExecution", "C# record with an IsValid method.", 100, 100),

                new(8, "L_PRG_2", "Practical 2.2: Stack vs Heap Memory Allocation Profiler",
                    "Demonstrate value-type stack allocation vs reference-type heap allocation.",
                    "1. Create a `struct PointValue` (allocated on stack).\n2. Create a `class PointRef` (allocated on heap).",
                    "public struct PointValue { public int X; public int Y; }\npublic class PointRef { public int X; public int Y; }",
                    "struct;class;PointValue;PointRef", "CodeExecution", "Structs reside on the stack; classes allocate on the managed heap.", 100, 100),

                new(9, "L_PRG_3", "Practical 2.3: Clean OOP Abstraction & Polymorphism",
                    "Define an `IDevice` interface and implement `Server` and `Workstation` classes with polymorphism.",
                    "1. Define `string Boot();` in `IDevice`.\n2. Implement `Server` and `Workstation` with unique boot messages.",
                    "public interface IDevice { string Boot(); }\npublic class Server : IDevice { public string Boot() => \"Server booting system daemons...\"; }\npublic class Workstation : IDevice { public string Boot() => \"Workstation loading desktop GUI...\"; }",
                    "interface IDevice;Server : IDevice;Workstation : IDevice;Boot()", "CodeExecution", "Implement interface methods polymorphically.", 100, 100),

                new(10, "L_PRG_4", "Practical 2.4: High-Performance Binary Search Algorithm",
                    "Implement a zero-allocation binary search algorithm running in O(log N) time.",
                    "1. Function `BinarySearch(int[] array, int target)`.\n2. Return index if found, or -1 if absent.",
                    "public static int BinarySearch(int[] arr, int target)\n{\n    int low = 0, high = arr.Length - 1;\n    while (low <= high)\n    {\n        int mid = low + (high - low) / 2;\n        if (arr[mid] == target) return mid;\n        if (arr[mid] < target) low = mid + 1;\n        else high = mid - 1;\n    }\n    return -1;\n}",
                    "low;high;mid;BinarySearch", "CodeExecution", "Calculate mid = low + (high - low) / 2 to avoid integer overflow.", 100, 100),

                new(11, "L_PRG_5", "Practical 2.5: Lock-Free Thread-Safe Atomic Counter",
                    "Build an ultra-fast lock-free counter using CPU atomic Compare-And-Swap (CAS).",
                    "1. Implement `Increment(ref int target)` using `Interlocked.CompareExchange`.\n2. Do NOT use lock or mutex statements.",
                    "public static void IncrementAtomic(ref int target)\n{\n    int current, updated;\n    do {\n        current = target;\n        updated = current + 1;\n    } while (Interlocked.CompareExchange(ref target, updated, current) != current);\n}",
                    "Interlocked.CompareExchange;current;updated", "CodeExecution", "Loop with CompareExchange until atomic CAS succeeds.", 100, 100),

                new(12, "L_PRG_6", "Practical 2.6: SIMD 8-Way Parallel Vector Addition",
                    "Use hardware SIMD vectors to compute the parallel sum of two float arrays.",
                    "1. Use `System.Numerics.Vector<float>`.\n2. Add corresponding vectors simultaneously.",
                    "using System.Numerics;\npublic static float[] SimdAdd(float[] a, float[] b)\n{\n    var res = new float[a.Length];\n    int simLen = Vector<float>.Count;\n    for (int i = 0; i <= a.Length - simLen; i += simLen)\n    {\n        var va = new Vector<float>(a, i);\n        var vb = new Vector<float>(b, i);\n        (va + vb).CopyTo(res, i);\n    }\n    return res;\n}",
                    "Vector<float>;simLen;CopyTo", "CodeExecution", "Construct Vector<float> slices and sum them in a single instruction.", 100, 100),

                // =========================================================================
                // 3. SOFTWARE ENGINEERING & DEVOPS (SW101)
                // =========================================================================
                new(13, "L_SW_1", "Practical 3.1: Git Commit Graph DAG Simulator",
                    "Simulate a Git commit chain by linking commit nodes with cryptographic parent SHA-1 hashes.",
                    "1. Create `CommitNode(string hash, string parentHash, string message)`.\n2. Chain 3 commits together.",
                    "public record CommitNode(string Hash, string? ParentHash, string Message);\nvar c1 = new CommitNode(\"a1b2c3d\", null, \"Initial commit\");\nvar c2 = new CommitNode(\"e4f5g6h\", \"a1b2c3d\", \"Add core logic\");\nvar c3 = new CommitNode(\"i7j8k9l\", \"e4f5g6h\", \"Release v1.0\");",
                    "CommitNode;ParentHash;Initial commit", "CodeExecution", "Model commits as a Directed Acyclic Graph.", 100, 100),

                new(14, "L_SW_2", "Practical 3.2: Compiler Lexer Tokenizer",
                    "Write a lexical analyzer that splits source code into Number, Identifier, and Operator tokens.",
                    "1. Tokenize expression: `x = 42 + y`.\n2. Return list of recognized token types.",
                    "public record Token(string Type, string Value);\npublic static List<Token> Lex(string code)\n{\n    var tokens = new List<Token>();\n    foreach (var part in code.Split(' ', StringSplitOptions.RemoveEmptyEntries))\n    {\n        if (int.TryParse(part, out _)) tokens.Add(new Token(\"NUMBER\", part));\n        else if (part == \"=\" || part == \"+\") tokens.Add(new Token(\"OPERATOR\", part));\n        else tokens.Add(new Token(\"IDENTIFIER\", part));\n    }\n    return tokens;\n}",
                    "Token;NUMBER;OPERATOR;IDENTIFIER", "CodeExecution", "Categorize numbers, operators, and identifiers.", 100, 100),

                new(15, "L_SW_3", "Practical 3.3: Shortest Job First (SJF) CPU Scheduler",
                    "Implement the Shortest Job First (SJF) scheduling algorithm to minimize average process wait time.",
                    "1. Sort processes by BurstTime.\n2. Calculate wait times and average wait time.",
                    "public record Process(int Pid, int BurstTime);\npublic static double CalculateAverageWait(List<Process> list)\n{\n    var sorted = list.OrderBy(p => p.BurstTime).ToList();\n    int wait = 0, total = 0;\n    foreach (var p in sorted) { total += wait; wait += p.BurstTime; }\n    return (double)total / list.Count;\n}",
                    "OrderBy;BurstTime;total;CalculateAverageWait", "CodeExecution", "Sort by BurstTime ascending before accumulating wait times.", 100, 100),

                new(16, "L_SW_4", "Practical 3.4: CAP Theorem Trade-Off Configurator",
                    "Define a distributed cluster configuration that explicitly chooses CP (Consistency/Partition Tolerance) or AP (Availability).",
                    "1. Configure cluster mode as either 'CP' (Banking) or 'AP' (Social Feed).",
                    "public static string GetClusterBehavior(string capChoice)\n{\n    return capChoice switch\n    {\n        \"CP\" => \"Consistency Guaranteed. Node rejects writes during network partition.\",\n        \"AP\" => \"High Availability Guaranteed. Nodes accept writes; eventual consistency.\",\n        _ => \"Invalid choice. CAP theorem permits only two.\"\n    };\n}",
                    "Consistency;Availability;partition", "CodeExecution", "In partition tolerance, choose Consistency (CP) or Availability (AP).", 100, 100),

                new(17, "L_SW_5", "Practical 3.5: Static Single Assignment (SSA) Code Transformer",
                    "Convert a sequential variable assignment sequence into compiler SSA form (every variable assigned once).",
                    "1. Transform: `x = 5; x = x + 1; x = x * 2;`\n2. Output SSA variables: `x_1`, `x_2`, `x_3`.",
                    "string ssaOutput = \"x_1 = 5; x_2 = x_1 + 1; x_3 = x_2 * 2;\";",
                    "x_1;x_2;x_3", "CodeExecution", "Increment version suffixes for every re-assignment.", 100, 100),

                new(18, "L_SW_6", "Practical 3.6: Raft Quorum Majority Election Verifier",
                    "Write a function that determines whether a Raft candidate has achieved a majority quorum in an N-node cluster.",
                    "1. Quorum formula: Majority = floor(N / 2) + 1.\n2. Check if receivedVotes >= quorum.",
                    "public static bool HasRaftQuorum(int totalNodes, int receivedVotes)\n{\n    int quorum = (totalNodes / 2) + 1;\n    return receivedVotes >= quorum;\n}",
                    "quorum;totalNodes;receivedVotes", "CodeExecution", "A 5-node cluster requires at least 3 votes for quorum.", 100, 100),

                // =========================================================================
                // 4. HARDWARE ASSEMBLY (HW101)
                // =========================================================================
                new(19, "L_HW_1", "Practical 4.1: PC Power Supply Unit (PSU) Wattage Sizing",
                    "Calculate total system TDP power draw and add a 30% safety headroom for PSU sizing.",
                    "1. CPU = 125W, GPU = 285W, Motherboard/RAM/Drives = 60W.\n2. Total TDP = 470W. Add 30% headroom -> Recommended PSU: 650W or 750W.",
                    "int cpuTdp = 125, gpuTdp = 285, baseTdp = 60;\nint totalTdp = cpuTdp + gpuTdp + baseTdp;\nint recommendedPsu = (int)(totalTdp * 1.30); // 611W -> Select 650W or 750W",
                    "totalTdp;recommendedPsu;1.3", "CodeExecution", "Multiply baseline TDP by 1.3 for transient power spikes.", 100, 100),

                new(20, "L_HW_2", "Practical 4.2: Motherboard VRM Phase Efficiency Calculation",
                    "Calculate per-phase current load for a 12-phase VRM delivering 240 Amperes to an overclocked CPU.",
                    "1. CurrentPerPhase = TotalCurrent / PhaseCount.\n2. 240A / 12 phases = 20 Amperes per phase (safe and cool).",
                    "double totalAmps = 240.0;\nint phases = 12;\ndouble ampsPerPhase = totalAmps / phases; // 20.0 Amps",
                    "phases;ampsPerPhase;20", "CodeExecution", "More phases distribute thermal dissipation evenly.", 100, 100),

                new(21, "L_HW_3", "Practical 4.3: Storage Throughput Benchmark Comparison",
                    "Compare transfer times for a 50 GB file between SATA III (550 MB/s) and NVMe Gen4 (7,000 MB/s).",
                    "1. SATA III: 50,000 MB / 550 MB/s = ~90.9 seconds.\n2. NVMe Gen4: 50,000 MB / 7,000 MB/s = ~7.1 seconds.",
                    "double fileSizeMb = 50000.0;\ndouble sataSeconds = fileSizeMb / 550.0;   // ~90.9s\ndouble nvmeSeconds = fileSizeMb / 7000.0;  // ~7.1s",
                    "sataSeconds;nvmeSeconds;550;7000", "CodeExecution", "NVMe PCIe storage is over 12x faster than legacy SATA.", 100, 100),

                new(22, "L_HW_4", "Practical 4.4: Motherboard POST Diagnostic Beep Code Triage",
                    "Write a diagnostic lookup function for AMI/Award BIOS POST beep code sequences.",
                    "1. Handle: '1 Long, 2 Short' (GPU error), 'Continuous Short' (RAM error), '1 Short' (Normal boot).",
                    "public static string DiagnosePost(string beepPattern)\n{\n    return beepPattern switch\n    {\n        \"1 Long, 2 Short\" => \"GPU / Video Card Fault\",\n        \"Continuous Short\" => \"RAM Memory Seating / Power Fault\",\n        \"1 Short\" => \"POST Succeeded: Normal Boot\",\n        _ => \"Unknown Beep Code\"\n    };\n}",
                    "GPU;RAM;Normal Boot", "CodeExecution", "Match common POST error patterns.", 100, 100),

                new(23, "L_HW_5", "Practical 4.5: Silicon V/F Frequency Curve Calculation",
                    "Calculate dynamic CPU power scaling using formula P = C * V^2 * f.",
                    "1. Compare 4.0 GHz @ 1.15V vs 5.2 GHz @ 1.35V.\n2. Show that power increases non-linearly due to the V^2 factor.",
                    "double c = 1.0; // Capacitance factor\ndouble pBase = c * Math.Pow(1.15, 2) * 4.0;\ndouble pOc = c * Math.Pow(1.35, 2) * 5.2;\ndouble powerIncreasePct = ((pOc - pBase) / pBase) * 100.0;",
                    "Math.Pow;powerIncreasePct;pBase;pOc", "CodeExecution", "Voltage squared exponentially increases heat generation.", 100, 100),

                new(24, "L_HW_6", "Practical 4.6: 2nm GAAFET Nanosheet Electrostatic Model",
                    "Model gate electrostatic contact area for Planar (1 side), FinFET (3 sides), and GAAFET (4 sides).",
                    "1. Calculate effective contact percentage: Planar=25%, FinFET=75%, GAAFET=100%.",
                    "int planarSides = 1, finfetSides = 3, gaafetSides = 4;\ndouble gaafetControlPct = (gaafetSides / 4.0) * 100.0; // 100% full gate enclosure",
                    "gaafetSides;gaafetControlPct;100", "CodeExecution", "GAAFET encircles the nanosheet on all 4 sides.", 100, 100),

                // =========================================================================
                // 5. MICROARCHITECTURE & BUSES (HW201)
                // =========================================================================
                new(25, "L_HW2_1", "Practical 5.1: SMT Logical Core Capacity Calculator",
                    "Calculate total logical worker threads on an 8-core CPU with SMT enabled vs disabled.",
                    "1. PhysicalCores = 8.\n2. With SMT: 8 * 2 = 16 logical processors.",
                    "int physicalCores = 8;\nint threadsPerCore = 2;\nint logicalProcessors = physicalCores * threadsPerCore; // 16",
                    "physicalCores;logicalProcessors;16", "CodeExecution", "SMT exposes 2 logical threads per physical execution core.", 100, 100),

                new(26, "L_HW2_2", "Practical 5.2: Multi-Level CPU Cache Hierarchy Latency Simulation",
                    "Calculate the weighted average memory access time (AMAT) given L1, L2, L3, and RAM latencies.",
                    "1. L1 (1ns, 95% hit), L2 (4ns, 98% cumulative), RAM (60ns). Calculate AMAT.",
                    "double l1Hit = 0.95, l1Lat = 1.0;\ndouble l2Hit = 0.04, l2Lat = 4.0;\ndouble ramMiss = 0.01, ramLat = 60.0;\ndouble amat = (l1Hit * l1Lat) + (l2Hit * l2Lat) + (ramMiss * ramLat); // ~1.71 ns",
                    "amat;l1Hit;ramMiss;l1Lat", "CodeExecution", "AMAT = HitTime + (MissRate * MissPenalty).", 100, 100),

                new(27, "L_HW2_3", "Practical 5.3: RAM First-Word Latency Formula",
                    "Calculate the real-world latency in nanoseconds using True Latency = (CAS * 2000) / DataRate.",
                    "1. Compute DDR4-3200 CL16: (16 * 2000) / 3200 = 10.0 ns.\n2. Compute DDR5-6000 CL30: (30 * 2000) / 6000 = 10.0 ns.",
                    "double ddr4Latency = (16.0 * 2000.0) / 3200.0; // 10.0 ns\ndouble ddr5Latency = (30.0 * 2000.0) / 6000.0; // 10.0 ns",
                    "ddr4Latency;ddr5Latency;2000;10", "CodeExecution", "True first-word latency in nanoseconds depends on clock cycle time.", 100, 100),

                new(28, "L_HW2_4", "Practical 5.4: PCIe Bandwidth Across Lanes and Generations",
                    "Calculate theoretical maximum bandwidth for PCIe Gen 3, Gen 4, and Gen 5 across x16 lanes.",
                    "1. Gen3 x16 = ~15.75 GB/s, Gen4 x16 = ~31.5 GB/s, Gen5 x16 = ~63.0 GB/s.",
                    "double gen3x16 = 0.985 * 16;\ndouble gen4x16 = 1.969 * 16; // ~31.5 GB/s\ndouble gen5x16 = 3.938 * 16; // ~63.0 GB/s",
                    "gen3x16;gen4x16;gen5x16;31.5", "CodeExecution", "Each generation of PCIe doubles transfer speed per lane.", 100, 100),

                new(29, "L_HW2_5", "Practical 5.5: False Sharing Elimination with 64-Byte Cache Line Padding",
                    "Structure a data struct with padding so two thread counters never share the same 64-byte L1 cache line.",
                    "1. Use `[StructLayout(LayoutKind.Explicit)]` or 64-byte padding bytes.",
                    "using System.Runtime.InteropServices;\n[StructLayout(LayoutKind.Explicit, Size = 128)]\npublic struct PaddedCounter\n{\n    [FieldOffset(0)] public int CounterA;\n    [FieldOffset(64)] public int CounterB; // Isolated across separate 64B cache lines!\n}",
                    "StructLayout;FieldOffset(64);CounterA;CounterB", "CodeExecution", "Placing variables 64 bytes apart prevents false sharing cache bouncing.", 100, 100),

                new(30, "L_HW2_6", "Practical 5.6: GPU Tensor Core Fused Multiply-Add (FMA) Computation",
                    "Simulate an FP16 Tensor Core matrix multiply-accumulate operation: D = (A * B) + C.",
                    "1. Implement `ComputeFma(float a, float b, float c)`.\n2. Return fused result.",
                    "public static float ComputeFma(float a, float b, float c) => (a * b) + c;",
                    "ComputeFma;a * b + c", "CodeExecution", "Fused multiply-add computes product and sum in a single pass.", 100, 100),

                // =========================================================================
                // 6. NETWORKING & CCNA (NET101)
                // =========================================================================
                new(31, "L_NET_1", "Practical 6.1: OSI 7-Layer Encapsulation Trace",
                    "Trace the protocol encapsulation header added at each layer of the OSI model.",
                    "1. Application: HTTP/Payload.\n2. Transport: TCP Port.\n3. Network: IP Address.\n4. Data Link: Ethernet MAC.",
                    "public static string Encapsulate(string payload, string srcPort, string srcIp, string srcMac)\n{\n    return $\"[MAC: {srcMac}] -> [IP: {srcIp}] -> [PORT: {srcPort}] -> {payload}\";\n}",
                    "MAC;IP;PORT;payload", "CodeExecution", "Headers wrap from inner application to outer Ethernet frame.", 100, 100),

                new(32, "L_NET_2", "Practical 6.2: IPv4 /24 Subnet Calculator",
                    "Write a function that calculates the Network Address and Broadcast Address for an IPv4 address with /24 subnet mask.",
                    "1. Given `192.168.1.45`.\n2. Network = `192.168.1.0`.\n3. Broadcast = `192.168.1.255`.\n4. Usable Hosts = 254.",
                    "public static (string net, string bcast, int hosts) CalculateSubnet24(string ip)\n{\n    var parts = ip.Split('.');\n    string baseNet = $\"{parts[0]}.{parts[1]}.{parts[2]}\";\n    return ($\"{baseNet}.0\", $\"{baseNet}.255\", 254);\n}",
                    "CalculateSubnet24;254;255", "CodeExecution", "A /24 subnet has 256 total addresses, with 254 usable for hosts.", 100, 100),

                new(33, "L_NET_3", "Practical 6.3: IEEE 802.1Q VLAN Tag Header Formatter",
                    "Format an Ethernet frame with an 802.1Q 12-bit VLAN ID tag (TPID 0x8100).",
                    "1. Validate VLAN ID is between 1 and 4094.\n2. Return formatted tag string.",
                    "public static string BuildVlanTag(int vlanId, int priority = 0)\n{\n    if (vlanId < 1 || vlanId > 4094) throw new ArgumentOutOfRangeException();\n    return $\"802.1Q Tag: TPID=0x8100, PCP={priority}, VID={vlanId}\";\n}",
                    "802.1Q;0x8100;VID", "CodeExecution", "VLAN IDs range from 1 to 4094, with TPID 0x8100.", 100, 100),

                new(34, "L_NET_4", "Practical 6.4: TCP 3-Way Handshake State Machine",
                    "Simulate the client-server TCP handshake state sequence.",
                    "1. Client sends SYN (SEQ=100).\n2. Server replies SYN-ACK (SEQ=500, ACK=101).\n3. Client confirms ACK (ACK=501).",
                    "public static List<string> SimulateTcpHandshake()\n{\n    return new List<string>\n    {\n        \"[1] Client -> Server: SYN (Seq=100)\",\n        \"[2] Server -> Client: SYN-ACK (Seq=500, Ack=101)\",\n        \"[3] Client -> Server: ACK (Seq=101, Ack=501) [ESTABLISHED]\"\n    };\n}",
                    "SYN;SYN-ACK;ACK;ESTABLISHED", "CodeExecution", "Handshake: SYN, SYN-ACK, ACK.", 100, 100),

                new(35, "L_NET_5", "Practical 6.5: BGP Anycast Routing Geo-Hop Selector",
                    "Simulate BGP Anycast routing selecting the datacenter with lowest hop count and latency.",
                    "1. Client at Frankfurt chooses between London (12ms), Tokyo (240ms), NYC (95ms).\n2. Select London.",
                    "var dcs = new (string dc, int latency)[] { (\"Tokyo\", 240), (\"London\", 12), (\"NYC\", 95) };\nvar best = dcs.OrderBy(d => d.latency).First(); // London selected",
                    "OrderBy;latency;best", "CodeExecution", "Anycast routes to the topologically closest data center.", 100, 100),

                new(36, "L_NET_6", "Practical 6.6: eBPF XDP Line-Rate Packet Filter Rule",
                    "Define an eBPF XDP filter rule that drops packets on malicious port 4444 before kernel sk_buff allocation.",
                    "1. Return `XDP_DROP` if destination port == 4444.\n2. Return `XDP_PASS` for all other traffic.",
                    "public static string XdpFilter(int destPort)\n{\n    if (destPort == 4444) return \"XDP_DROP\"; // Line-rate drop!\n    return \"XDP_PASS\";\n}",
                    "XDP_DROP;XDP_PASS;4444", "CodeExecution", "XDP intercepts packets at driver level before kernel allocation.", 100, 100),

                // =========================================================================
                // 7. CYBERSECURITY & ETHICAL HACKING (SEC101)
                // =========================================================================
                new(37, "L_SEC_1", "Practical 7.1: CIA Triad Threat Classification Engine",
                    "Classify cyber incidents into Confidentiality, Integrity, or Availability violations.",
                    "1. Ransomware encrypting DB -> Availability.\n2. Hacker altering bank balance -> Integrity.\n3. Data breach leaking passwords -> Confidentiality.",
                    "public static string ClassifyThreat(string incident)\n{\n    if (incident.Contains(\"leak\")) return \"Confidentiality\";\n    if (incident.Contains(\"alter\")) return \"Integrity\";\n    return \"Availability\";\n}",
                    "Confidentiality;Integrity;Availability", "CodeExecution", "Match CIA triad pillar to incident type.", 100, 100),

                new(38, "L_SEC_2", "Practical 7.2: Nmap Command Construction for Stealth SYN Scan",
                    "Construct an Nmap command to execute a stealth SYN scan (-sS) on target 192.168.1.100 without pinging (-Pn).",
                    "1. Write the exact command line string.\n2. Include `-sS`, `-Pn`, and the target IP.",
                    "nmap -sS -Pn 192.168.1.100 -p 1-1000",
                    "nmap;-sS;-Pn;192.168.1.100", "CommandSimulation", "Use nmap with -sS for SYN scan and -Pn to skip host discovery.", 100, 100),

                new(39, "L_SEC_3", "Practical 7.3: SQL Injection Vulnerability Remediation",
                    "Fix vulnerable dynamic SQL string concatenation by rewriting it with parameterized queries.",
                    "1. Replace `\"SELECT * FROM Users WHERE Username = '\" + input + \"'\"`\n2. Use `@user` parameter with `cmd.Parameters.AddWithValue`.",
                    "cmd.CommandText = \"SELECT * FROM Users WHERE Username = @user\";\ncmd.Parameters.AddWithValue(\"@user\", input);",
                    "@user;Parameters.AddWithValue", "CodeExecution", "Use parameterized queries to separate SQL logic from data.", 100, 100),

                new(40, "L_SEC_4", "Practical 7.4: Cryptographic Password Salting & SHA-256 Hashing",
                    "Implement secure password hashing using a cryptographic salt and SHA-256.",
                    "1. Combine password with a random salt.\n2. Hash the concatenated bytes with SHA-256.",
                    "using System.Security.Cryptography;\nusing System.Text;\npublic static string HashPassword(string password, string salt)\n{\n    using var sha = SHA256.Create();\n    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt));\n    return Convert.ToHexString(hash);\n}",
                    "SHA256.Create;ComputeHash;salt", "CodeExecution", "Always append a salt before computing the hash digest.", 100, 100),

                new(41, "L_SEC_5", "Practical 7.5: Return-Oriented Programming (ROP) Gadget Chain",
                    "Assemble a simulated ROP gadget sequence to pop argument into RDI and call system function.",
                    "1. Gadget 1: `pop rdi; ret` (loads /bin/sh address).\n2. Gadget 2: `call system`.",
                    "var ropChain = new List<string> { \"0x401122: pop rdi; ret\", \"0x7ffff7a: /bin/sh\", \"0x401190: call system\" };",
                    "pop rdi; ret;call system;/bin/sh", "CodeExecution", "Chain gadgets so register arguments are loaded before function call.", 100, 100),

                new(42, "L_SEC_6", "Practical 7.6: Post-Quantum Lattice Cryptography Validator",
                    "Identify NIST-approved Post-Quantum Cryptography standards resistant to Shor's algorithm.",
                    "1. Key Encapsulation: ML-KEM (Kyber).\n2. Digital Signatures: ML-DSA (Dilithium).",
                    "public static bool IsPostQuantum(string algo) => algo == \"ML-KEM\" || algo == \"ML-DSA\" || algo == \"CRYSTALS-Kyber\";",
                    "ML-KEM;ML-DSA", "CodeExecution", "NIST finalized ML-KEM (Kyber) and ML-DSA (Dilithium).", 100, 100),

                // =========================================================================
                // 8. LINUX SYSTEMS & KERNEL (LNX101)
                // =========================================================================
                new(43, "L_LNX_1", "Practical 8.1: Linux Filesystem Navigation & Log Inspection",
                    "Write bash commands to navigate to the system log directory and view the last 20 lines of the authentication log.",
                    "1. Command 1: change directory to `/var/log`.\n2. Command 2: `tail -n 20 auth.log`.",
                    "cd /var/log\ntail -n 20 auth.log",
                    "cd /var/log;tail", "CommandSimulation", "cd into /var/log and use tail -n 20.", 100, 100),

                new(44, "L_LNX_2", "Practical 8.2: Linux chmod Octal Permission Enforcement",
                    "Set secure permissions on an SSH private key `id_rsa` so only the owner can read/write it (octal 600).",
                    "1. Write the chmod command with octal 600.\n2. Target file: `~/.ssh/id_rsa`.",
                    "chmod 600 ~/.ssh/id_rsa",
                    "chmod 600;id_rsa", "CommandSimulation", "chmod 600 grants rw------- only to the file owner.", 100, 100),

                new(45, "L_LNX_3", "Practical 8.3: Linux Process Triage & Termination",
                    "Find the process ID of an unresponsive `nginx` daemon and send a graceful SIGTERM signal.",
                    "1. Find process: `ps aux | grep nginx`.\n2. Kill PID 1420 with SIGTERM (-15): `kill -15 1420`.",
                    "ps aux | grep nginx\nkill -15 1420",
                    "ps aux;grep nginx;kill -15", "CommandSimulation", "Use ps aux | grep and kill -15 for graceful termination.", 100, 100),

                new(46, "L_LNX_4", "Practical 8.4: Systemd Service Lifecycle & UFW Firewall Hardening",
                    "Start and enable the `nginx` service and allow HTTPS port 443 through UFW firewall.",
                    "1. `sudo systemctl enable --now nginx`\n2. `sudo ufw allow 443/tcp`",
                    "sudo systemctl enable --now nginx\nsudo ufw allow 443/tcp",
                    "systemctl;nginx;ufw allow 443", "CommandSimulation", "Use systemctl enable and ufw allow 443/tcp.", 100, 100),

                new(47, "L_LNX_5", "Practical 8.5: Linux cgroups v2 Memory Limit Configuration",
                    "Write the configuration setting to enforce a hard memory limit of 512 Megabytes on a cgroup slice.",
                    "1. Set `memory.max` in the cgroup directory.\n2. 512MB = 536870912 bytes.",
                    "echo \"536870912\" > /sys/fs/cgroup/sandbox/memory.max",
                    "memory.max;536870912", "CommandSimulation", "Write byte limit directly to memory.max.", 100, 100),

                new(48, "L_LNX_6", "Practical 8.6: Loadable Kernel Module (LKM) Skeleton",
                    "Write a standard Linux kernel module skeleton with `init_module` and `cleanup_module`.",
                    "1. Include `<linux/module.h>`.\n2. Return 0 in init_module and specify `MODULE_LICENSE(\"GPL\")`.",
                    "#include <linux/module.h>\nint init_module(void) { printk(KERN_INFO \"Module loaded\\n\"); return 0; }\nvoid cleanup_module(void) { printk(KERN_INFO \"Module unloaded\\n\"); }\nMODULE_LICENSE(\"GPL\");",
                    "linux/module.h;init_module;cleanup_module;MODULE_LICENSE", "CodeExecution", "Standard C kernel module boilerplate.", 100, 100),

                // =========================================================================
                // 9. ELECTRONICS & CIRCUITS (ELE101)
                // =========================================================================
                new(49, "L_ELE_1", "Practical 9.1: Ohm's Law Calculator Engine",
                    "Calculate Current given Voltage (12V) and Resistance (24 Ohms) using I = V / R.",
                    "1. Implement `CalculateCurrent(double v, double r)`.\n2. I = 12 / 24 = 0.5 Amperes.",
                    "public static double CalculateCurrent(double v, double r) => v / r;\ndouble current = CalculateCurrent(12.0, 24.0); // 0.5 A",
                    "CalculateCurrent;v / r;0.5", "CodeExecution", "Current equals Voltage divided by Resistance.", 100, 100),

                new(50, "L_ELE_2", "Practical 9.2: LED Current-Limiting Resistor Sizing",
                    "Calculate resistor value to safely power a 2.0V, 20mA (0.02A) LED from a 5.0V source.",
                    "1. Formula: R = (Vsource - Vled) / Iled.\n2. (5.0 - 2.0) / 0.02 = 150 Ohms.",
                    "double vSource = 5.0, vLed = 2.0, iLed = 0.02;\ndouble resistorOhms = (vSource - vLed) / iLed; // 150.0 Ohms",
                    "resistorOhms;vSource - vLed;150", "CodeExecution", "R = (5 - 2) / 0.02 = 150 Ohms.", 100, 100),

                new(51, "L_ELE_3", "Practical 9.3: Boolean Logic Gate Simulator (Half-Adder)",
                    "Implement a digital half-adder circuit using XOR for Sum and AND for Carry.",
                    "1. `Sum = A ^ B`.\n2. `Carry = A & B`.\n3. Test with A=1, B=1 -> Sum=0, Carry=1.",
                    "public static (bool sum, bool carry) HalfAdder(bool a, bool b)\n{\n    return (a ^ b, a & b);\n}",
                    "HalfAdder;a ^ b;a & b", "CodeExecution", "Sum is computed via XOR; carry is computed via AND.", 100, 100),

                new(52, "L_ELE_4", "Practical 9.4: Microcontroller PWM Duty Cycle Sizing",
                    "Convert a desired DC voltage (3.3V) into an 8-bit PWM duty cycle value (0 to 255) for a 5.0V microcontroller.",
                    "1. Duty cycle = (3.3 / 5.0) * 255 = ~168.\n2. Return duty value.",
                    "double targetV = 3.3, maxV = 5.0;\nint pwmDuty = (int)Math.Round((targetV / maxV) * 255); // 168",
                    "pwmDuty;255;168", "CodeExecution", "Multiply target voltage ratio by 255.", 100, 100),

                new(53, "L_ELE_5", "Practical 9.5: Verilog 8-Bit Counter Module",
                    "Write a Verilog HDL module describing an 8-bit sequential counter with active-high reset.",
                    "1. Clock input `clk`, reset `rst`, output `[7:0] count`.\n2. Reset sets count to 0; otherwise count increments.",
                    "module Counter(input clk, input rst, output reg [7:0] count);\n    always @(posedge clk or posedge rst) begin\n        if (rst) count <= 8'b0;\n        else count <= count + 1'b1;\n    end\nendmodule",
                    "module Counter;always @;posedge clk;count <=", "CodeExecution", "Sequential Verilog always block with non-blocking assignments.", 100, 100),

                new(54, "L_ELE_6", "Practical 9.6: Closed-Loop PID Controller Algorithm",
                    "Implement the proportional, integral, and derivative terms in a robotics motor PID controller.",
                    "1. Calculate error = setpoint - current.\n2. output = (Kp * error) + (Ki * integral) + (Kd * derivative).",
                    "public static double ComputePid(double setpoint, double current, double Kp, double Ki, double Kd, ref double integral, double prevError, double dt)\n{\n    double error = setpoint - current;\n    integral += error * dt;\n    double derivative = (error - prevError) / dt;\n    return (Kp * error) + (Ki * integral) + (Kd * derivative);\n}",
                    "ComputePid;error * dt;derivative;Kp * error", "CodeExecution", "Sum of proportional, integral, and derivative contributions.", 100, 100),

                // =========================================================================
                // 10. ARTIFICIAL INTELLIGENCE & ML (AI101)
                // =========================================================================
                new(55, "L_AI_1", "Practical 10.1: Linear Regression Prediction Model",
                    "Implement a 1D linear regression prediction function: y = (w * x) + b.",
                    "1. Given weight w = 2.5, bias b = 1.0.\n2. Compute y for input x = 4.0 -> y = 11.0.",
                    "public static double PredictLinear(double x, double w = 2.5, double b = 1.0) => (w * x) + b;\ndouble y = PredictLinear(4.0); // 11.0",
                    "PredictLinear;w * x + b", "CodeExecution", "Evaluate y = wx + b.", 100, 100),

                new(56, "L_AI_2", "Practical 10.2: Binary Classification Confusion Matrix",
                    "Calculate Precision and Recall from True Positives (TP=80), False Positives (FP=20), and False Negatives (FN=10).",
                    "1. Precision = TP / (TP + FP) = 80 / 100 = 80%.\n2. Recall = TP / (TP + FN) = 80 / 90 = 88.9%.",
                    "double tp = 80.0, fp = 20.0, fn = 10.0;\ndouble precision = tp / (tp + fp); // 0.80\ndouble recall = tp / (tp + fn);    // 0.889",
                    "precision;recall;tp / (tp + fp)", "CodeExecution", "Precision evaluates predicted positives; Recall evaluates actual positives.", 100, 100),

                new(57, "L_AI_3", "Practical 10.3: Artificial Neuron ReLU Activation & Gradient",
                    "Implement the ReLU activation function and its derivative (gradient) for backpropagation.",
                    "1. ReLU(x) = max(0, x).\n2. Derivative = 1 if x > 0 else 0.",
                    "public static double Relu(double x) => Math.Max(0.0, x);\npublic static double ReluDerivative(double x) => x > 0 ? 1.0 : 0.0;",
                    "Relu;ReluDerivative;Math.Max", "CodeExecution", "ReLU clamps negative values to zero; derivative is 1 for positive inputs.", 100, 100),

                new(58, "L_AI_4", "Practical 10.4: Offline LLM Temperature Sampling Logit Scaler",
                    "Scale logits by sampling temperature T before applying softmax.",
                    "1. Formula: scaled_logit = logit / temperature.\n2. Show that T=0.2 sharpens probabilities (deterministic), while T=1.5 flattens them (creative).",
                    "public static double[] ScaleLogits(double[] logits, double temperature)\n{\n    return logits.Select(l => l / temperature).ToArray();\n}",
                    "ScaleLogits;l / temperature", "CodeExecution", "Divide each raw logit by temperature.", 100, 100),

                new(59, "L_AI_5", "Practical 10.5: Multi-Head Scaled Dot-Product Attention Equation",
                    "Implement the scaled dot-product attention calculation: Softmax((Q * K^T) / sqrt(d_k)) * V.",
                    "1. Calculate scale factor = sqrt(d_k).\n2. Divide raw score by scale factor.",
                    "public static double ScaleAttentionScore(double dotProduct, int dK)\n{\n    return dotProduct / Math.Sqrt(dK);\n}",
                    "ScaleAttentionScore;Math.Sqrt(dK)", "CodeExecution", "Divide by sqrt(d_k) to prevent vanishing gradients in softmax.", 100, 100),

                new(60, "L_AI_6", "Practical 10.6: LoRA Rank Decomposition Parameter Reduction",
                    "Calculate the parameter reduction when decomposing a (4096 x 4096) weight matrix into LoRA rank r=8 matrices A and B.",
                    "1. Full weights = 4096 * 4096 = 16,777,216 params.\n2. LoRA weights = (4096 * 8) + (8 * 4096) = 65,536 params.\n3. Reduction = 99.6%!",
                    "int d = 4096, r = 8;\nlong fullParams = (long)d * d;           // 16,777,216\nlong loraParams = (long)d * r + (long)r * d; // 65,536\ndouble reductionPct = (1.0 - ((double)loraParams / fullParams)) * 100.0; // 99.6%",
                    "fullParams;loraParams;reductionPct", "CodeExecution", "LoRA cuts trainable parameter count by over 99%.", 100, 100),

                // =========================================================================
                // 11. IT TROUBLESHOOTING (IT101)
                // =========================================================================
                new(61, "L_IT_1", "Practical 11.1: Incident Triage 6-Step Plan Generator",
                    "Order the 6 CompTIA troubleshooting steps into the correct chronological sequence.",
                    "1. Steps: Identify -> Establish Theory -> Test Theory -> Action Plan -> Verify -> Document.",
                    "var steps = new List<string> { \"Identify\", \"Establish Theory\", \"Test Theory\", \"Action Plan\", \"Verify\", \"Document\" };",
                    "Identify;Establish Theory;Test Theory;Action Plan;Verify;Document", "CodeExecution", "Always identify problem before formulating and testing theories.", 100, 100),

                new(62, "L_IT_2", "Practical 11.2: Hard Drive SMART Health Assessment",
                    "Evaluate SMART attribute values to determine if a hard drive is at imminent risk of failure.",
                    "1. If `ReallocatedSectors > 50` or `PendingSectors > 10`, flag as 'CRITICAL_FAILURE_RISK'.",
                    "public static string CheckSmartHealth(int reallocated, int pending)\n{\n    if (reallocated > 50 || pending > 10) return \"CRITICAL_FAILURE_RISK\";\n    return \"HEALTHY\";\n}",
                    "CheckSmartHealth;CRITICAL_FAILURE_RISK;HEALTHY", "CodeExecution", "Non-zero reallocated sectors indicate physical platter decay.", 100, 100),

                new(63, "L_IT_3", "Practical 11.3: Network Connectivity Diagnostic Ladder",
                    "Implement the systematic ping ladder algorithm to isolate network failure points.",
                    "1. Step 1: 127.0.0.1 (Loopback).\n2. Step 2: Local IP (NIC).\n3. Step 3: Default Gateway (Router).\n4. Step 4: 8.8.8.8 (Internet).",
                    "public static string DiagnoseNetwork(bool pingLoopback, bool pingNic, bool pingGw, bool pingWan)\n{\n    if (!pingLoopback) return \"TCP/IP Stack Corrupt\";\n    if (!pingNic) return \"Network Adapter Fault\";\n    if (!pingGw) return \"Local Switch/Router Disconnected\";\n    if (!pingWan) return \"ISP Outage / Gateway Failure\";\n    return \"Network Fully Functional\";\n}",
                    "DiagnoseNetwork;TCP/IP Stack;Local Switch;Network Fully Functional", "CodeExecution", "Diagnose upwards from local stack to internet gateway.", 100, 100),

                new(64, "L_IT_4", "Practical 11.4: Windows BSOD Minidump Bugcheck Parser",
                    "Parse Windows stop codes and map to likely culprit components.",
                    "1. `0x0A` (IRQL_NOT_LESS_OR_EQUAL) -> Driver Fault.\n2. `0x50` (PAGE_FAULT_IN_NONPAGED_AREA) -> Defective RAM.",
                    "public static string AnalyzeBugcheck(string stopCode)\n{\n    return stopCode switch\n    {\n        \"0x0A\" => \"Driver Fault: Invalid Memory Access at high IRQL\",\n        \"0x50\" => \"Hardware Fault: Defective RAM or Corrupt FileSystem\",\n        _ => \"General System Exception\"\n    };\n}",
                    "AnalyzeBugcheck;Driver Fault;Defective RAM", "CodeExecution", "Map stop codes 0x0A and 0x50.", 100, 100),

                new(65, "L_IT_5", "Practical 11.5: SRE Error Budget Downtime Calculator",
                    "Calculate allowable monthly downtime in minutes for an SLO of 99.95% over a 30-day month (43,200 total minutes).",
                    "1. Error Budget = 100% - 99.95% = 0.05%.\n2. Allowable downtime = 43,200 * 0.0005 = 21.6 minutes.",
                    "double totalMinutesMonth = 30.0 * 24.0 * 60.0; // 43,200\ndouble slo = 0.9995;\ndouble errorBudgetMinutes = totalMinutesMonth * (1.0 - slo); // 21.6 minutes",
                    "totalMinutesMonth;errorBudgetMinutes;21.6", "CodeExecution", "Error budget defines acceptable unreliability for feature releases.", 100, 100),

                new(66, "L_IT_6", "Practical 11.6: Kubernetes Pod Scheduler Capacity Scoring",
                    "Implement a node scoring algorithm assigning an unscheduled pod to the worker node with highest available memory capacity.",
                    "1. Given NodeA (2GB free) and NodeB (6GB free).\n2. Select NodeB for pod needing 1GB memory.",
                    "var nodes = new (string name, int freeMem)[] { (\"NodeA\", 2048), (\"NodeB\", 6144) };\nvar selectedNode = nodes.OrderByDescending(n => n.freeMem).First().name; // NodeB",
                    "OrderByDescending;freeMem;selectedNode", "CodeExecution", "Scheduler assigns pods to the most capable node matching resource requests.", 100, 100)
            };

            list.AddRange(GetAdvancedExams());
            list.AddRange(GetGreyHatExams());
            return list;
        }
    }
}
