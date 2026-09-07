using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class DistributedSystemsService
    {
        // 1. CAP Theorem Simulator
        public record CapEvaluation(string Strategy, bool Consistency, bool Availability, bool PartitionTolerance, string ResultMessage);

        public CapEvaluation SimulateNetworkPartition(string databaseType)
        {
            return databaseType.ToUpperInvariant() switch
            {
                "CP" => new CapEvaluation("CP", true, false, true, "Network Partition! System guarantees Consistency. Nodes reject reads/writes to prevent stale data, losing Availability."),
                "AP" => new CapEvaluation("AP", false, true, true, "Network Partition! System guarantees Availability. Nodes serve reads/writes, but data may be stale or conflicting (Eventual Consistency)."),
                "CA" => new CapEvaluation("CA", true, true, false, "Theoretical only. Cannot survive a Network Partition without compromising C or A. Not a true distributed system choice."),
                _ => new CapEvaluation("Unknown", false, false, false, "Invalid strategy.")
            };
        }

        // 2. Raft Consensus Simulator
        public enum RaftState { Follower, Candidate, Leader }
        public class RaftNode
        {
            public required string Id { get; set; }
            public RaftState State { get; set; } = RaftState.Follower;
            public int CurrentTerm { get; set; } = 0;
            public string? VotedFor { get; set; } = null;
        }

        public record RaftElectionResult(string? NewLeader, int Term, List<string> Events);

        public RaftElectionResult SimulateLeaderElection(List<RaftNode> nodes, string candidateId)
        {
            var events = new List<string>();
            var candidate = nodes.FirstOrDefault(n => n.Id == candidateId);
            if (candidate == null) return new RaftElectionResult(null, 0, new List<string> { "Candidate not found." });

            candidate.State = RaftState.Candidate;
            candidate.CurrentTerm++;
            candidate.VotedFor = candidate.Id;
            events.Add($"Node {candidate.Id} times out, becomes Candidate for Term {candidate.CurrentTerm}.");
            events.Add($"Node {candidate.Id} votes for itself.");

            int votes = 1;
            foreach (var node in nodes.Where(n => n.Id != candidate.Id))
            {
                if (node.CurrentTerm < candidate.CurrentTerm)
                {
                    node.CurrentTerm = candidate.CurrentTerm;
                    node.VotedFor = candidate.Id;
                    node.State = RaftState.Follower;
                    votes++;
                    events.Add($"Node {node.Id} grants vote to {candidate.Id}.");
                }
                else
                {
                    events.Add($"Node {node.Id} rejects vote for {candidate.Id} (Term >= Candidate Term).");
                }
            }

            int majority = (nodes.Count / 2) + 1;
            if (votes >= majority)
            {
                candidate.State = RaftState.Leader;
                events.Add($"Node {candidate.Id} receives {votes}/{nodes.Count} votes and becomes LEADER.");
                return new RaftElectionResult(candidate.Id, candidate.CurrentTerm, events);
            }
            else
            {
                candidate.State = RaftState.Follower;
                events.Add($"Node {candidate.Id} failed to get majority. Reverting to Follower.");
                return new RaftElectionResult(null, candidate.CurrentTerm, events);
            }
        }

        // 3. Load Balancer Simulator
        public record LbRoute(string RequestId, string ServerIp, string Algorithm);

        public List<LbRoute> SimulateLoadBalancer(List<string> serverIps, List<string> requests, string algorithm)
        {
            var routes = new List<LbRoute>();
            if (algorithm == "RoundRobin")
            {
                for (int i = 0; i < requests.Count; i++)
                    routes.Add(new LbRoute(requests[i], serverIps[i % serverIps.Count], "RoundRobin"));
            }
            else if (algorithm == "IPHash")
            {
                foreach (var req in requests)
                {
                    int hash = Math.Abs(req.GetHashCode());
                    routes.Add(new LbRoute(req, serverIps[hash % serverIps.Count], "IPHash"));
                }
            }
            return routes;
        }
    }
}
