using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record DeadlockAnalysisResult(
        bool DeadlockDetected,
        List<string> CycleNodes,
        string ResourceGraphAscii,
        string PreventionRecommendation
    );

    public record DiningPhilosophersStep(
        int StepNumber,
        string Action,
        List<string> PhilosopherStates,
        bool IsDeadlocked
    );

    public record AtomicCasTrace(
        int InitialValue,
        int ExpectedValue,
        int DesiredValue,
        int FinalValue,
        bool CasSucceeded,
        string Explanation
    );

    public class ConcurrencySimulatorService
    {
        public AtomicCasTrace SimulateAtomicCas(int initial, int expected, int desired)
        {
            int current = initial;
            bool success = (current == expected);
            if (success)
            {
                current = desired;
            }

            string exp = success
                ? $"✅ Atomic CAS Succeeded: Target memory matched expected {expected}. Atomic hardware bus lock updated value to {desired} in a single clock cycle without mutex locks!"
                : $"❌ Atomic CAS Failed: Target memory had {initial}, but expected was {expected}. Value was modified concurrently by another core; thread must retry (lock-free loop).";

            return new AtomicCasTrace(initial, expected, desired, current, success, exp);
        }

        public DeadlockAnalysisResult DetectDeadlockInGraph(Dictionary<string, List<string>> waitGraph)
        {
            // Detect cycle using DFS
            var visited = new HashSet<string>();
            var recStack = new HashSet<string>();
            var cycle = new List<string>();

            bool HasCycle(string node)
            {
                visited.Add(node);
                recStack.Add(node);

                if (waitGraph.ContainsKey(node))
                {
                    foreach (var neighbor in waitGraph[node])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            if (HasCycle(neighbor))
                            {
                                cycle.Add(neighbor);
                                return true;
                            }
                        }
                        else if (recStack.Contains(neighbor))
                        {
                            cycle.Add(neighbor);
                            return true;
                        }
                    }
                }

                recStack.Remove(node);
                return false;
            }

            bool deadlocked = false;
            foreach (var node in waitGraph.Keys)
            {
                if (!visited.Contains(node))
                {
                    if (HasCycle(node))
                    {
                        cycle.Add(node);
                        deadlocked = true;
                        break;
                    }
                }
            }

            cycle.Reverse();

            var sb = new StringBuilder();
            sb.AppendLine("=== RESOURCE-ALLOCATION GRAPH (RAG) CYCLE DETECTOR ===");
            foreach (var kvp in waitGraph)
            {
                sb.AppendLine($" • [{kvp.Key}] ---> Waits for ---> [{string.Join(", ", kvp.Value)}]");
            }

            string rec = deadlocked
                ? $"🚨 DEADLOCK DETECTED! Circular wait chain identified: {string.Join(" -> ", cycle)}. Solution: Enforce global lock hierarchy (always acquire resources in ascending memory address order) or implement lock timeouts."
                : "✅ NO DEADLOCK DETECTED: Resource allocation graph is a Directed Acyclic Graph (DAG). All threads will make progress.";

            return new DeadlockAnalysisResult(deadlocked, cycle, sb.ToString(), rec);
        }

        public List<DiningPhilosophersStep> SimulateDiningPhilosophers(bool useHierarchySolution)
        {
            var steps = new List<DiningPhilosophersStep>();

            if (!useHierarchySolution)
            {
                // Naive strategy: All 5 philosophers simultaneously pick left fork
                steps.Add(new DiningPhilosophersStep(1, "All 5 philosophers become hungry", new List<string> { "Hungry", "Hungry", "Hungry", "Hungry", "Hungry" }, false));
                steps.Add(new DiningPhilosophersStep(2, "Philosopher 0 acquires Fork 0 (Left)", new List<string> { "Holding Fork 0", "Hungry", "Hungry", "Hungry", "Hungry" }, false));
                steps.Add(new DiningPhilosophersStep(3, "Philosopher 1 acquires Fork 1 (Left)", new List<string> { "Holding Fork 0", "Holding Fork 1", "Hungry", "Hungry", "Hungry" }, false));
                steps.Add(new DiningPhilosophersStep(4, "Philosopher 2 acquires Fork 2 (Left)", new List<string> { "Holding Fork 0", "Holding Fork 1", "Holding Fork 2", "Hungry", "Hungry" }, false));
                steps.Add(new DiningPhilosophersStep(5, "Philosopher 3 acquires Fork 3 (Left)", new List<string> { "Holding Fork 0", "Holding Fork 1", "Holding Fork 2", "Holding Fork 3", "Hungry" }, false));
                steps.Add(new DiningPhilosophersStep(6, "Philosopher 4 acquires Fork 4 (Left)", new List<string> { "Holding Fork 0", "Holding Fork 1", "Holding Fork 2", "Holding Fork 3", "Holding Fork 4" }, false));
                steps.Add(new DiningPhilosophersStep(7, "All philosophers attempt to acquire Right Fork -> BLOCKED! Circular Wait Condition satisfied!", new List<string> { "Blocked on 1", "Blocked on 2", "Blocked on 3", "Blocked on 4", "Blocked on 0" }, true));
            }
            else
            {
                // Dijkstra Resource Hierarchy: Philosopher 4 picks Fork 0 first instead of Fork 4
                steps.Add(new DiningPhilosophersStep(1, "All 5 philosophers become hungry with Resource Hierarchy rule active", new List<string> { "Hungry", "Hungry", "Hungry", "Hungry", "Hungry" }, false));
                steps.Add(new DiningPhilosophersStep(2, "Philosopher 0 acquires Fork 0 (Lowest) and Fork 1", new List<string> { "Eating 🍝", "Thinking", "Thinking", "Thinking", "Thinking" }, false));
                steps.Add(new DiningPhilosophersStep(3, "Philosopher 0 finishes eating and releases Fork 0 & 1", new List<string> { "Thinking", "Hungry", "Thinking", "Thinking", "Thinking" }, false));
                steps.Add(new DiningPhilosophersStep(4, "Philosopher 1 acquires Fork 1 and Fork 2", new List<string> { "Thinking", "Eating 🍝", "Thinking", "Thinking", "Thinking" }, false));
                steps.Add(new DiningPhilosophersStep(5, "All philosophers dine sequentially with 0% deadlock risk", new List<string> { "Full", "Full", "Full", "Full", "Full" }, false));
            }

            return steps;
        }
    }
}
