using System.Collections.Generic;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Services
{
    public static class NetworkSimulationService
    {
        public static List<NetworkPacketStep> SimulatePing(string sourceIP, string destIP)
        {
            var steps = new List<NetworkPacketStep>
            {
                new NetworkPacketStep("1. Host ARP Query", "Student PC (192.168.1.10)", "Switch-01", "Layer 2 Data Link", "Broadcast ARP Request: Who has 192.168.1.1 gateway?", true),
                new NetworkPacketStep("2. Switch Frame Forwarding", "Switch-01", "Gateway Router (192.168.1.1)", "Layer 2 MAC Table", "Forwarding frame to MAC 00:1A:2B:3C:4D:5E", true),
                new NetworkPacketStep("3. Router IP Packet Routing", "Gateway Router", "Server (8.8.8.8)", "Layer 3 IP Routing", "Route lookup: Next Hop via WAN interface 203.0.113.1", true),
                new NetworkPacketStep("4. ICMP Echo Request", "Router WAN", "DNS Server (8.8.8.8)", "Layer 4 ICMP Protocol", "ICMP Type 8 (Echo Request), TTL=64, Payload=32 bytes", true),
                new NetworkPacketStep("5. ICMP Echo Reply", "DNS Server (8.8.8.8)", "Student PC (192.168.1.10)", "Layer 4 ICMP Protocol", "ICMP Type 0 (Echo Reply), RTT = 4 ms. Success!", true)
            };

            return steps;
        }

        public static List<NetworkPacketStep> SimulateDhcpDns(string domainName)
        {
            return new List<NetworkPacketStep>
            {
                new NetworkPacketStep("1. DHCP Discover", "Client PC", "Broadcast (255.255.255.255)", "UDP Port 67/68", "Client requesting IP assignment", true),
                new NetworkPacketStep("2. DHCP Offer", "DHCP Server", "Client PC", "UDP Port 68", "Offered IP: 192.168.1.105, Subnet: /24, DNS: 8.8.8.8", true),
                new NetworkPacketStep("3. DNS Query", "Client PC", "DNS Server (8.8.8.8)", "UDP Port 53", $"Standard Query A {domainName}", true),
                new NetworkPacketStep("4. DNS Response", "DNS Server", "Client PC", "UDP Port 53", $"{domainName} resolved to IP 104.21.48.12", true)
            };
        }
    }
}
