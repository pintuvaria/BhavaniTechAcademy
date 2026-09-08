using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class OsSimulationService
    {
        // 1. Virtual Memory Paging Simulator
        public record PageFaultResult(int TotalReferences, int PageFaults, int PageHits, List<string> FrameTrace);

        public PageFaultResult SimulatePagingLRU(int[] pageReferences, int frameCapacity)
        {
            var frames = new List<int>();
            int faults = 0;
            int hits = 0;
            var trace = new List<string>();
            var recentUsage = new Dictionary<int, int>();

            for (int t = 0; t < pageReferences.Length; t++)
            {
                int page = pageReferences[t];
                if (frames.Contains(page))
                {
                    hits++;
                    recentUsage[page] = t;
                    trace.Add($"Time {t}: Hit on page {page}. Frames: [{string.Join(", ", frames)}]");
                }
                else
                {
                    faults++;
                    if (frames.Count < frameCapacity)
                    {
                        frames.Add(page);
                    }
                    else
                    {
                        int lruPage = frames.OrderBy(p => recentUsage[p]).First();
                        frames.Remove(lruPage);
                        recentUsage.Remove(lruPage);
                        frames.Add(page);
                    }
                    recentUsage[page] = t;
                    trace.Add($"Time {t}: Fault on page {page}. Frames: [{string.Join(", ", frames)}]");
                }
            }
            return new PageFaultResult(pageReferences.Length, faults, hits, trace);
        }

        public PageFaultResult SimulatePagingFIFO(int[] pageReferences, int frameCapacity)
        {
            var frames = new Queue<int>();
            int faults = 0;
            int hits = 0;
            var trace = new List<string>();

            for (int t = 0; t < pageReferences.Length; t++)
            {
                int page = pageReferences[t];
                if (frames.Contains(page))
                {
                    hits++;
                    trace.Add($"Time {t}: Hit on page {page}. Frames: [{string.Join(", ", frames)}]");
                }
                else
                {
                    faults++;
                    if (frames.Count >= frameCapacity)
                    {
                        frames.Dequeue();
                    }
                    frames.Enqueue(page);
                    trace.Add($"Time {t}: Fault on page {page}. Frames: [{string.Join(", ", frames)}]");
                }
            }
            return new PageFaultResult(pageReferences.Length, faults, hits, trace);
        }

        public PageFaultResult SimulatePagingOptimal(int[] pageReferences, int frameCapacity)
        {
            var frames = new List<int>();
            int faults = 0;
            int hits = 0;
            var trace = new List<string>();

            for (int t = 0; t < pageReferences.Length; t++)
            {
                int page = pageReferences[t];
                if (frames.Contains(page))
                {
                    hits++;
                    trace.Add($"Time {t}: Hit on page {page}. Frames: [{string.Join(", ", frames)}]");
                }
                else
                {
                    faults++;
                    if (frames.Count < frameCapacity)
                    {
                        frames.Add(page);
                    }
                    else
                    {
                        // Find page not used for the longest time in future
                        int victim = -1;
                        int farthest = -1;
                        foreach (var f in frames)
                        {
                            int nextUse = int.MaxValue;
                            for (int future = t + 1; future < pageReferences.Length; future++)
                            {
                                if (pageReferences[future] == f)
                                {
                                    nextUse = future;
                                    break;
                                }
                            }
                            if (nextUse > farthest)
                            {
                                farthest = nextUse;
                                victim = f;
                            }
                        }
                        frames.Remove(victim);
                        frames.Add(page);
                    }
                    trace.Add($"Time {t}: Fault on page {page}. Frames: [{string.Join(", ", frames)}]");
                }
            }
            return new PageFaultResult(pageReferences.Length, faults, hits, trace);
        }

        // Shortest Remaining Time First (SRTF / Preemptive SJF)
        public ScheduleResult SimulateSRTF(List<ProcessInfo> processes)
        {
            var gantt = new List<string>();
            var workingSet = processes.Select(p => p with { RemainingTime = p.BurstTime }).ToList();
            int totalProcesses = processes.Count;
            int completed = 0;
            int currentTime = 0;
            var completionTimes = new Dictionary<string, int>();

            while (completed < totalProcesses)
            {
                var available = workingSet.Where(p => p.ArrivalTime <= currentTime && p.RemainingTime > 0)
                                          .OrderBy(p => p.RemainingTime)
                                          .FirstOrDefault();

                if (available != null)
                {
                    gantt.Add($"[Time {currentTime:00}-{currentTime + 1:00}] CPU Runs {available.Id} (Rem: {available.RemainingTime - 1})");
                    int idx = workingSet.FindIndex(p => p.Id == available.Id);
                    workingSet[idx] = available with { RemainingTime = available.RemainingTime - 1 };
                    currentTime++;

                    if (workingSet[idx].RemainingTime == 0)
                    {
                        completed++;
                        completionTimes[available.Id] = currentTime;
                    }
                }
                else
                {
                    gantt.Add($"[Time {currentTime:00}-{currentTime + 1:00}] CPU IDLE");
                    currentTime++;
                }
            }

            double totalWait = 0, totalTurnaround = 0;
            foreach (var p in processes)
            {
                int turnaround = completionTimes[p.Id] - p.ArrivalTime;
                totalTurnaround += turnaround;
                totalWait += (turnaround - p.BurstTime);
            }

            return new ScheduleResult(gantt, totalTurnaround / processes.Count, totalWait / processes.Count);
        }

        // 2. Process Scheduling Simulator (Round Robin)
        public record ProcessInfo(string Id, int ArrivalTime, int BurstTime, int RemainingTime);
        public record ScheduleResult(List<string> GanttChart, double AverageTurnaroundTime, double AverageWaitingTime);

        public ScheduleResult SimulateRoundRobin(List<ProcessInfo> processes, int quantum)
        {
            var queue = new Queue<ProcessInfo>();
            var gantt = new List<string>();
            var completed = new List<ProcessInfo>();
            var workingSet = processes.Select(p => p with { RemainingTime = p.BurstTime }).OrderBy(p => p.ArrivalTime).ToList();
            int currentTime = 0;
            int processIndex = 0;
            var completionTimes = new Dictionary<string, int>();

            while (completed.Count < processes.Count)
            {
                while (processIndex < workingSet.Count && workingSet[processIndex].ArrivalTime <= currentTime)
                {
                    queue.Enqueue(workingSet[processIndex]);
                    processIndex++;
                }

                if (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    int timeSlice = Math.Min(current.RemainingTime, quantum);
                    gantt.Add($"[Time {currentTime:00}-{currentTime + timeSlice:00}] CPU Runs {current.Id}");
                    currentTime += timeSlice;
                    current = current with { RemainingTime = current.RemainingTime - timeSlice };

                    while (processIndex < workingSet.Count && workingSet[processIndex].ArrivalTime <= currentTime)
                    {
                        queue.Enqueue(workingSet[processIndex]);
                        processIndex++;
                    }

                    if (current.RemainingTime > 0)
                        queue.Enqueue(current);
                    else
                    {
                        completed.Add(current);
                        completionTimes[current.Id] = currentTime;
                    }
                }
                else
                {
                    gantt.Add($"[Time {currentTime:00}-{currentTime + 1:00}] CPU IDLE");
                    currentTime++;
                }
            }

            double totalWait = 0, totalTurnaround = 0;
            foreach (var p in processes)
            {
                int turnaround = completionTimes[p.Id] - p.ArrivalTime;
                totalTurnaround += turnaround;
                totalWait += (turnaround - p.BurstTime);
            }

            return new ScheduleResult(gantt, totalTurnaround / processes.Count, totalWait / processes.Count);
        }

        // 3. Deadlock Detection (Banker's Algorithm)
        public record DeadlockCheckResult(bool IsSafe, string Sequence);

        public DeadlockCheckResult CheckSafeState(int[,] max, int[,] allocated, int[] available)
        {
            int numProcesses = max.GetLength(0);
            int numResources = max.GetLength(1);
            int[,] need = new int[numProcesses, numResources];
            for (int i = 0; i < numProcesses; i++)
                for (int j = 0; j < numResources; j++)
                    need[i, j] = max[i, j] - allocated[i, j];

            bool[] finish = new bool[numProcesses];
            int[] work = (int[])available.Clone();
            var sequence = new List<int>();
            int count = 0;

            while (count < numProcesses)
            {
                bool found = false;
                for (int p = 0; p < numProcesses; p++)
                {
                    if (!finish[p])
                    {
                        bool canSatisfy = true;
                        for (int j = 0; j < numResources; j++)
                            if (need[p, j] > work[j]) { canSatisfy = false; break; }

                        if (canSatisfy)
                        {
                            for (int k = 0; k < numResources; k++)
                                work[k] += allocated[p, k];
                            sequence.Add(p);
                            finish[p] = true;
                            found = true;
                            count++;
                        }
                    }
                }
                if (!found) return new DeadlockCheckResult(false, "Unsafe state detected. Potential Deadlock.");
            }
            return new DeadlockCheckResult(true, "Safe Sequence: P" + string.Join(" -> P", sequence));
        }
    }
}
