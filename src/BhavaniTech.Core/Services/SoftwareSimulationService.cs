using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public record ProcessItem(string ProcessName, int BurstTimeMs, int Priority);
    public record ProcessScheduleResult(string Algorithm, List<string> ExecutionSequence, double AverageWaitTimeMs, double AverageTurnaroundTimeMs);

    public record CodeToken(string TokenType, string Literal, int LineNumber);
    public record LexerResult(int TotalTokens, List<CodeToken> Tokens);

    public record GitCommitRecord(string CommitHash, string Author, string Message, DateTime Timestamp);

    public record ContainerIsolationResult(
        string ContainerName,
        int HostPid,
        int ContainerPid,
        string RootFsMount,
        string VirtualNetInterface,
        string AssignedIp,
        string CgroupsCpuLimit,
        string CgroupsMemoryLimit,
        string KernelExplanation
    );

    public static class SoftwareSimulationService
    {
        public static ProcessScheduleResult SimulateProcessScheduling(string algorithm, List<ProcessItem> processes)
        {
            var seq = new List<string>();
            double totalWait = 0;
            double totalTurnaround = 0;
            double currentTime = 0;

            var orderedList = algorithm.ToUpperInvariant() switch
            {
                "SJF" => processes.OrderBy(p => p.BurstTimeMs).ToList(),
                "PRIORITY" => processes.OrderBy(p => p.Priority).ToList(),
                _ => processes.ToList() // FIFO
            };

            foreach (var p in orderedList)
            {
                double wait = currentTime;
                double turnaround = wait + p.BurstTimeMs;
                totalWait += wait;
                totalTurnaround += turnaround;
                currentTime += p.BurstTimeMs;

                seq.Add($"[{p.ProcessName}] Burst: {p.BurstTimeMs}ms | Start: {wait}ms -> End: {currentTime}ms (Wait: {wait}ms)");
            }

            double avgWait = processes.Count > 0 ? totalWait / processes.Count : 0;
            double avgTurnaround = processes.Count > 0 ? totalTurnaround / processes.Count : 0;

            return new ProcessScheduleResult(algorithm, seq, Math.Round(avgWait, 2), Math.Round(avgTurnaround, 2));
        }

        public static LexerResult RunCompilerLexer(string sourceCode)
        {
            var tokens = new List<CodeToken>();
            var lines = sourceCode.Split('\n');
            int lineNum = 1;

            string[] keywords = { "class", "public", "void", "int", "string", "if", "else", "for", "while", "return", "import", "def", "print" };

            foreach (var rawLine in lines)
            {
                var words = System.Text.RegularExpressions.Regex.Matches(rawLine, @"\b\w+\b|[+\-*/=(){}<>;]");
                foreach (System.Text.RegularExpressions.Match m in words)
                {
                    string word = m.Value;
                    string type = keywords.Contains(word) ? "KEYWORD" :
                                 (int.TryParse(word, out _) ? "NUMBER_LITERAL" :
                                 (word.Length == 1 && "+-*/=(){}<>;".Contains(word) ? "OPERATOR_SYMBOL" : "IDENTIFIER"));

                    tokens.Add(new CodeToken(type, word, lineNum));
                }
                lineNum++;
            }

            return new LexerResult(tokens.Count, tokens);
        }

        public static List<GitCommitRecord> RunSimulatedGitLog()
        {
            return new List<GitCommitRecord>
            {
                new GitCommitRecord("a1b2c3d", "Dharmesh Varia", "feat: Add Low-Hardware Performance Engine", DateTime.Now.AddDays(-3)),
                new GitCommitRecord("e5f6g7h", "Young Innovator", "feat: Integrate 100% Offline Local AI Tutor", DateTime.Now.AddDays(-2)),
                new GitCommitRecord("i9j0k1l", "Dharmesh Varia", "fix: Optimize SQLite WAL Journal & 500+ Troubleshooting Scenarios", DateTime.Now.AddDays(-1)),
                new GitCommitRecord("m2n3o4p", "Young Innovator", "feat: Integrate Software Engineering & OS Scheduler Simulator", DateTime.Now)
            };
        }

        // =====================================================================
        // LINUX NAMESPACES & CGROUPS V2 CONTAINER RUNTIME SIMULATOR
        // =====================================================================
        public static ContainerIsolationResult SimulateContainerNamespaces(string containerName = "bhavani-sandbox", int cpuQuotaPercent = 50, int memoryLimitMb = 512)
        {
            if (string.IsNullOrWhiteSpace(containerName)) containerName = "bhavani-app";
            if (cpuQuotaPercent <= 0) cpuQuotaPercent = 10;
            if (cpuQuotaPercent > 100) cpuQuotaPercent = 100;
            if (memoryLimitMb <= 0) memoryLimitMb = 64;

            int hostPid = 14820 + (Math.Abs(containerName.GetHashCode()) % 5000);
            int containerPid = 1; // Always PID 1 inside isolated PID namespace!
            string rootFs = $"/var/lib/containers/storage/overlay/{containerName}/merged";
            string veth = $"veth_{containerName.ToLowerInvariant()[..Math.Min(6, containerName.Length)]}";
            string assignedIp = $"10.244.0.{10 + (Math.Abs(containerName.GetHashCode()) % 200)}/24";

            // cgroups v2 format: cpu.max is [quota_us] [period_us]
            int quotaUs = cpuQuotaPercent * 1000;
            string cgroupsCpu = $"{quotaUs} 100000 ({cpuQuotaPercent}% of 1 CPU Core)";
            string cgroupsMem = $"{memoryLimitMb * 1024 * 1024} bytes ({memoryLimitMb} MB max - OOM Killer armed)";

            var explanation = new System.Text.StringBuilder();
            explanation.AppendLine($"=== CONTAINER ARCHITECTURE BREAKDOWN: {containerName} ===");
            explanation.AppendLine("[1] PID Namespace: Host sees PID " + hostPid + ", but container process sees itself as PID 1 (/init).");
            explanation.AppendLine("[2] Mount Namespace (pivot_root): Application only sees isolated rootfs at " + rootFs + ".");
            explanation.AppendLine("[3] Network Namespace: Dedicated virtual interface " + veth + " bridged to host, assigned " + assignedIp + ".");
            explanation.AppendLine("[4] cgroups v2 Enforced: CPU restricted to " + cpuQuotaPercent + "%, Memory capped at " + memoryLimitMb + " MB.");
            explanation.AppendLine("[5] Crucial Fact: Containers are NOT VMs! There is no hypervisor or guest OS kernel. Containers are isolated Linux processes governed directly by the host Linux kernel.");

            return new ContainerIsolationResult(
                containerName,
                hostPid,
                containerPid,
                rootFs,
                veth,
                assignedIp,
                cgroupsCpu,
                cgroupsMem,
                explanation.ToString()
            );
        }
    }
}
