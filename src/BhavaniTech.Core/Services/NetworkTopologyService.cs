using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record NetworkDevice(
        string Id,
        string Name,
        string DeviceType, // PC, Switch, Router, Firewall, Server
        string IpAddress,
        string MacAddress,
        string SubnetMask,
        string DefaultGateway
    );

    public record PacketHop(
        int StepNumber,
        string FromDevice,
        string ToDevice,
        string Layer, // Layer 2 (Data Link), Layer 3 (Network), Layer 4/7
        string ActionDescription,
        string Protocol,
        bool IsDropped,
        string DropReason
    );

    public record PacketTraceResult(
        string SourceIp,
        string DestinationIp,
        string Protocol,
        bool IsSuccessful,
        int RoundTripTimeMs,
        List<PacketHop> Hops,
        string Summary
    );

    public static class NetworkTopologyService
    {
        public static List<NetworkDevice> GetPresetTopology()
        {
            return new List<NetworkDevice>
            {
                new NetworkDevice("pc1", "Student-PC", "PC", "192.168.1.10", "AA:BB:CC:11:22:33", "255.255.255.0", "192.168.1.1"),
                new NetworkDevice("sw1", "LAN-Switch-1", "Switch", "N/A", "AA:BB:CC:00:00:01", "N/A", "N/A"),
                new NetworkDevice("rt1", "Gateway-Router", "Router", "192.168.1.1", "AA:BB:CC:DD:EE:01", "255.255.255.0", "10.0.0.1"),
                new NetworkDevice("fw1", "Corporate-Firewall", "Firewall", "10.0.0.1", "AA:BB:CC:DD:EE:02", "255.255.255.0", "N/A"),
                new NetworkDevice("srv1", "Academy-Cloud-Server", "Server", "10.0.0.50", "AA:BB:CC:99:88:77", "255.255.255.0", "10.0.0.1")
            };
        }

        public static PacketTraceResult TracePacket(string sourceIp, string destinationIp, string protocol = "HTTP", bool firewallBlock = false)
        {
            var hops = new List<PacketHop>();
            int step = 1;

            // Step 1: PC ARP & Routing Decision
            hops.Add(new PacketHop(
                StepNumber: step++,
                FromDevice: "Student-PC (192.168.1.10)",
                ToDevice: "LAN-Switch-1",
                Layer: "L3/L2 Decision",
                ActionDescription: $"Destination {destinationIp} is on an external subnet. Consulting route table: Next hop is Default Gateway 192.168.1.1. Resolving MAC via ARP.",
                Protocol: "ARP / Ethernet II",
                IsDropped: false,
                DropReason: ""
            ));

            // Step 2: Switch CAM Table Lookup
            hops.Add(new PacketHop(
                StepNumber: step++,
                FromDevice: "LAN-Switch-1",
                ToDevice: "Gateway-Router (192.168.1.1)",
                Layer: "Layer 2 (Data Link)",
                ActionDescription: "Switch examines Ethernet frame header. Dest MAC matches Router interface Port 1. Frame forwarded.",
                Protocol: "Ethernet II",
                IsDropped: false,
                DropReason: ""
            ));

            // Step 3: Router Longest Prefix Match
            hops.Add(new PacketHop(
                StepNumber: step++,
                FromDevice: "Gateway-Router (192.168.1.1)",
                ToDevice: "Corporate-Firewall (10.0.0.1)",
                Layer: "Layer 3 (Network)",
                ActionDescription: $"Router strips L2 header, decrements IPv4 TTL=64->63. Re-computes checksum. Forwards across WAN/VLAN interface to Firewall.",
                Protocol: "IPv4",
                IsDropped: false,
                DropReason: ""
            ));

            // Step 4: Firewall Stateful Inspection
            if (firewallBlock)
            {
                hops.Add(new PacketHop(
                    StepNumber: step++,
                    FromDevice: "Corporate-Firewall",
                    ToDevice: "DROPPED",
                    Layer: "Layer 4/7 (Security)",
                    ActionDescription: $"Firewall Rule #4: REJECT {protocol} from 192.168.1.0/24 to {destinationIp}. Packet dropped and logged.",
                    Protocol: protocol,
                    IsDropped: true,
                    DropReason: "BLOCKED by Firewall Rule #4 (Inbound Security Policy)"
                ));

                return new PacketTraceResult(
                    SourceIp: sourceIp,
                    DestinationIp: destinationIp,
                    Protocol: protocol,
                    IsSuccessful: false,
                    RoundTripTimeMs: 0,
                    Hops: hops,
                    Summary: $"Packet transmission failed: Corporate Firewall dropped the {protocol} packet due to security filtering."
                );
            }

            hops.Add(new PacketHop(
                StepNumber: step++,
                FromDevice: "Corporate-Firewall",
                ToDevice: "Academy-Cloud-Server (10.0.0.50)",
                Layer: "Layer 4/7 (Security)",
                ActionDescription: $"Stateful Inspection PASS: Inbound {protocol} allowed on target port. Frame delivered to target server NIC.",
                Protocol: protocol,
                IsDropped: false,
                DropReason: ""
            ));

            // Step 5: Server ACK / Response
            hops.Add(new PacketHop(
                StepNumber: step++,
                FromDevice: "Academy-Cloud-Server (10.0.0.50)",
                ToDevice: "Student-PC (192.168.1.10)",
                Layer: "Layer 7 (Application)",
                ActionDescription: $"Server processes request. Generates HTTP/2 200 OK Response (Payload: 1420 bytes). Transmission complete.",
                Protocol: $"{protocol} Response",
                IsDropped: false,
                DropReason: ""
            ));

            return new PacketTraceResult(
                SourceIp: sourceIp,
                DestinationIp: destinationIp,
                Protocol: protocol,
                IsSuccessful: true,
                RoundTripTimeMs: 14,
                Hops: hops,
                Summary: $"End-to-End Success: 5 hops traversed in 14ms RTT with 0% packet loss."
            );
        }
    }
}
