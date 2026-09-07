using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class K8sNode
    {
        public string Name { get; set; } = "";
        public int TotalCpuCores { get; set; }
        public int TotalMemoryGb { get; set; }
        public int UsedCpuCores { get; set; }
        public int UsedMemoryGb { get; set; }
        public Dictionary<string, string> Labels { get; set; } = new();
        public List<string> Taints { get; set; } = new();

        public int AvailableCpu => Math.Max(0, TotalCpuCores - UsedCpuCores);
        public int AvailableMemory => Math.Max(0, TotalMemoryGb - UsedMemoryGb);
    }

    public class K8sPod
    {
        public string Name { get; set; } = "";
        public int RequestCpuCores { get; set; }
        public int RequestMemoryGb { get; set; }
        public Dictionary<string, string> NodeSelector { get; set; } = new();
        public List<string> Tolerations { get; set; } = new();
    }

    public record NodeEvaluation(
        string NodeName,
        bool FilterPassed,
        string FilterReason,
        int Score,
        string ScoreReason
    );

    public record SchedulingDecision(
        string PodName,
        bool IsScheduled,
        string TargetNode,
        List<NodeEvaluation> Evaluations,
        string Summary
    );

    public static class K8sSchedulerService
    {
        public static List<K8sNode> GetDefaultClusterNodes()
        {
            return new List<K8sNode>
            {
                new K8sNode
                {
                    Name = "worker-pool-node-1",
                    TotalCpuCores = 4,
                    TotalMemoryGb = 8,
                    UsedCpuCores = 1,
                    UsedMemoryGb = 2,
                    Labels = new() { { "tier", "frontend" }, { "zone", "us-east-1a" } },
                    Taints = new()
                },
                new K8sNode
                {
                    Name = "worker-pool-node-2",
                    TotalCpuCores = 8,
                    TotalMemoryGb = 16,
                    UsedCpuCores = 3,
                    UsedMemoryGb = 6,
                    Labels = new() { { "tier", "backend" }, { "zone", "us-east-1b" } },
                    Taints = new()
                },
                new K8sNode
                {
                    Name = "gpu-accelerator-node-3",
                    TotalCpuCores = 16,
                    TotalMemoryGb = 32,
                    UsedCpuCores = 2,
                    UsedMemoryGb = 4,
                    Labels = new() { { "accelerator", "nvidia-tensor" }, { "tier", "compute" } },
                    Taints = new() { "specialized=gpu:NoSchedule" }
                }
            };
        }

        public static SchedulingDecision SchedulePod(K8sPod pod, List<K8sNode>? clusterNodes = null)
        {
            var nodes = clusterNodes ?? GetDefaultClusterNodes();
            var evals = new List<NodeEvaluation>();
            K8sNode? bestNode = null;
            int highestScore = -1;

            foreach (var node in nodes)
            {
                // Stage 1: Filtering / Predicates
                bool fits = true;
                string failReason = "";

                // Check CPU
                if (pod.RequestCpuCores > node.AvailableCpu)
                {
                    fits = false;
                    failReason = $"Insufficient CPU: Requested {pod.RequestCpuCores} cores, only {node.AvailableCpu} available.";
                }
                // Check RAM
                else if (pod.RequestMemoryGb > node.AvailableMemory)
                {
                    fits = false;
                    failReason = $"Insufficient Memory: Requested {pod.RequestMemoryGb}GB, only {node.AvailableMemory}GB available.";
                }
                // Check NodeSelector
                else if (pod.NodeSelector.Count > 0)
                {
                    foreach (var kv in pod.NodeSelector)
                    {
                        if (!node.Labels.TryGetValue(kv.Key, out var val) || val != kv.Value)
                        {
                            fits = false;
                            failReason = $"NodeSelector mismatch: Requires {kv.Key}={kv.Value}.";
                            break;
                        }
                    }
                }

                // Check Taints
                if (fits && node.Taints.Count > 0)
                {
                    foreach (var taint in node.Taints)
                    {
                        if (!pod.Tolerations.Contains(taint))
                        {
                            fits = false;
                            failReason = $"Taint not tolerated: {taint}.";
                            break;
                        }
                    }
                }

                // Stage 2: Prioritizing / Scoring
                int score = 0;
                string scoreReason;

                if (!fits)
                {
                    scoreReason = "Excluded in Filtering stage (Predicates failed).";
                }
                else
                {
                    // LeastRequestedPriority: (AvailableCpu / TotalCpu + AvailableMem / TotalMem) / 2 * 10
                    double cpuRatio = (double)node.AvailableCpu / node.TotalCpuCores;
                    double memRatio = (double)node.AvailableMemory / node.TotalMemoryGb;
                    score = (int)Math.Round(((cpuRatio + memRatio) / 2.0) * 100.0);
                    scoreReason = $"LeastRequestedPriority Score: {score}/100 (Free CPU: {node.AvailableCpu}/{node.TotalCpuCores}, Free RAM: {node.AvailableMemory}/{node.TotalMemoryGb}GB).";

                    if (score > highestScore)
                    {
                        highestScore = score;
                        bestNode = node;
                    }
                }

                evals.Add(new NodeEvaluation(
                    NodeName: node.Name,
                    FilterPassed: fits,
                    FilterReason: fits ? "Passed Predicates" : failReason,
                    Score: score,
                    ScoreReason: scoreReason
                ));
            }

            if (bestNode != null)
            {
                bestNode.UsedCpuCores += pod.RequestCpuCores;
                bestNode.UsedMemoryGb += pod.RequestMemoryGb;

                return new SchedulingDecision(
                    PodName: pod.Name,
                    IsScheduled: true,
                    TargetNode: bestNode.Name,
                    Evaluations: evals,
                    Summary: $"kube-scheduler: Successfully bound Pod '{pod.Name}' to Node '{bestNode.Name}' with score {highestScore}."
                );
            }
            else
            {
                return new SchedulingDecision(
                    PodName: pod.Name,
                    IsScheduled: false,
                    TargetNode: "None (Pending)",
                    Evaluations: evals,
                    Summary: $"kube-scheduler: Pod '{pod.Name}' cannot be scheduled on any node (Status: Pending - 0/3 nodes available)."
                );
            }
        }
    }
}
