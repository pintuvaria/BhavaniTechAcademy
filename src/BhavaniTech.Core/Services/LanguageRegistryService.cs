using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public record LanguageSpec(
        string Id,
        string DisplayName,
        string LogoEmoji,
        string BrandHexColor,
        string Version,
        string Paradigm,
        string FileExtension,
        string Description,
        string KeyFeatures,
        string StarterTemplateCode
    );

    public static class LanguageRegistryService
    {
        private static readonly Dictionary<string, LanguageSpec> Languages = new(StringComparer.OrdinalIgnoreCase)
        {
            ["python"] = new LanguageSpec(
                Id: "python",
                DisplayName: "Python",
                LogoEmoji: "🐍",
                BrandHexColor: "#3776AB",
                Version: "Python 3.12",
                Paradigm: "Interpreted • Multi-Paradigm • Dynamic",
                FileExtension: ".py",
                Description: "High-level, readable language widely used in AI, Data Science, Web Development, and Automation.",
                KeyFeatures: "Dynamic typing, extensive standard library, list comprehensions, NumPy/PyTorch ecosystems.",
                StarterTemplateCode: 
                    "# ========================================================\n" +
                    "# Python 3.12 — Algorithms & Data Science Starter\n" +
                    "# Bhavani Technology Academy\n" +
                    "# ========================================================\n\n" +
                    "def fibonacci_sequence(n: int) -> list[int]:\n" +
                    "    \"\"\"Generate the first n numbers of the Fibonacci series.\"\"\"\n" +
                    "    if n <= 0: return []\n" +
                    "    if n == 1: return [0]\n" +
                    "    seq = [0, 1]\n" +
                    "    while len(seq) < n:\n" +
                    "        seq.append(seq[-1] + seq[-2])\n" +
                    "    return seq\n\n" +
                    "# Test Fibonacci\n" +
                    "numbers = fibonacci_sequence(10)\n" +
                    "print(f\"Fibonacci (First 10): {numbers}\")\n\n" +
                    "# List Comprehension & Aggregations\n" +
                    "squares = [x**2 for x in range(1, 8)]\n" +
                    "print(f\"Squares: {squares}\")\n" +
                    "print(f\"Sum of squares: {sum(squares)}\")\n"
            ),

            ["csharp"] = new LanguageSpec(
                Id: "csharp",
                DisplayName: "C#",
                LogoEmoji: "🔷",
                BrandHexColor: "#512BD4",
                Version: "C# 13 / .NET 9",
                Paradigm: "Compiled • Strongly Typed • Object-Oriented",
                FileExtension: ".cs",
                Description: "Modern, type-safe, object-oriented language engineered for cloud services, desktop apps, and game development.",
                KeyFeatures: "Pattern matching, records, LINQ queries, async/await, memory spans, cross-platform runtime.",
                StarterTemplateCode:
                    "// ========================================================\n" +
                    "// C# 13 / .NET 9 — Modern High-Performance OOP\n" +
                    "// Bhavani Technology Academy\n" +
                    "// ========================================================\n" +
                    "using System;\n" +
                    "using System.Linq;\n" +
                    "using System.Collections.Generic;\n\n" +
                    "public record Student(string Name, int Grade, double Score);\n\n" +
                    "public class Program\n" +
                    "{\n" +
                    "    public static void Main()\n" +
                    "    {\n" +
                    "        Console.WriteLine(\"Welcome to C# 13 in Bhavani Technology Academy!\");\n\n" +
                    "        var students = new List<Student>\n" +
                    "        {\n" +
                    "            new(\"Alice Sharma\", 10, 95.5),\n" +
                    "            new(\"Dharmesh Varia\", 12, 98.8),\n" +
                    "            new(\"Rohan Patel\", 11, 89.2)\n" +
                    "        };\n\n" +
                    "        var honors = students.Where(s => s.Score >= 90.0).OrderByDescending(s => s.Score);\n" +
                    "        Console.WriteLine(\"--- HONOR ROLL STUDENTS ---\");\n" +
                    "        foreach (var s in honors)\n" +
                    "        {\n" +
                    "            Console.WriteLine($\"⭐ {s.Name} | Grade: {s.Grade} | Score: {s.Score}%\");\n" +
                    "        }\n" +
                    "    }\n" +
                    "}\n"
            ),

            ["javascript"] = new LanguageSpec(
                Id: "javascript",
                DisplayName: "JavaScript",
                LogoEmoji: "⚡",
                BrandHexColor: "#F7DF1E",
                Version: "ECMAScript 2024",
                Paradigm: "Interpreted • Event-Driven • Prototype-Based",
                FileExtension: ".js",
                Description: "The universal programming language of the web, powering frontend UI and backend Node.js runtimes.",
                KeyFeatures: "First-class functions, closures, dynamic objects, Promises, async/await, JSON native support.",
                StarterTemplateCode:
                    "// ========================================================\n" +
                    "// JavaScript ES2024 — Modern Web & Server Scripting\n" +
                    "// Bhavani Technology Academy\n" +
                    "// ========================================================\n\n" +
                    "function calculateCart(items) {\n" +
                    "    const taxRate = 0.08;\n" +
                    "    const subtotal = items.reduce((sum, item) => sum + (item.price * item.qty), 0);\n" +
                    "    const tax = subtotal * taxRate;\n" +
                    "    const total = subtotal + tax;\n" +
                    "    return { subtotal, tax, total };\n" +
                    "}\n\n" +
                    "const cart = [\n" +
                    "    { title: 'Arduino Uno R4', price: 27.50, qty: 2 },\n" +
                    "    { title: 'Breadboard Jumper Wires', price: 6.99, qty: 3 },\n" +
                    "    { title: 'ESP32 Wi-Fi Module', price: 9.80, qty: 1 }\n" +
                    "];\n\n" +
                    "const bill = calculateCart(cart);\n" +
                    "console.log('--- CART INVOICE SUMMARY ---');\n" +
                    "console.log('Subtotal: $' + bill.subtotal.toFixed(2));\n" +
                    "console.log('Tax (8%): $' + bill.tax.toFixed(2));\n" +
                    "console.log('Total Due: $' + bill.total.toFixed(2));\n\n" +
                    "return bill.total;\n"
            ),

            ["htmlcss"] = new LanguageSpec(
                Id: "htmlcss",
                DisplayName: "HTML5 & CSS3",
                LogoEmoji: "🌐",
                BrandHexColor: "#E34F26",
                Version: "HTML5 / CSS3 Standard",
                Paradigm: "Declarative Markup • Responsive Cascading Styles",
                FileExtension: ".html",
                Description: "The core markup and presentation language that structures and styles all modern web pages and user interfaces.",
                KeyFeatures: "Semantic tags (<header>, <main>, <nav>), CSS Flexbox & CSS Grid, media queries, CSS variables, transitions.",
                StarterTemplateCode:
                    "<!DOCTYPE html>\n" +
                    "<html lang=\"en\">\n" +
                    "<head>\n" +
                    "  <meta charset=\"UTF-8\">\n" +
                    "  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n" +
                    "  <title>Bhavani Web Project</title>\n" +
                    "  <style>\n" +
                    "    body { font-family: 'Segoe UI', sans-serif; background: #0F172A; color: #F8FAFC; padding: 24px; margin: 0; }\n" +
                    "    .card { background: #1E293B; border-radius: 12px; padding: 20px; border: 1px solid #334155; max-width: 500px; margin: auto; box-shadow: 0 10px 25px rgba(0,0,0,0.5); }\n" +
                    "    h1 { color: #38BDF8; font-size: 22px; margin-top: 0; }\n" +
                    "    p { color: #CBD5E1; font-size: 14px; line-height: 1.6; }\n" +
                    "    .btn { background: #0284C7; color: white; border: none; padding: 10px 18px; border-radius: 6px; font-weight: bold; cursor: pointer; transition: 0.2s; }\n" +
                    "    .btn:hover { background: #0369A1; transform: translateY(-2px); }\n" +
                    "    .counter-box { margin-top: 16px; padding: 12px; background: #020617; border-radius: 8px; text-align: center; }\n" +
                    "    .count-display { font-size: 28px; font-weight: bold; color: #4ADE80; }\n" +
                    "  </style>\n" +
                    "</head>\n" +
                    "<body>\n" +
                    "  <div class=\"card\">\n" +
                    "    <h1>🚀 Bhavani Tech Interactive Project</h1>\n" +
                    "    <p>You can write custom HTML, CSS styles, and JavaScript and preview the live interactive website instantly!</p>\n" +
                    "    <div class=\"counter-box\">\n" +
                    "      <div class=\"count-display\" id=\"countValue\">0</div>\n" +
                    "      <p>Interactive Click Counter</p>\n" +
                    "      <button class=\"btn\" onclick=\"increment()\">Click Me (+1)</button>\n" +
                    "      <button class=\"btn\" style=\"background:#64748B; margin-left:6px;\" onclick=\"resetCount()\">Reset</button>\n" +
                    "    </div>\n" +
                    "  </div>\n" +
                    "  <script>\n" +
                    "    var count = 0;\n" +
                    "    function increment() {\n" +
                    "      count++;\n" +
                    "      document.getElementById('countValue').innerText = count;\n" +
                    "    }\n" +
                    "    function resetCount() {\n" +
                    "      count = 0;\n" +
                    "      document.getElementById('countValue').innerText = count;\n" +
                    "    }\n" +
                    "  </script>\n" +
                    "</body>\n" +
                    "</html>\n"
            ),

            ["sql"] = new LanguageSpec(
                Id: "sql",
                DisplayName: "SQL",
                LogoEmoji: "🗄️",
                BrandHexColor: "#00758F",
                Version: "SQLite 3 Standard",
                Paradigm: "Declarative • Relational Data Manipulation",
                FileExtension: ".sql",
                Description: "Structured Query Language used to create, query, update, and manage relational database systems.",
                KeyFeatures: "DDL table creation, DML data manipulation, multi-table JOINs, GROUP BY aggregations, transactions.",
                StarterTemplateCode:
                    "-- ========================================================\n" +
                    "-- SQL Relational Database Queries\n" +
                    "-- Bhavani Technology Academy\n" +
                    "-- ========================================================\n\n" +
                    "CREATE TABLE courses (\n" +
                    "    id VARCHAR(10) PRIMARY KEY,\n" +
                    "    title TEXT NOT NULL,\n" +
                    "    category TEXT NOT NULL,\n" +
                    "    credits INT NOT NULL\n" +
                    ");\n\n" +
                    "INSERT INTO courses VALUES ('CS101', 'Computer Fundamentals', 'Core', 4);\n" +
                    "INSERT INTO courses VALUES ('PROG101', 'Polyglot Programming', 'Coding', 4);\n" +
                    "INSERT INTO courses VALUES ('SEC101', 'Cybersecurity & Hacking', 'Security', 5);\n" +
                    "INSERT INTO courses VALUES ('AI101', 'Artificial Intelligence', 'Data Science', 5);\n\n" +
                    "SELECT id, title, category, credits FROM courses WHERE credits >= 4 ORDER BY credits DESC;\n"
            ),

            ["cpp"] = new LanguageSpec(
                Id: "cpp",
                DisplayName: "C++",
                LogoEmoji: "⚙️",
                BrandHexColor: "#00599C",
                Version: "ISO C++20",
                Paradigm: "Compiled • Multi-Paradigm • Systems Programming",
                FileExtension: ".cpp",
                Description: "High-performance systems programming language powering game engines, operating systems, and finance infrastructure.",
                KeyFeatures: "Direct pointer control, zero-overhead abstractions, RAII memory management, templates, standard library (STL).",
                StarterTemplateCode:
                    "// ========================================================\n" +
                    "// ISO C++20 — High Performance Systems Programming\n" +
                    "// Bhavani Technology Academy\n" +
                    "// ========================================================\n" +
                    "#include <iostream>\n" +
                    "#include <vector>\n" +
                    "#include <numeric>\n" +
                    "#include <string>\n\n" +
                    "struct BenchmarkRecord {\n" +
                    "    std::string taskName;\n" +
                    "    double executionNs;\n" +
                    "};\n\n" +
                    "int main() {\n" +
                    "    std::cout << \"=== BHAVANI TECH C++20 SYSTEMS LAB ===\\n\";\n" +
                    "    std::vector<BenchmarkRecord> benchmarks = {\n" +
                    "        {\"SIMD AVX-512 Matrix Mult\", 14.2},\n" +
                    "        {\"Lockless Ringbuffer Dispatch\", 8.5},\n" +
                    "        {\"L1 Cache Ray-Triangle Hit\", 3.1}\n" +
                    "    };\n\n" +
                    "    double totalTime = 0;\n" +
                    "    for (const auto& b : benchmarks) {\n" +
                    "        std::cout << \"[*] Task: \" << b.taskName \n" +
                    "                  << \" | Latency: \" << b.executionNs << \" ns\\n\";\n" +
                    "        totalTime += b.executionNs;\n" +
                    "    }\n" +
                    "    std::cout << \"\\nTotal Combined Latency: \" << totalTime << \" ns\\n\";\n" +
                    "    return 0;\n" +
                    "}\n"
            ),

            ["rust"] = new LanguageSpec(
                Id: "rust",
                DisplayName: "Rust",
                LogoEmoji: "🦀",
                BrandHexColor: "#DEA584",
                Version: "Rust 2024 Edition",
                Paradigm: "Compiled • Memory-Safe • Concurrent Systems",
                FileExtension: ".rs",
                Description: "Fast, reliable systems language preventing memory corruption bugs at compile time without a garbage collector.",
                KeyFeatures: "Ownership and borrowing rules, fearless concurrency, zero-cost abstractions, pattern matching, Cargo ecosystem.",
                StarterTemplateCode:
                    "// ========================================================\n" +
                    "// Rust 2024 — Memory-Safe Systems & Zero-Cost Abstractions\n" +
                    "// Bhavani Technology Academy\n" +
                    "// ========================================================\n\n" +
                    "#[derive(Debug)]\n" +
                    "struct NetworkPacket {\n" +
                    "    source_ip: String,\n" +
                    "    dest_ip: String,\n" +
                    "    payload_bytes: usize,\n" +
                    "}\n\n" +
                    "fn main() {\n" +
                    "    println!(\"🦀 Welcome to Safe Systems Programming in Rust!\");\n\n" +
                    "    let packet = NetworkPacket {\n" +
                    "        source_ip: String::from(\"192.168.1.50\"),\n" +
                    "        dest_ip: String::from(\"10.0.0.1\"),\n" +
                    "        payload_bytes: 1460,\n" +
                    "    };\n\n" +
                    "    println!(\"Packet Routing Header: {:?}\", packet);\n" +
                    "    let throughput_mbps = (packet.payload_bytes as f64 * 8.0) / 1_000_000.0;\n" +
                    "    println!(\"Throughput factor: {:.6} Mbit\", throughput_mbps);\n" +
                    "}\n"
            ),

            ["java"] = new LanguageSpec(
                Id: "java",
                DisplayName: "Java",
                LogoEmoji: "☕",
                BrandHexColor: "#ED8B00",
                Version: "Java 21 LTS",
                Paradigm: "Compiled to Bytecode • Strict OOP • Garbage Collected",
                FileExtension: ".java",
                Description: "Enterprise-grade, cross-platform language running on the JVM with robust type checking and automatic memory management.",
                KeyFeatures: "Class-based OOP, virtual threads (Project Loom), records, streams, extensive enterprise ecosystem (Spring, Android).",
                StarterTemplateCode:
                    "// ========================================================\n" +
                    "// Java 21 LTS — Enterprise Object-Oriented Software\n" +
                    "// Bhavani Technology Academy\n" +
                    "// ========================================================\n" +
                    "import java.util.List;\n" +
                    "import java.util.stream.Collectors;\n\n" +
                    "record CloudNode(String nodeId, String region, int cpuCores, boolean isHealthy) {}\n\n" +
                    "public class Main {\n" +
                    "    public static void main(String[] args) {\n" +
                    "        System.out.println(\"☕ Java 21 Enterprise Cluster Manager\");\n\n" +
                    "        List<CloudNode> cluster = List.of(\n" +
                    "            new CloudNode(\"us-east-prod-01\", \"us-east\", 32, true),\n" +
                    "            new CloudNode(\"eu-west-prod-02\", \"eu-west\", 64, true),\n" +
                    "            new CloudNode(\"ap-south-prod-03\", \"ap-south\", 16, false)\n" +
                    "        );\n\n" +
                    "        List<CloudNode> active = cluster.stream()\n" +
                    "            .filter(CloudNode::isHealthy)\n" +
                    "            .collect(Collectors.toList());\n\n" +
                    "        System.out.println(\"Active Healthy Nodes: \" + active.size());\n" +
                    "        for (CloudNode node : active) {\n" +
                    "            System.out.println(\" -> Node: \" + node.nodeId() + \" (\" + node.region() + \") | Cores: \" + node.cpuCores());\n" +
                    "        }\n" +
                    "    }\n" +
                    "}\n"
            ),

            ["go"] = new LanguageSpec(
                Id: "go",
                DisplayName: "Go (Golang)",
                LogoEmoji: "🐹",
                BrandHexColor: "#00ADD8",
                Version: "Go 1.22",
                Paradigm: "Compiled • Concurrent • Procedural",
                FileExtension: ".go",
                Description: "Simple, fast, and concurrent language designed at Google for modern cloud infrastructure, microservices, and networking tools.",
                KeyFeatures: "Lightweight goroutines, CSP-style channels, fast compile times, zero external runtime dependencies in output binaries.",
                StarterTemplateCode:
                    "// ========================================================\n" +
                    "// Go 1.22 — Concurrent Cloud Services & Micro-APIs\n" +
                    "// Bhavani Technology Academy\n" +
                    "// ========================================================\n" +
                    "package main\n\n" +
                    "import (\n" +
                    "    \"fmt\"\n" +
                    "    \"time\"\n" +
                    ")\n\n" +
                    "type ServicePing struct {\n" +
                    "    Endpoint string\n" +
                    "    LatencyMs int\n" +
                    "}\n\n" +
                    "func main() {\n" +
                    "    fmt.Println(\"🐹 Go Microservice Health Sentinel\")\n" +
                    "    services := []ServicePing{\n" +
                    "        {\"api.auth.internal\", 12},\n" +
                    "        {\"db.primary.internal\", 4},\n" +
                    "        {\"cache.redis.internal\", 1},\n" +
                    "    }\n\n" +
                    "    for _, s := range services {\n" +
                    "        fmt.Printf(\"[PING %v] %s -> %d ms latency\\n\", time.Now().Format(\"15:04:05\"), s.Endpoint, s.LatencyMs)\n" +
                    "    }\n" +
                    "    fmt.Println(\"All 3 internal endpoints healthy and operational.\")\n" +
                    "}\n"
            ),

            ["assembly"] = new LanguageSpec(
                Id: "assembly",
                DisplayName: "x86_64 Assembly",
                LogoEmoji: "📟",
                BrandHexColor: "#4D5BCE",
                Version: "NASM / Intel Syntax",
                Paradigm: "Low-Level Machine Code • Direct Register Control",
                FileExtension: ".asm",
                Description: "The raw instruction set architecture of Intel and AMD processors, revealing CPU registers, stacks, and hardware interrupts.",
                KeyFeatures: "Registers (RAX, RBX, RCX, RDX, RSI, RDI, RSP, RBP), flags register, direct stack pointer manipulation, syscalls.",
                StarterTemplateCode:
                    "; ========================================================\n" +
                    "; x86_64 Assembly (NASM / Intel Syntax)\n" +
                    "; Bhavani Technology Academy — Direct CPU Register Control\n" +
                    "; ========================================================\n" +
                    "global _start\n\n" +
                    "section .data\n" +
                    "    msg db \"Hello from x86_64 Machine Instructions!\", 10\n" +
                    "    len equ $ - msg\n\n" +
                    "section .text\n" +
                    "_start:\n" +
                    "    ; Syscall: sys_write (rax = 1)\n" +
                    "    mov rax, 1          ; System call number (sys_write)\n" +
                    "    mov rdi, 1          ; File descriptor 1 (stdout)\n" +
                    "    lea rsi, [msg]      ; Pointer to message buffer in memory\n" +
                    "    mov rdx, len        ; Number of bytes to transfer\n" +
                    "    syscall             ; Transfer control to OS kernel\n\n" +
                    "    ; Syscall: sys_exit (rax = 60)\n" +
                    "    mov rax, 60         ; System call number (sys_exit)\n" +
                    "    xor rdi, rdi        ; Return exit code 0 (success)\n" +
                    "    syscall             ; Process terminates cleanly\n"
            )
        };

        public static IReadOnlyList<LanguageSpec> GetAllLanguages() => Languages.Values.ToList();

        public static LanguageSpec GetLanguage(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return Languages["python"];
            if (Languages.TryGetValue(id.Trim(), out var spec)) return spec;

            // Alias normalization
            string norm = id.Trim().ToLowerInvariant();
            if (norm.Contains("c#") || norm.Contains("csharp")) return Languages["csharp"];
            if (norm.Contains("js") || norm.Contains("javascript")) return Languages["javascript"];
            if (norm.Contains("html") || norm.Contains("css") || norm.Contains("web")) return Languages["htmlcss"];
            if (norm.Contains("sql")) return Languages["sql"];
            if (norm.Contains("c++") || norm.Contains("cpp")) return Languages["cpp"];
            if (norm.Contains("rust")) return Languages["rust"];
            if (norm.Contains("java") && !norm.Contains("script")) return Languages["java"];
            if (norm.Contains("go") || norm.Contains("golang")) return Languages["go"];
            if (norm.Contains("asm") || norm.Contains("assembly")) return Languages["assembly"];

            return Languages["python"];
        }
    }
}
