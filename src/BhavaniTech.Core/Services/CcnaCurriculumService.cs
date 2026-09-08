using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class CcnaCurriculumService
    {
        public record CcnaModule(string Title, string Content);
        public record CcnaExamQuestion(string Question, string[] Options, int CorrectIndex, string Explanation = "");
        
        public List<CcnaModule> GetTutorials()
        {
            return new List<CcnaModule>
            {
                new CcnaModule("1. OSI & TCP/IP Models", "The OSI model has 7 layers: Physical, Data Link, Network, Transport, Session, Presentation, Application. TCP/IP condenses this into 4 layers. The Network layer (Layer 3) handles logical IP addressing and routing. The Data Link layer (Layer 2) handles physical MAC addressing and switching."),
                new CcnaModule("2. IPv4 Subnetting & VLSM", "Subnetting divides a larger network into smaller, manageable subnets to reduce broadcast domains. VLSM (Variable Length Subnet Masking) allows different subnet masks within the same network class. The formula for usable hosts is: (2^h) - 2, where h is the number of host bits."),
                new CcnaModule("3. Switching & VLANs", "Switches operate at Layer 2. A VLAN (Virtual LAN) logically segments a physical switch into multiple isolated broadcast domains, improving security and performance. IEEE 802.1Q is the trunking protocol used to carry traffic for multiple VLANs across a single link."),
                new CcnaModule("4. Spanning Tree Protocol (STP)", "STP (IEEE 802.1D) prevents Layer 2 broadcast storms in networks with redundant links. It elects a Root Bridge based on the lowest Bridge Priority (or lowest MAC address in a tie), and blocks redundant ports to create a strictly loop-free topology."),
                new CcnaModule("5. Routing Protocols (OSPF)", "OSPF (Open Shortest Path First) is a Link-State routing protocol operating at Layer 3. It uses the Dijkstra SPF algorithm to calculate the shortest path to destinations based on bandwidth cost. It organizes networks hierarchically into Areas, with Area 0 designated as the strict backbone."),
                new CcnaModule("6. Network Security & ACLs", "Access Control Lists (ACLs) filter traffic as it passes through a router. Standard ACLs (1-99) filter based exclusively on the source IP address. Extended ACLs (100-199) are granular, filtering on source/destination IP, protocol (TCP/UDP), and exact port numbers. There is an 'Implicit Deny Any' at the end of every ACL.")
            };
        }

        public List<CcnaExamQuestion> GetExamQuestions()
        {
            return new List<CcnaExamQuestion>
            {
                new CcnaExamQuestion("Which layer of the OSI model does a router primarily operate on?", new[] { "Layer 1 (Physical)", "Layer 2 (Data Link)", "Layer 3 (Network)", "Layer 4 (Transport)" }, 2, "Routers inspect Layer 3 IP headers to make path forwarding decisions."),
                new CcnaExamQuestion("What is the default subnet mask for a Class C IPv4 address?", new[] { "255.0.0.0", "255.255.0.0", "255.255.255.0", "255.255.255.255" }, 2, "Class C addresses have a default 24-bit network prefix (255.255.255.0 or /24)."),
                new CcnaExamQuestion("Which protocol is heavily utilized to prevent Layer 2 switching loops?", new[] { "OSPF", "BGP", "STP", "DHCP" }, 2, "Spanning Tree Protocol (STP - IEEE 802.1D) blocks redundant switch links to prevent broadcast storms."),
                new CcnaExamQuestion("In a multi-area OSPF deployment, what is the required significance of Area 0?", new[] { "It is the stub area", "It is the transit backbone area", "It is the Not-So-Stubby-Area (NSSA)", "It is used solely for external routes" }, 1, "Area 0 (backbone) is required as the central hub through which all non-backbone areas exchange inter-area routing information."),
                new CcnaExamQuestion("When configuring an Extended IPv4 ACL, which elements can be matched?", new[] { "Source IP Only", "MAC Address Only", "Source IP, Dest IP, Protocol, and Port", "VLAN Tag Only" }, 2, "Extended ACLs filter on source IP, destination IP, protocol (TCP, UDP, ICMP), and port numbers."),
                new CcnaExamQuestion("How many usable host IP addresses are provided by a /27 subnet mask?", new[] { "14", "30", "62", "126" }, 1, "A /27 mask leaves 5 host bits (32 - 27 = 5). Usable hosts = 2^5 - 2 = 30."),
                new CcnaExamQuestion("What is the inverse wildcard mask corresponding to a subnet mask of 255.255.255.240 (/28)?", new[] { "0.0.0.15", "0.0.0.31", "0.0.0.7", "0.0.0.255" }, 0, "Wildcard = 255.255.255.255 - 255.255.255.240 = 0.0.0.15."),
                new CcnaExamQuestion("Which TCP port is used by SSH for secure remote shell management?", new[] { "21", "22", "23", "25" }, 1, "SSH operates on TCP port 22, replacing unencrypted Telnet (TCP 23)."),
                new CcnaExamQuestion("What is the primary protocol used to dynamically assign IP addresses to network hosts?", new[] { "DNS", "SNMP", "DHCP", "NTP" }, 2, "Dynamic Host Configuration Protocol (DHCP) automatically assigns IP addresses, subnet masks, and default gateways."),
                new CcnaExamQuestion("In Cisco IOS, what is the default administrative distance (AD) of OSPF?", new[] { "90", "110", "115", "120" }, 1, "OSPF has an administrative distance of 110 (EIGRP is 90, IS-IS is 115, RIP is 120)."),
                new CcnaExamQuestion("Which type of IPv6 address starts with the prefix fe80::/10?", new[] { "Global Unicast", "Unique Local", "Link-Local", "Multicast" }, 2, "fe80::/10 identifies IPv6 Link-Local addresses used strictly within a single local link segment."),
                new CcnaExamQuestion("What is the MAC address format length used in IEEE 802.3 Ethernet?", new[] { "32 bits (4 bytes)", "48 bits (6 bytes)", "64 bits (8 bytes)", "128 bits (16 bytes)" }, 1, "MAC addresses are 48 bits (6 bytes) long, represented in hexadecimal."),
                new CcnaExamQuestion("Which state does an OSPF adjacency reach when link-state databases are fully synchronized?", new[] { "2-Way", "ExStart", "Loading", "Full" }, 3, "The 'Full' state indicates that both OSPF neighbors have fully synchronized Link-State Databases (LSDB)."),
                new CcnaExamQuestion("What does the transport layer protocol TCP use to guarantee reliable data delivery?", new[] { "Best-effort UDP datagrams", "Three-way handshake, Sequence numbers, and ACKs", "CSMA/CD collision detection", "ARP broadcasting" }, 1, "TCP establishes connection via SYN-SYN/ACK-ACK handshake and uses sequence numbers and ACKs for retransmission."),
                new CcnaExamQuestion("Which command in Cisco IOS switches is used to configure an interface as an 802.1Q trunk port?", new[] { "switchport mode access", "switchport mode trunk", "spanning-tree portfast", "ip routing" }, 1, "'switchport mode trunk' sets the interface to multiplex multiple VLANs tagged with 802.1Q headers."),
                new CcnaExamQuestion("What is the primary function of the Address Resolution Protocol (ARP)?", new[] { "Resolve hostnames to IP addresses", "Map known IP addresses to local MAC addresses", "Filter unauthorized switch packets", "Route packets between autonomous systems" }, 1, "ARP broadcasts at Layer 2 to discover the MAC address associated with a given Layer 3 IPv4 address."),
                new CcnaExamQuestion("What address is the IPv4 loopback address reserved for local system testing?", new[] { "0.0.0.0", "127.0.0.1", "192.168.1.1", "255.255.255.255" }, 1, "127.0.0.1 (or 127.0.0.0/8) is reserved as the local host loopback interface."),
                new CcnaExamQuestion("In Cisco Spanning Tree Protocol (STP), what is the default bridge priority value?", new[] { "0", "4096", "32768", "65535" }, 2, "The default STP bridge priority is 32768 (plus the sys-id-ext VLAN number)."),
                new CcnaExamQuestion("Which NAT technique allows multiple private internal IP addresses to share a single public IP address using unique port numbers?", new[] { "Static NAT", "Dynamic NAT Pool", "PAT (Port Address Translation / NAT Overload)", "Carrier Grade BGP NAT" }, 2, "PAT (NAT Overload) multiplexes private hosts onto a single public IPv4 address using source TCP/UDP ports."),
                new CcnaExamQuestion("What happens to an inbound packet if it does not match any permit statements in an IPv4 ACL?", new[] { "It is forwarded to the default gateway", "It is dropped due to the implicit deny any rule at the end", "It is logged and forwarded without filtering", "It is bounced back to the sender via ICMP Echo Request" }, 1, "All Cisco ACLs end with an invisible implicit 'deny any' statement that drops unmatched packets.")
            };
        }

        public record ExamGradeResult(int Score, int TotalQuestions, bool Passed, double Percentage, List<string> Explanations);

        public ExamGradeResult GradeExam(List<int> studentAnswers)
        {
            var questions = GetExamQuestions();
            int score = 0;
            var explanations = new List<string>();

            for (int i = 0; i < studentAnswers.Count && i < questions.Count; i++)
            {
                var q = questions[i];
                if (studentAnswers[i] == q.CorrectIndex)
                {
                    score++;
                    explanations.Add($"Q{i + 1}: Correct! {q.Explanation}");
                }
                else
                {
                    explanations.Add($"Q{i + 1}: Incorrect. You selected '{q.Options[studentAnswers[i]]}'. Correct answer: '{q.Options[q.CorrectIndex]}'. {q.Explanation}");
                }
            }
            
            double percentage = ((double)score / questions.Count) * 100;
            return new ExamGradeResult(score, questions.Count, percentage >= 80.0, percentage, explanations);
        }
    }
}
