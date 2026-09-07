using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class CcnaCurriculumService
    {
        public record CcnaModule(string Title, string Content);
        public record CcnaExamQuestion(string Question, string[] Options, int CorrectIndex);
        
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
                new CcnaExamQuestion("Which layer of the OSI model does a router primarily operate on?", new[] { "Layer 1 (Physical)", "Layer 2 (Data Link)", "Layer 3 (Network)", "Layer 4 (Transport)" }, 2),
                new CcnaExamQuestion("What is the default subnet mask for a Class C IPv4 address?", new[] { "255.0.0.0", "255.255.0.0", "255.255.255.0", "255.255.255.255" }, 2),
                new CcnaExamQuestion("Which protocol is heavily utilized to prevent Layer 2 switching loops?", new[] { "OSPF", "BGP", "STP", "DHCP" }, 2),
                new CcnaExamQuestion("In a multi-area OSPF deployment, what is the required significance of Area 0?", new[] { "It is the stub area", "It is the transit backbone area", "It is the Not-So-Stubby-Area (NSSA)", "It is used solely for external routes" }, 1),
                new CcnaExamQuestion("When configuring an Extended IPv4 ACL, which elements can be matched?", new[] { "Source IP Only", "MAC Address Only", "Source IP, Dest IP, Protocol, and Port", "VLAN Tag Only" }, 2)
            };
        }

        public record ExamGradeResult(int Score, int TotalQuestions, bool Passed, double Percentage);

        public ExamGradeResult GradeExam(List<int> studentAnswers)
        {
            var questions = GetExamQuestions();
            int score = 0;
            for (int i = 0; i < studentAnswers.Count && i < questions.Count; i++)
            {
                if (studentAnswers[i] == questions[i].CorrectIndex) score++;
            }
            
            double percentage = ((double)score / questions.Count) * 100;
            return new ExamGradeResult(score, questions.Count, percentage >= 80.0, percentage);
        }
    }
}
