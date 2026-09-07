using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record PortScanResult(int Port, string Service, string Status, string RiskLevel);
    public record PasswordStrengthResult(string Rating, double EntropyBits, string Feedback);
    public record SqlInjectionLabResult(string QueryExecuted, bool ExploitSuccessful, string ResultData, string SecurityAdvice);
    public record XssLabResult(string RawInput, string RenderedOutput, bool ScriptExecuted, string SecurityAdvice);
    public record LogForensicResult(int TotalLinesScanned, int FailedLoginAttempts, int SuspiciousIPsCount, List<string> SecurityAlerts);

    public record CtfChallenge(
        int Id,
        string Title,
        string Category,
        string Difficulty,
        int XpReward,
        string MissionBriefing,
        string Hint,
        string ExpectedFlag
    );

    public record CtfValidationResult(
        bool Success,
        int XpEarned,
        string FeedbackMessage
    );

    public record PacketStreamEntry(
        int PacketNumber,
        string Timestamp,
        string SourceIp,
        string DestinationIp,
        string Protocol,
        int Length,
        string Summary,
        string RawPayloadText,
        bool IsPlaintextVulnerable
    );

    public record DictionaryCrackResult(
        bool Cracked,
        string PlaintextFound,
        int AttemptsMade,
        double ElapsedTimeMs,
        string SecurityAnalysis
    );

    public record RopGadget(string Address, string AssemblyOpcode, string RegisterEffect);
    public record RopExploitResult(
        bool DepBypassed,
        string TargetPayload,
        List<RopGadget> GadgetChain,
        string ExecutionTrace,
        string MitigationsAnalysis
    );

    public static class CybersecurityLabService
    {
        public static List<PortScanResult> RunSimulatedPortScan(string targetIp)
        {
            return new List<PortScanResult>
            {
                new PortScanResult(21, "FTP", "CLOSED", "Low"),
                new PortScanResult(22, "SSH", "OPEN", "Medium (Ensure Key Authentication)"),
                new PortScanResult(80, "HTTP", "OPEN", "High (Unencrypted Plain Text)"),
                new PortScanResult(443, "HTTPS", "OPEN", "Secure (TLS 1.3 Encryption)"),
                new PortScanResult(3306, "MySQL", "FILTERED", "High (Exposed Database Port)"),
                new PortScanResult(8080, "HTTP-Proxy", "CLOSED", "Low"),
                new PortScanResult(31337, "Elite-Backdoor", "OPEN", "CRITICAL (Unauthorized Listening Daemon)")
            };
        }

        public static string ComputeSha256(string rawData)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public static string ComputeMd5(string rawData)
        {
            using var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public static PasswordStrengthResult EvaluatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return new PasswordStrengthResult("Very Weak", 0, "Password is empty!");

            int length = password.Length;
            int charsetSize = 0;

            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) charsetSize += 26;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) charsetSize += 26;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]")) charsetSize += 10;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[^a-zA-Z0-9]")) charsetSize += 32;

            double entropyBits = length * Math.Log2(Math.Max(2, charsetSize));
            string rating;
            string advice;

            if (entropyBits < 30)
            {
                rating = "Weak (Crackable in seconds)";
                advice = "Add uppercase letters, numbers, and symbols. Increase length > 12.";
            }
            else if (entropyBits < 60)
            {
                rating = "Moderate (Safe for basic accounts)";
                advice = "Good start! Consider using a passphrase for higher entropy.";
            }
            else
            {
                rating = "Strong (Cryptographically Secure)";
                advice = "Excellent password entropy! Hardened against brute-force attacks.";
            }

            return new PasswordStrengthResult(rating, Math.Round(entropyBits, 1), advice);
        }

        public static SqlInjectionLabResult TestSqlInjectionLab(string inputUsername, string inputPassword, bool useParameterizedProtection)
        {
            if (useParameterizedProtection)
            {
                string query = "SELECT * FROM Users WHERE Username = @user AND Password = @pass;";
                return new SqlInjectionLabResult(
                    QueryExecuted: query,
                    ExploitSuccessful: false,
                    ResultData: "Authentication Failed. Safe query prepared statements escaped special SQL characters.",
                    SecurityAdvice: "PROTECTED! Always use Prepared Statements / Parameterized Queries to neutralize SQL Injection."
                );
            }
            else
            {
                string query = $"SELECT * FROM Users WHERE Username = '{inputUsername}' AND Password = '{inputPassword}';";
                bool isBypassed = inputUsername.Contains("' OR '1'='1") || inputPassword.Contains("' OR '1'='1") || inputUsername.Contains("' OR 1=1") || inputUsername.Contains("admin' --");
                
                string data = isBypassed 
                    ? "SUCCESS! Bypass achieved. Returned Record: [ID: 1, User: 'admin', Role: 'Administrator', Flag: 'FLAG{Bhavani_SQLi_Master_9921}']" 
                    : "Access Denied. Normal query execution.";

                return new SqlInjectionLabResult(
                    QueryExecuted: query,
                    ExploitSuccessful: isBypassed,
                    ResultData: data,
                    SecurityAdvice: isBypassed 
                        ? "VULNERABLE! The string concatenation allowed attacker logic `' OR 1=1` to force true condition and bypass authentication."
                        : "Query executed via string concatenation. Highly vulnerable if malicious input is entered."
                );
            }
        }

        public static XssLabResult TestXssLab(string userInput, bool sanitizeOutput)
        {
            bool hasScriptTag = userInput.Contains("<script>", StringComparison.OrdinalIgnoreCase) || 
                                userInput.Contains("javascript:", StringComparison.OrdinalIgnoreCase) ||
                                userInput.Contains("onerror=", StringComparison.OrdinalIgnoreCase);

            if (sanitizeOutput)
            {
                string encoded = WebUtility.HtmlEncode(userInput);
                return new XssLabResult(
                    RawInput: userInput,
                    RenderedOutput: encoded,
                    ScriptExecuted: false,
                    SecurityAdvice: "PROTECTED! HTML Entity Encoding converted dangerous tags like `<script>` into `&lt;script&gt;`, preventing browser script execution."
                );
            }
            else
            {
                return new XssLabResult(
                    RawInput: userInput,
                    RenderedOutput: userInput,
                    ScriptExecuted: hasScriptTag,
                    SecurityAdvice: hasScriptTag 
                        ? "VULNERABLE! Unescaped user input allowed Cross-Site Scripting (XSS). The browser executed arbitrary JavaScript code!" 
                        : "Rendered raw HTML string. Vulnerable to XSS if malicious scripts are injected."
                );
            }
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return "Invalid Base64 string format.";
            }
        }

        public static LogForensicResult AnalyzeLogFile(string rawLogText)
        {
            var lines = rawLogText.Split('\n');
            int failedLogins = 0;
            var alertList = new List<string>();
            var suspiciousIPs = new HashSet<string>();

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (line.Contains("FAILED_LOGIN", StringComparison.OrdinalIgnoreCase) || line.Contains("401 Unauthorized", StringComparison.OrdinalIgnoreCase))
                {
                    failedLogins++;
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                    if (match.Success)
                    {
                        suspiciousIPs.Add(match.Value);
                    }
                }
            }

            if (failedLogins >= 3)
            {
                alertList.Add($"[ALERT] Potential SSH/Web Brute Force Attack detected! ({failedLogins} failed logins)");
            }
            foreach (var ip in suspiciousIPs)
            {
                alertList.Add($"[SUSPICIOUS IP] {ip} exceeded failure threshold");
            }

            return new LogForensicResult(lines.Length, failedLogins, suspiciousIPs.Count, alertList);
        }

        // =====================================================================
        // CTF (CAPTURE THE FLAG) ETHICAL HACKING ARENA
        // =====================================================================
        public static List<CtfChallenge> GetCtfChallenges()
        {
            return new List<CtfChallenge>
            {
                new CtfChallenge(
                    1,
                    "Challenge 1: SQL Injection Login Bypass",
                    "Web Security",
                    "Beginner",
                    100,
                    "MISSION: A legacy student administration portal is vulnerable to dynamic SQL concatenation.\nTarget Login: http://simulated-portal.local/login.php\nTask: Input an authentication bypass string to log in as administrator and extract the secret flag!",
                    "HINT: What happens when you supply ' OR '1'='1 in the username field?",
                    "FLAG{Bhavani_SQLi_Master_9921}"
                ),
                new CtfChallenge(
                    2,
                    "Challenge 2: Cryptographic Hash Breaking",
                    "Cryptography",
                    "Intermediate",
                    150,
                    "MISSION: A leaked database record reveals a user password stored as an unsalted MD5 hash:\nTarget Hash: 5f4dcc3b5aa765d61d8327deb882cf99\nTask: Use the offline dictionary attack tool to crack the plaintext password, then format the flag as: FLAG{<plaintext_password>}",
                    "HINT: This is one of the most common 6-letter English words for password!",
                    "FLAG{password}"
                ),
                new CtfChallenge(
                    3,
                    "Challenge 3: Port Recon & Rogue Daemon Hunting",
                    "Network Recon",
                    "Intermediate",
                    150,
                    "MISSION: A rogue service is listening on an obscure port on the internal server (127.0.0.1).\nTask: Run the port scan, inspect port 31337, and identify the unauthorized flag in the service banner!",
                    "HINT: Inspect the high port 31337 on the simulated host.",
                    "FLAG{Bhavani_Backdoor_Found_7719}"
                ),
                new CtfChallenge(
                    4,
                    "Challenge 4: Covert Steganography & Base64 Payload",
                    "Digital Forensics",
                    "Advanced",
                    200,
                    "MISSION: An adversary exfiltrated an intercepted network stream encoded in Base64:\nEncoded Payload: RkxBR3tCaGF2YW5pX0NvdmVydF9EZWNvZGVkXzEwNTJ9\nTask: Decode the Base64 stream to reveal the secret flag!",
                    "HINT: Use the Base64 Decoder in the Hasher tab or Local AI prompt.",
                    "FLAG{Bhavani_Covert_Decoded_1052}"
                ),
                new CtfChallenge(
                    5,
                    "Challenge 5: Linux SUID Privilege Escalation",
                    "System Security",
                    "Master",
                    250,
                    "MISSION: A misconfigured Linux binary `/usr/local/bin/backup_tool` has the SUID bit set (`-rwsr-xr-x`).\nBy calling `strings /usr/local/bin/backup_tool`, you uncover the embedded hardcoded root master key:\nFlag: FLAG{Bhavani_Root_PrivEsc_8830}\nTask: Submit the extracted root authorization token.",
                    "HINT: Misconfigured SUID binaries execute with the permissions of the file owner (root).",
                    "FLAG{Bhavani_Root_PrivEsc_8830}"
                )
            };
        }

        public static CtfValidationResult ValidateCtfFlag(int challengeId, string submittedFlag)
        {
            var challenges = GetCtfChallenges();
            var challenge = challenges.Find(c => c.Id == challengeId);

            if (challenge == null)
            {
                return new CtfValidationResult(false, 0, "Challenge not found.");
            }

            string cleanSubmitted = submittedFlag.Trim();
            if (string.Equals(cleanSubmitted, challenge.ExpectedFlag, StringComparison.OrdinalIgnoreCase))
            {
                return new CtfValidationResult(
                    true,
                    challenge.XpReward,
                    $"CONGRATULATIONS! Flag Verified Successfully! You earned +{challenge.XpReward} XP for solving {challenge.Title}!"
                );
            }
            else
            {
                return new CtfValidationResult(
                    false,
                    0,
                    "Incorrect Flag! Double check your exploit syntax or decoding method and try again."
                );
            }
        }

        // =====================================================================
        // WIRESHARK / PACKET SNIFFER STREAM INSPECTOR
        // =====================================================================
        public static List<PacketStreamEntry> GetSimulatedPacketStream()
        {
            return new List<PacketStreamEntry>
            {
                new PacketStreamEntry(
                    1,
                    "00:00.012",
                    "192.168.1.10",
                    "192.168.1.1",
                    "DNS",
                    74,
                    "Standard query 0x1a2b A student-portal.bhavani.edu",
                    "DNS Query: student-portal.bhavani.edu [Type A, Class IN]",
                    false
                ),
                new PacketStreamEntry(
                    2,
                    "00:00.018",
                    "192.168.1.1",
                    "192.168.1.10",
                    "DNS",
                    90,
                    "Standard query response 0x1a2b A 192.168.1.50",
                    "DNS Response: student-portal.bhavani.edu -> 192.168.1.50",
                    false
                ),
                new PacketStreamEntry(
                    3,
                    "00:00.045",
                    "192.168.1.10",
                    "192.168.1.50",
                    "TCP",
                    66,
                    "54321 -> 80 [SYN] Seq=0 Win=64240",
                    "TCP SYN Packet - Establishing unencrypted HTTP connection",
                    false
                ),
                new PacketStreamEntry(
                    4,
                    "00:00.048",
                    "192.168.1.50",
                    "192.168.1.10",
                    "TCP",
                    66,
                    "80 -> 54321 [SYN, ACK] Seq=0 Ack=1 Win=65160",
                    "TCP SYN-ACK Packet - Handshake in progress",
                    false
                ),
                new PacketStreamEntry(
                    5,
                    "00:00.052",
                    "192.168.1.10",
                    "192.168.1.50",
                    "HTTP",
                    412,
                    "POST /login.php HTTP/1.1 (application/x-www-form-urlencoded)",
                    "POST /login.php HTTP/1.1\r\nHost: 192.168.1.50\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\nusername=admin&password=SecretSuperPassword2026!&token=xyz998",
                    true // Plaintext password leaked!
                ),
                new PacketStreamEntry(
                    6,
                    "00:00.089",
                    "192.168.1.10",
                    "10.0.0.1",
                    "TLS 1.3",
                    528,
                    "Application Data (AES-256-GCM Encrypted Payload)",
                    "[ENCRYPTED CIPHERTEXT: a4 b8 c9 d0 e1 f2 33 44 55 66 77 88 99 aa bb cc ... Zero plaintext leaked!]",
                    false // Encrypted and safe
                )
            };
        }

        // =====================================================================
        // PASSWORD DICTIONARY ATTACK SIMULATOR
        // =====================================================================
        public static DictionaryCrackResult RunDictionaryAttack(string targetHash, bool isMd5)
        {
            var wordlist = new[]
            {
                "123456", "password", "12345678", "qwerty", "123456789", "12345", "1234", "111111",
                "1234567", "dragon", "welcome", "bhavani", "admin", "administrator", "student", "ninja",
                "superman", "iloveyou", "secret", "master", "charlie", "monkey", "shadow", "sunshine"
            };

            var sw = System.Diagnostics.Stopwatch.StartNew();
            int attempts = 0;

            foreach (var word in wordlist)
            {
                attempts++;
                string hash = isMd5 ? ComputeMd5(word) : ComputeSha256(word);
                if (string.Equals(hash, targetHash.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    sw.Stop();
                    return new DictionaryCrackResult(
                        true,
                        word,
                        attempts,
                        sw.Elapsed.TotalMilliseconds,
                        $"CRACKED! The password '{word}' was found in the dictionary after {attempts} attempts. Unsalted hashes provide zero defense against dictionary lookup attacks."
                    );
                }
            }

            sw.Stop();
            return new DictionaryCrackResult(
                false,
                "",
                attempts,
                sw.Elapsed.TotalMilliseconds,
                $"FAILED TO CRACK: Exhausted {attempts} dictionary entries without finding a matching plaintext. Note: Larger wordlists (e.g. rockyou.txt) or cryptographic salts are the next line of defense."
            );
        }

        // =====================================================================
        // RETURN-ORIENTED PROGRAMMING (ROP CHAIN) EXPLOIT SIMULATOR
        // =====================================================================
        public static RopExploitResult SimulateRopChain(string payloadTarget = "/bin/sh")
        {
            var gadgets = new List<RopGadget>
            {
                new("0x00007ffff7de1230", "pop rdi ; ret", "Pops address of target string ('" + payloadTarget + "') into RDI (Arg 1 for x86_64 SysV ABI)"),
                new("0x00007ffff7f845a0", "pop rsi ; ret", "Pops 0x0 (NULL pointer for argv[]) into RSI (Arg 2)"),
                new("0x00007ffff7e129b0", "pop rdx ; ret", "Pops 0x0 (NULL pointer for envp[]) into RDX (Arg 3)"),
                new("0x00007ffff7d45120", "pop rax ; ret", "Pops 59 (SYS_execve syscall number) into RAX"),
                new("0x00007ffff7df8900", "syscall ; ret", "Executes Linux Kernel Syscall: execve('/bin/sh', NULL, NULL)")
            };

            var trace = new StringBuilder();
            trace.AppendLine("=== ROP CHAIN MICRO-EXECUTION TRACE (x86_64 SysV ABI) ===");
            trace.AppendLine("[+] Non-Executable Stack (DEP/NX) is ACTIVE: Injected shellcode on stack cannot execute.");
            trace.AppendLine("[+] Attacker controls RSP (Stack Pointer) via buffer overflow return address overwrite.");
            trace.AppendLine("");

            foreach (var g in gadgets)
            {
                trace.AppendLine($"[STACK -> RIP] Jump to Gadget: {g.Address} | Opcode: {g.AssemblyOpcode}");
                trace.AppendLine($"               Effect: {g.RegisterEffect}");
            }

            trace.AppendLine("");
            trace.AppendLine($"[+] Syscall Dispatched: execve(\"{payloadTarget}\", NULL, NULL)");
            trace.AppendLine("[+] DEP/NX Bypassed: Arbitrary execution achieved solely by reusing existing executable .text segments!");

            string mitigation = "Defenses against ROP Chains:\n" +
                "1. Full ASLR (Address Space Layout Randomization): Randomizes libc and binary base addresses, making gadget addresses unpredictable.\n" +
                "2. Stack Canaries (-fstack-protector-all): Detects buffer overwrite before RET instruction.\n" +
                "3. Control Flow Integrity (CFI) & Intel CET (Control-Flow Enforcement Technology): Hardware shadow stacks track return addresses and abort execution if modified.";

            return new RopExploitResult(true, payloadTarget, gadgets, trace.ToString(), mitigation);
        }
    }
}
