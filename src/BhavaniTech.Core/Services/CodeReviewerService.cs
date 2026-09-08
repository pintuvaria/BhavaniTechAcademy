using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BhavaniTech.Core.Services
{
    public record CodeReviewIssue(
        string Category,
        string Severity, // "CRITICAL", "WARNING", "SUGGESTION"
        string Description,
        int LineNumber,
        string Recommendation
    );

    public record CodeReviewResult(
        string Language,
        int TotalIssues,
        List<CodeReviewIssue> Issues,
        string SecurityScore, // e.g. "A+", "B", "C", "F"
        string AuditSummary,
        string PatchedCode
    );

    public static class CodeReviewerService
    {
        public static CodeReviewResult ReviewCode(string code, string language = "csharp")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new CodeReviewResult(
                    language,
                    0,
                    new List<CodeReviewIssue>(),
                    "N/A",
                    "No code provided to review.",
                    "// Paste your code here to inspect for vulnerabilities and optimizations"
                );
            }

            var issues = new List<CodeReviewIssue>();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string lang = language.ToLowerInvariant();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                int lineNum = i + 1;

                // 1. SQL Injection vulnerability check
                if (Regex.IsMatch(line, @"(?i)(SELECT|INSERT|UPDATE|DELETE)\b.*(\+|format\(|\$""|\{).*(FROM|INTO|SET|WHERE)"))
                {
                    issues.Add(new CodeReviewIssue(
                        "Security / SQL Injection",
                        "CRITICAL",
                        "Direct string concatenation or interpolation detected in SQL statement.",
                        lineNum,
                        "Use parameterized queries (e.g., cmd.Parameters.AddWithValue) or an ORM to prevent SQLi."
                    ));
                }

                // 2. Resource / Memory Leak (Missing using or dispose)
                if (lang.Contains("cs") || lang.Contains("csharp"))
                {
                    if (Regex.IsMatch(line, @"new\s+(StreamReader|StreamWriter|FileStream|HttpClient|SqliteConnection|SqlConnection)\(") &&
                        !line.StartsWith("using") && !lines.Take(i).Any(l => l.Contains("using (" + line.Split('=')[0].Trim())))
                    {
                        issues.Add(new CodeReviewIssue(
                            "Resource Management",
                            "WARNING",
                            "Disposable resource instantiated without a 'using' declaration or statement.",
                            lineNum,
                            "Wrap in 'using var ...' or 'using (...) { }' to guarantee deterministic disposal."
                        ));
                    }
                }

                // 3. Unsafe memory & Buffer Overflow vulnerabilities in C/C++
                if (lang.Contains("c") || lang.Contains("cpp"))
                {
                    if (Regex.IsMatch(line, @"\b(strcpy|strcat|sprintf|gets)\b"))
                    {
                        issues.Add(new CodeReviewIssue(
                            "Memory Safety / Buffer Overflow",
                            "CRITICAL",
                            "Use of inherently unsafe C-string manipulation functions prone to buffer overflow.",
                            lineNum,
                            "Replace with bounded alternatives: strncpy_s, snprintf, or std::string."
                        ));
                    }
                }

                // 4. Empty Catch Blocks (Exception Swallowing)
                if (Regex.IsMatch(line, @"catch\s*(\([^\)]*\))?\s*\{\s*\}"))
                {
                    issues.Add(new CodeReviewIssue(
                        "Error Handling",
                        "WARNING",
                        "Empty catch block detected. Swallowing exceptions obscures runtime errors.",
                        lineNum,
                        "Log the caught exception or rethrow with 'throw;'."
                    ));
                }

                // 5. Hardcoded Credentials / Secrets
                if (Regex.IsMatch(line, @"(?i)(password|secret|apikey|api_key|token)\s*=\s*[""'][^""']{4,}[""']"))
                {
                    issues.Add(new CodeReviewIssue(
                        "Hardcoded Secret",
                        "CRITICAL",
                        "Potential hardcoded password or API credential detected in plain text.",
                        lineNum,
                        "Store credentials in environment variables or a secure key vault."
                    ));
                }

                // 6. Infinite Loop Hazard
                if (Regex.IsMatch(line, @"while\s*\(\s*(true|1)\s*\)") && !code.Contains("break") && !code.Contains("return"))
                {
                    issues.Add(new CodeReviewIssue(
                        "Algorithmic Reliability",
                        "WARNING",
                        "Infinite loop pattern without an observable exit break condition.",
                        lineNum,
                        "Ensure an exit condition triggers 'break;' or 'return;' to prevent freezing."
                    ));
                }

                // 7. Weak Cryptography (MD5 / SHA1)
                if (Regex.IsMatch(line, @"(?i)\b(MD5|SHA1)\.Create\(\)"))
                {
                    issues.Add(new CodeReviewIssue(
                        "Cryptography",
                        "WARNING",
                        "MD5 or SHA-1 is cryptographically broken and vulnerable to collision attacks.",
                        lineNum,
                        "Use SHA-256 or SHA-512 (e.g., SHA256.Create()) for cryptographic integrity."
                    ));
                }
            }

            // Calculate overall security grade
            int criticalCount = issues.Count(x => x.Severity == "CRITICAL");
            int warningCount = issues.Count(x => x.Severity == "WARNING");

            string score = criticalCount > 0 ? (criticalCount > 2 ? "F" : "C-")
                         : warningCount > 2 ? "B-"
                         : warningCount > 0 ? "B+"
                         : "A+";

            string auditSummary = criticalCount > 0
                ? $"🚨 {criticalCount} Critical Vulnerabilities Found! Code requires security remediation before deployment."
                : warningCount > 0
                ? $"⚠️ {warningCount} Code Quality Warnings. Functionally operational, but optimizations recommended."
                : "🛡️ Outstanding! Zero security flaws or anti-patterns detected. Code is robust and secure.";

            // Generate an automated suggested patch
            string patched = GeneratePatchedCode(code, issues);

            return new CodeReviewResult(
                language,
                issues.Count,
                issues,
                score,
                auditSummary,
                patched
            );
        }

        private static string GeneratePatchedCode(string code, List<CodeReviewIssue> issues)
        {
            string patched = code;

            // Patch empty catch
            patched = Regex.Replace(patched, @"catch\s*(\([^\)]*\))?\s*\{\s*\}", "catch (Exception ex)\n{\n    // Log error securely\n    Console.Error.WriteLine($\"Runtime Error: {ex.Message}\");\n}");

            // Patch MD5 to SHA256
            patched = Regex.Replace(patched, @"MD5\.Create\(\)", "SHA256.Create()");

            // Patch unsafe gets to fgets
            patched = Regex.Replace(patched, @"\bgets\(([^)]+)\)", "fgets($1, sizeof($1), stdin)");

            return patched;
        }
    }
}
