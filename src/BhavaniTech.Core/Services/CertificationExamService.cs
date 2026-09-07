using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record ExamQuestion(
        int Id,
        string Category,
        string QuestionText,
        List<string> Options,
        int CorrectIndex,
        string Explanation
    );

    public record TechnologyCertificate(
        string CertificateId,
        string StudentName,
        DateTime IssueDate,
        int ScorePercent,
        string GradeHonors,
        string VerificationHash,
        string AsciiCertificateText
    );

    public record ExamGradingResult(
        int TotalQuestions,
        int CorrectCount,
        int ScorePercent,
        bool Passed,
        TechnologyCertificate? Certificate,
        List<string> DetailedReviews
    );

    public record AchievementBadge(
        string Id,
        string Title,
        string Category,
        string IconEmoji,
        string Description,
        bool IsUnlocked,
        DateTime? UnlockedAt
    );

    public static class CertificationExamService
    {
        public static List<AchievementBadge> GetAllBadges()
        {
            return new List<AchievementBadge>
            {
                new("badge_byte", "Byte Master", "Hardware", "⚡", "Mastered binary logic gates and truth tables", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_whitehat", "Whitehat Cyber Specialist", "Cybersecurity", "🛡️", "Captured virtual terminal CTF flags and analyzed binary exploits", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_silicon", "Silicon Architect", "Microarchitecture", "⚙️", "Simulated 5-stage RISC hazard resolution and ALU bypass", false, null),
                new("badge_sql", "SQL Maestro", "Databases", "🗄️", "Executed relational multi-statement DDL/DML in-memory", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_ai", "AI Frontier Pioneer", "Artificial Intelligence", "🧠", "Trained neural decision boundary and vector RAG retrieval", false, null),
                new("badge_hardware", "Hardware Hacker", "Electronics", "🔌", "Simulated breadboard circuits with safe current limits", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_kernel", "Kernel Explorer", "Operating Systems", "🐧", "Explored Unix filesystem and authenticated root access", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_quantum", "Quantum Physicist", "Emerging Tech", "⚛️", "Evaluated Hadamard qubit superposition and quantum measurement", false, null),
                new("badge_cloud", "Cloud Master", "DevOps & Cloud", "☁️", "Scheduled workloads with Kubernetes predicates and scoring", false, null),
                new("badge_polyglot", "Polyglot Coder", "Programming", "💻", "Executed code across C#, Python, SQL, and JavaScript", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_crypto", "Cryptographer", "Security", "🔐", "Derived RSA asymmetric keys and authenticated AES-GCM", true, DateTime.UtcNow.AddDays(-1)),
                new("badge_grandmaster", "Grandmaster of Technology", "Mastery", "🏆", "Scored 85%+ on the Comprehensive Certification Exam", false, null)
            };
        }

        public static List<ExamQuestion> GetCertificationQuestionBank()
        {
            return new List<ExamQuestion>
            {
                new(1, "Operating Systems", "What event occurs when a requested virtual memory page is not present in physical RAM?",
                    new() { "Segmentation Fault", "Page Fault Interrupt", "Cache Miss", "Kernel Panic" }, 1,
                    "A Page Fault interrupt signals the MMU to swap the requested page from secondary disk storage into physical RAM."),

                new(2, "Cryptography", "In RSA public-key encryption, what mathematical problem ensures the secrecy of the private key?",
                    new() { "Discrete Logarithm on Elliptic Curves", "Integer Prime Factorization", "Collision Resistance of SHA-256", "AES S-Box Inversion" }, 1,
                    "RSA relies on the computational hardness of factoring the product n = p * q of two large prime numbers."),

                new(3, "Microarchitecture", "Which stage of the 5-stage RISC pipeline is responsible for computing ALU results or branch target addresses?",
                    new() { "Instruction Fetch (IF)", "Instruction Decode (ID)", "Execute (EX)", "Memory Access (MEM)" }, 2,
                    "The Execute (EX) stage carries out arithmetic calculations and resolves branch conditions in the ALU."),

                new(4, "Cybersecurity", "What defensive measure places randomized canary words before the saved frame pointer to detect stack smashing?",
                    new() { "Address Space Layout Randomization (ASLR)", "Data Execution Prevention (DEP)", "Stack Canary / Guard", "Control Flow Guard" }, 2,
                    "Stack Canaries check whether the buffer bounds were overrun before executing the function return (ret) instruction."),

                new(5, "Networking", "When routing across subnets, which protocol is used by the host to resolve the default gateway's MAC address?",
                    new() { "DNS", "BGP", "ARP", "DHCP" }, 2,
                    "ARP (Address Resolution Protocol) broadcasts on Layer 2 to map the Gateway IP to its physical Ethernet MAC address."),

                new(6, "Electronics", "If a 5V supply is connected to a Red LED with 2.0V forward voltage drop and a 300-ohm resistor, what is the series current?",
                    new() { "50.0 mA", "10.0 mA", "25.0 mA", "6.6 mA" }, 1,
                    "I = (Vs - Vf) / R = (5.0 - 2.0) / 300 = 3.0 / 300 = 0.010 A = 10.0 mA."),

                new(7, "Artificial Intelligence", "What activation function outputs 0 for negative inputs and identity for positive inputs: f(x) = max(0, x)?",
                    new() { "Sigmoid", "Hyperbolic Tangent (Tanh)", "Rectified Linear Unit (ReLU)", "Softmax" }, 2,
                    "ReLU provides fast sparse representation and avoids vanishing gradients for positive inputs."),

                new(8, "Cloud & Kubernetes", "In kube-scheduler, what phase eliminates nodes that fail resource capacity or taint requirements?",
                    new() { "Scoring / Prioritizing", "Filtering / Predicates", "Binding", "Preemption" }, 1,
                    "Filtering evaluates node predicates (CPU, memory, taints) to remove unqualified nodes before scoring remaining candidates.")
            };
        }

        public static ExamGradingResult GradeExam(string studentName, Dictionary<int, int> studentAnswers)
        {
            var bank = GetCertificationQuestionBank();
            int total = bank.Count;
            int correct = 0;
            var reviews = new List<string>();

            foreach (var q in bank)
            {
                bool isCorrect = studentAnswers.TryGetValue(q.Id, out int ans) && ans == q.CorrectIndex;
                if (isCorrect)
                {
                    correct++;
                    reviews.Add($"[PASS] Q{q.Id} ({q.Category}): Correct! {q.Explanation}");
                }
                else
                {
                    string chosen = (studentAnswers.ContainsKey(q.Id) && studentAnswers[q.Id] >= 0 && studentAnswers[q.Id] < q.Options.Count) 
                        ? q.Options[studentAnswers[q.Id]] 
                        : "No Answer";
                    reviews.Add($"[FAIL] Q{q.Id} ({q.Category}): You chose '{chosen}'. Correct: '{q.Options[q.CorrectIndex]}'. Explanation: {q.Explanation}");
                }
            }

            int scorePercent = (int)Math.Round(((double)correct / total) * 100.0);
            bool passed = scorePercent >= 75;

            TechnologyCertificate? cert = null;
            if (passed)
            {
                cert = GenerateCertificate(studentName, scorePercent);
            }

            return new ExamGradingResult(
                TotalQuestions: total,
                CorrectCount: correct,
                ScorePercent: scorePercent,
                Passed: passed,
                Certificate: cert,
                DetailedReviews: reviews
            );
        }

        public static TechnologyCertificate GenerateCertificate(string studentName, int score)
        {
            string cleanName = string.IsNullOrWhiteSpace(studentName) ? "Dharmesh Varia" : studentName.Trim();
            var issueDate = DateTime.UtcNow;
            string certId = "BHAVANI-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

            string honors = score >= 90 ? "High Distinction with Honors" : (score >= 80 ? "Distinction" : "Certified Pass");

            // Cryptographic verification hash
            string rawData = $"{certId}|{cleanName}|{score}|{issueDate:yyyy-MM-dd}|BhavaniAcademySecretKey";
            using var sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            string hashHex = Convert.ToHexString(hashBytes)[..24];

            string asciiCert =
$@"╔════════════════════════════════════════════════════════════════════════════════════╗
║                               BHAVANI TECHNOLOGY ACADEMY                            ║
║                             CERTIFICATE OF TECHNOLOGY MASTERY                      ║
╠════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                    ║
║   This is to certify that:                                                         ║
║                                                                                    ║
║                     ★ ★ ★   {cleanName.ToUpperInvariant()}   ★ ★ ★                 
║                                                                                    ║
║   has successfully passed the Comprehensive Technology Examination with a score of  ║
║                                                                                    ║
║                                   {score}% ({honors.ToUpperInvariant()})           
║                                                                                    ║
║   demonstrating advanced proficiency in:                                           ║
║   • Operating Systems Architecture        • Cybersecurity & Ethical Hacking         ║
║   • Asymmetric & Symmetric Cryptography   • Microarchitecture & RISC Pipelines      ║
║   • Computer Networking & Cloud K8s       • Frontier Artificial Intelligence        ║
║   • Analog/Digital Electronics & IoT      • Relational Databases & Polyglot Coding  ║
║                                                                                    ║
║   Certificate ID:    {certId}                                                      ║
║   Issue Date (UTC):  {issueDate:yyyy-MM-dd HH:mm:ss}                               ║
║   Security Token:    {hashHex}                                                     ║
║                                                                                    ║
║   Verified by: Dharmesh Varia, Founder & Chief Architect                           ║
║   Bhavani Technology Learning Engine (100% Offline Verifiable)                     ║
╚════════════════════════════════════════════════════════════════════════════════════╝";

            return new TechnologyCertificate(
                CertificateId: certId,
                StudentName: cleanName,
                IssueDate: issueDate,
                ScorePercent: score,
                GradeHonors: honors,
                VerificationHash: hashHex,
                AsciiCertificateText: asciiCert
            );
        }
    }
}
