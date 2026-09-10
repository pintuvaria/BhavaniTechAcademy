using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public enum RaftNodeRole
    {
        Follower,
        Candidate,
        Leader
    }

    public record RaftLogEntry(int Term, int Index, string Command);

    public class RaftNode
    {
        public int Id { get; }
        public RaftNodeRole Role { get; set; } = RaftNodeRole.Follower;
        public int CurrentTerm { get; set; } = 1;
        public int? VotedFor { get; set; } = null;
        public List<RaftLogEntry> Log { get; } = new();
        public int CommitIndex { get; set; } = 0;
        public int LastApplied { get; set; } = 0;
        public int PartitionGroup { get; set; } = 1; // Used to simulate network partitions
        public bool IsAlive { get; set; } = true;
        public int VotesReceived { get; set; } = 0;

        public RaftNode(int id)
        {
            Id = id;
        }

        public int LastLogIndex => Log.Count;
        public int LastLogTerm => Log.Count > 0 ? Log[^1].Term : 0;
    }

    public class RaftClusterSimulation
    {
        public List<RaftNode> Nodes { get; } = new();
        public List<string> EventLog { get; } = new();
        private readonly Random _rand = new(42);

        public RaftClusterSimulation(int nodeCount = 5)
        {
            for (int i = 1; i <= nodeCount; i++)
            {
                Nodes.Add(new RaftNode(i));
            }
            Nodes[0].Role = RaftNodeRole.Leader;
            Nodes[0].CurrentTerm = 1;
            Nodes[0].Log.Add(new RaftLogEntry(1, 1, "SET cluster_init=true"));
            Nodes[0].CommitIndex = 1;

            // Replicate initial log to all followers
            for (int i = 1; i < Nodes.Count; i++)
            {
                Nodes[i].Log.Add(new RaftLogEntry(1, 1, "SET cluster_init=true"));
                Nodes[i].CommitIndex = 1;
            }

            EventLog.Add($"[INIT] Raft cluster initialized with {nodeCount} nodes. Node 1 is initial Leader (Term 1).");
        }

        public RaftNode? CurrentLeader => Nodes.FirstOrDefault(n => n.IsAlive && n.Role == RaftNodeRole.Leader);

        /// <summary>
        /// Proposes a new key-value state mutation to the cluster leader.
        /// </summary>
        public bool ProposeCommand(string command, out string statusMessage)
        {
            var leader = CurrentLeader;
            if (leader == null)
            {
                statusMessage = "Write rejected: No active Leader currently elected in cluster.";
                EventLog.Add($"[CLIENT ERROR] {statusMessage}");
                return false;
            }

            int newIndex = leader.Log.Count + 1;
            var entry = new RaftLogEntry(leader.CurrentTerm, newIndex, command);
            leader.Log.Add(entry);
            EventLog.Add($"[LEADER #{leader.Id}] Appended Log #{newIndex} (Term {leader.CurrentTerm}): '{command}'. Replicating to followers...");

            // Check quorum replication among reachable peers in the same partition
            var reachablePeers = Nodes.Where(n => n.IsAlive && n.PartitionGroup == leader.PartitionGroup).ToList();
            int acks = 1; // Leader itself

            foreach (var peer in reachablePeers)
            {
                if (peer.Id == leader.Id) continue;
                peer.Log.Add(entry);
                acks++;
            }

            int majority = (Nodes.Count / 2) + 1;
            if (acks >= majority)
            {
                leader.CommitIndex = newIndex;
                foreach (var peer in reachablePeers)
                {
                    peer.CommitIndex = newIndex;
                }
                statusMessage = $"Success! Quorum reached ({acks}/{Nodes.Count} nodes). Log #{newIndex} committed to state machine.";
                EventLog.Add($"[QUORUM COMMIT] {statusMessage}");
                return true;
            }
            else
            {
                statusMessage = $"Split-brain / Minority write! Only {acks}/{Nodes.Count} acks (Need {majority} for Quorum). Entry uncommitted!";
                EventLog.Add($"[UNCOMMITTED] {statusMessage}");
                return false;
            }
        }

        /// <summary>
        /// Simulates a network partition isolating minority nodes from the majority.
        /// </summary>
        public void PartitionCluster(List<int> minorityNodeIds)
        {
            foreach (var node in Nodes)
            {
                if (minorityNodeIds.Contains(node.Id))
                {
                    node.PartitionGroup = 2; // Isolated partition
                }
                else
                {
                    node.PartitionGroup = 1; // Majority partition
                }
            }

            EventLog.Add($"[NETWORK PARTITION] Cluster split into Partition 1: [{string.Join(", ", Nodes.Where(n => n.PartitionGroup == 1).Select(n => $"Node {n.Id}"))}] and Partition 2: [{string.Join(", ", Nodes.Where(n => n.PartitionGroup == 2).Select(n => $"Node {n.Id}"))}].");
        }

        /// <summary>
        /// Triggers an election cycle in a given partition group if leader heartbeat is missing.
        /// </summary>
        public void TriggerElection(int partitionGroup)
        {
            var partitionNodes = Nodes.Where(n => n.IsAlive && n.PartitionGroup == partitionGroup).ToList();
            if (partitionNodes.Count == 0) return;

            // Pick first node as candidate
            var candidate = partitionNodes.FirstOrDefault(n => n.Role != RaftNodeRole.Leader) ?? partitionNodes[0];
            candidate.Role = RaftNodeRole.Candidate;
            candidate.CurrentTerm++;
            candidate.VotedFor = candidate.Id;
            candidate.VotesReceived = 1;

            EventLog.Add($"[ELECTION] Node {candidate.Id} timeout expired. Transitioned to Candidate (Term {candidate.CurrentTerm}). Requesting votes...");

            int totalNodes = Nodes.Count;
            int quorumNeeded = (totalNodes / 2) + 1;

            foreach (var peer in partitionNodes)
            {
                if (peer.Id == candidate.Id) continue;
                // Grant vote if peer term is <= candidate term
                if (peer.CurrentTerm < candidate.CurrentTerm || peer.VotedFor == null || peer.VotedFor == candidate.Id)
                {
                    peer.CurrentTerm = candidate.CurrentTerm;
                    peer.VotedFor = candidate.Id;
                    peer.Role = RaftNodeRole.Follower;
                    candidate.VotesReceived++;
                }
            }

            if (candidate.VotesReceived >= quorumNeeded)
            {
                candidate.Role = RaftNodeRole.Leader;
                EventLog.Add($"[LEADER ELECTED] Node {candidate.Id} achieved quorum ({candidate.VotesReceived}/{totalNodes} votes). Now Leader for Term {candidate.CurrentTerm}!");
            }
            else
            {
                EventLog.Add($"[ELECTION STALEMATE] Node {candidate.Id} received {candidate.VotesReceived} votes (Needed {quorumNeeded}). Quorum failed in minority partition.");
            }
        }

        /// <summary>
        /// Heals all network partitions, synchronizing cluster back to single partition.
        /// Higher-term leaders take precedence; minority stale leaders abdicate.
        /// </summary>
        public void HealPartition()
        {
            foreach (var node in Nodes)
            {
                node.PartitionGroup = 1;
            }

            EventLog.Add("[NETWORK HEALED] All partition boundaries removed. Nodes can communicate freely.");

            // Find highest term among all leaders
            var leaders = Nodes.Where(n => n.Role == RaftNodeRole.Leader).OrderByDescending(n => n.CurrentTerm).ToList();
            if (leaders.Count > 1)
            {
                var trueLeader = leaders[0];
                for (int i = 1; i < leaders.Count; i++)
                {
                    var staleLeader = leaders[i];
                    staleLeader.Role = RaftNodeRole.Follower;
                    staleLeader.CurrentTerm = trueLeader.CurrentTerm;
                    staleLeader.VotedFor = trueLeader.Id;
                    EventLog.Add($"[ABDICATION] Stale Leader Node {staleLeader.Id} (Term {staleLeader.CurrentTerm}) discovered higher Term {trueLeader.CurrentTerm} Leader Node {trueLeader.Id}. Stepping down to Follower.");
                }

                // Log reconciliation: overwrite uncommitted entries from stale leader
                foreach (var node in Nodes)
                {
                    if (node.Id != trueLeader.Id)
                    {
                        node.Log.Clear();
                        node.Log.AddRange(trueLeader.Log);
                        node.CommitIndex = trueLeader.CommitIndex;
                        node.CurrentTerm = trueLeader.CurrentTerm;
                    }
                }
                EventLog.Add($"[LOG RECONCILIATION] All follower logs replicated & reconciled with Leader Node {trueLeader.Id} (Total entries: {trueLeader.Log.Count}).");
            }
        }

        /// <summary>
        /// Generates an ASCII topology diagram of the Raft cluster state.
        /// </summary>
        public string RenderClusterTopology()
        {
            var sb = new StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                     RAFT DISTRIBUTED CONSENSUS CLUSTER TOPOLOGY                     ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════════════════════════════════╝");

            var groups = Nodes.GroupBy(n => n.PartitionGroup).OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                sb.AppendLine($"\n  ─── PARTITION {group.Key} (Reachable Domain) ───────────────────────────────────────");
                foreach (var node in group)
                {
                    string statusIcon = !node.IsAlive ? "[OFFLINE]" : node.Role switch
                    {
                        RaftNodeRole.Leader => "[★ LEADER ]",
                        RaftNodeRole.Candidate => "[? CANDID ]",
                        _ => "[● FOLLOWER]"
                    };

                    string logSummary = $"Logs: {node.Log.Count} | Committed: {node.CommitIndex} | Term: {node.CurrentTerm}";
                    sb.AppendLine($"    Node #{node.Id} {statusIcon} | {logSummary}");
                }
            }

            sb.AppendLine("\n  ─── RECENT RAFT EVENTS ──────────────────────────────────────────────────────────");
            int startIdx = Math.Max(0, EventLog.Count - 6);
            for (int i = startIdx; i < EventLog.Count; i++)
            {
                sb.AppendLine($"    {EventLog[i]}");
            }

            return sb.ToString();
        }
    }
}
