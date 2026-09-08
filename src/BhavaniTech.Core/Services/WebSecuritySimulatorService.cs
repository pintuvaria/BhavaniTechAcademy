using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public class WebSecuritySimulatorService
    {
        public enum SqlInjectionType { None, Tautology, UnionBased, BlindTimeBased }

        public record SqlInjectionResult(
            bool IsBypassed,
            string QueryExecuted,
            string Explanation,
            SqlInjectionType InjectionType,
            bool WafBypassed
        );

        public SqlInjectionResult SimulateLogin(string username, string password, bool useParameterized, bool enableWaf = false)
        {
            if (useParameterized)
            {
                // Parameterized prevents injection completely
                return new SqlInjectionResult(
                    IsBypassed: false, 
                    QueryExecuted: "SELECT * FROM Users WHERE Username = @User AND Password = @Pass", 
                    Explanation: "Secure! Parameterized queries (prepared statements) treat all inputs as literal strings, preventing syntax alteration.",
                    InjectionType: SqlInjectionType.None,
                    WafBypassed: false
                );
            }

            // Raw dynamic SQL construction
            string query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";
            string combined = (username + " " + password).ToUpperInvariant();

            // Detect injection category
            SqlInjectionType injectionType = SqlInjectionType.None;
            if (combined.Contains("UNION SELECT") || combined.Contains("UNION/**/SELECT"))
            {
                injectionType = SqlInjectionType.UnionBased;
            }
            else if (combined.Contains("WAITFOR DELAY") || combined.Contains("SLEEP(") || combined.Contains("BENCHMARK("))
            {
                injectionType = SqlInjectionType.BlindTimeBased;
            }
            else if (combined.Contains("' OR '1'='1") || combined.Contains("' OR 1=1") || combined.Contains("'--") || combined.Contains("' /*"))
            {
                injectionType = SqlInjectionType.Tautology;
            }

            bool bypassed = injectionType != SqlInjectionType.None;
            bool wafBlocked = false;
            bool wafEvaded = false;

            if (enableWaf && bypassed)
            {
                // Basic WAF looks for exact space-separated keywords
                if (combined.Contains("UNION SELECT") || combined.Contains("' OR '1'='1'"))
                {
                    wafBlocked = true;
                    bypassed = false;
                }
                else if (combined.Contains("/**/"))
                {
                    // Evaded using inline comment obfuscation
                    wafEvaded = true;
                    bypassed = true;
                }
            }

            string expl;
            if (wafBlocked)
            {
                expl = "BLOCKED BY WAF: Signature matched common attack pattern. Web Application Firewall dropped the request.";
            }
            else if (wafEvaded)
            {
                expl = "WAF EVASION SUCCESSFUL: Obfuscated SQL payload (e.g. inline SQL comments /**/) bypassed simple signature detection filters!";
            }
            else if (bypassed)
            {
                expl = injectionType switch
                {
                    SqlInjectionType.Tautology => "Vulnerable! The tautology (' OR '1'='1) made the WHERE clause evaluate to TRUE for all rows, logging in as the first user (admin).",
                    SqlInjectionType.UnionBased => "Vulnerable! UNION SELECT appended an extra result set, allowing attacker to dump data from unauthorized database tables.",
                    SqlInjectionType.BlindTimeBased => "Vulnerable! Time-based blind payload injected sleep delay, allowing side-channel database enumeration bit-by-bit.",
                    _ => "Vulnerable SQL injection detected."
                };
            }
            else
            {
                expl = "Login failed. Payload did not successfully alter the SQL Abstract Syntax Tree (AST).";
            }

            return new SqlInjectionResult(bypassed, query, expl, injectionType, wafEvaded);
        }

        public record XssSimulationResult(bool IsExecuted, string RenderedDom, string Explanation);

        public XssSimulationResult SimulateCommentBoard(string commentInput, bool useOutputEncoding)
        {
            if (useOutputEncoding)
            {
                string encoded = commentInput.Replace("<", "&lt;").Replace(">", "&gt;");
                return new XssSimulationResult(false, 
                    $"<div>{encoded}</div>", 
                    "Secure! HTML entities were encoded. The browser treats the input as text, not executable JavaScript.");
            }
            else
            {
                bool executed = commentInput.Contains("<script>") || commentInput.Contains("onerror=");
                return new XssSimulationResult(executed, 
                    $"<div>{commentInput}</div>", 
                    executed 
                        ? "Vulnerable! Unescaped input was rendered directly in the DOM. The browser executed the malicious JavaScript payload." 
                        : "No active XSS payload detected, but the field remains vulnerable to unescaped input.");
            }
        }
    }
}
