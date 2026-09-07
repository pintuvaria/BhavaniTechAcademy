using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public class WebSecuritySimulatorService
    {
        public record SqlInjectionResult(bool IsBypassed, string QueryExecuted, string Explanation);

        public SqlInjectionResult SimulateLogin(string username, string password, bool useParameterized)
        {
            if (useParameterized)
            {
                // Parameterized prevents injection
                return new SqlInjectionResult(false, 
                    "SELECT * FROM Users WHERE Username = @User AND Password = @Pass", 
                    "Secure! Parameterized queries treat input as literal strings, not executable code.");
            }
            else
            {
                // Vulnerable concatenation
                string query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";
                
                // Extremely simple logic flaw detector for demonstration (' OR '1'='1)
                bool bypassed = query.Contains("' OR '1'='1") || query.Contains("' OR 1=1") || query.Contains("'--");
                
                string expl = bypassed 
                    ? "Vulnerable! The injected payload altered the SQL logic to always evaluate to true, bypassing authentication."
                    : "Login failed. Payload did not successfully alter the WHERE clause to bypass authentication.";

                return new SqlInjectionResult(bypassed, query, expl);
            }
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
