using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using BhavaniTech.Core.Database;
using BhavaniTech.Core.Models;
using Microsoft.Data.Sqlite;

namespace BhavaniTech.Core.Services
{
    // =========================================================================
    // 1. OFFLINE TEXT-TO-SPEECH (TTS) VOICE MENTOR
    // =========================================================================
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class TextToSpeechService
    {
        private static SpeechSynthesizer? _synth;
        private static readonly object _lock = new();

        public static void Initialize()
        {
            try
            {
                lock (_lock)
                {
                    if (_synth == null)
                    {
                        _synth = new SpeechSynthesizer();
                        _synth.SetOutputToDefaultAudioDevice();
                    }
                }
            }
            catch
            {
                // Fallback gracefully on systems without standard audio endpoints
            }
        }

        public static void SpeakAsync(string text, int rate = 0, int volume = 100)
        {
            try
            {
                lock (_lock)
                {
                    Initialize();
                    if (_synth != null)
                    {
                        _synth.SpeakAsyncCancelAll();
                        _synth.Rate = Math.Clamp(rate, -10, 10);
                        _synth.Volume = Math.Clamp(volume, 0, 100);
                        _synth.SpeakAsync(text);
                    }
                }
            }
            catch
            {
                // Non-fatal if audio hardware is disabled
            }
        }

        public static void Stop()
        {
            try
            {
                lock (_lock)
                {
                    _synth?.SpeakAsyncCancelAll();
                }
            }
            catch { }
        }
    }

    // =========================================================================
    // 2. VISUAL GIT BRANCHING & MERGE CONFLICT SIMULATOR
    // =========================================================================
    public class VisualGitSimulator
    {
        private readonly List<GitNode> _commits = new();
        private readonly Dictionary<string, string> _branches = new(); // Branch -> Head Commit Hash
        private string _currentBranch = "main";

        public VisualGitSimulator()
        {
            Reset();
        }

        public void Reset()
        {
            _commits.Clear();
            _branches.Clear();
            _currentBranch = "main";

            // Root commit
            var root = new GitNode("c1a01", "Initial commit (Repo initialized)", "main", null, DateTime.UtcNow.AddMinutes(-30));
            _commits.Add(root);
            _branches["main"] = root.CommitHash;
        }

        public string CurrentBranch => _currentBranch;

        public GitNode Commit(string message)
        {
            string? parentHash = _branches.TryGetValue(_currentBranch, out var head) ? head : null;
            string newHash = "c" + Guid.NewGuid().ToString("N")[..4];
            var commit = new GitNode(newHash, message, _currentBranch, parentHash, DateTime.UtcNow);
            _commits.Add(commit);
            _branches[_currentBranch] = newHash;
            return commit;
        }

        public bool CreateBranch(string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName) || _branches.ContainsKey(branchName)) return false;
            string currentHead = _branches[_currentBranch];
            _branches[branchName] = currentHead;
            return true;
        }

        public bool Checkout(string branchName)
        {
            if (!_branches.ContainsKey(branchName)) return false;
            _currentBranch = branchName;
            return true;
        }

        public (bool Success, string MergeMessage, bool HasConflict) Merge(string sourceBranch)
        {
            if (!_branches.ContainsKey(sourceBranch) || sourceBranch == _currentBranch)
                return (false, "Invalid source branch for merge.", false);

            string currentHead = _branches[_currentBranch];
            string sourceHead = _branches[sourceBranch];

            // If source branch has diverged with specific conflict scenario
            bool conflict = sourceBranch.Contains("conflict", StringComparison.OrdinalIgnoreCase);
            if (conflict)
            {
                return (false, $"💥 MERGE CONFLICT in src/App.cs!\nAuto-merging failed. Fix conflicts and then commit the result.", true);
            }

            // Create merge commit
            string mergeHash = "m" + Guid.NewGuid().ToString("N")[..4];
            var mergeCommit = new GitNode(mergeHash, $"Merge branch '{sourceBranch}' into {_currentBranch}", _currentBranch, currentHead, DateTime.UtcNow);
            _commits.Add(mergeCommit);
            _branches[_currentBranch] = mergeHash;

            return (true, $"✅ Fast-forward & recursive merge complete! Created merge commit [{mergeHash}].", false);
        }

        public List<GitNode> GetCommitGraph() => new(_commits);
        public List<string> GetBranches() => _branches.Keys.ToList();
    }

    // =========================================================================
    // 3. CODE STEP-DEBUGGER SERVICE
    // =========================================================================
    public class CodeStepDebuggerService
    {
        public static List<DebuggerFrameState> TraceExecution(string programType)
        {
            var frames = new List<DebuggerFrameState>();

            switch (programType)
            {
                case "fibonacci":
                    frames.Add(new DebuggerFrameState(1, "int a = 0, b = 1;", new Dictionary<string, string> { { "a", "0" }, { "b", "1" } }, new List<string> { "Main()" }, ""));
                    frames.Add(new DebuggerFrameState(2, "for (int i = 0; i < 3; i++) {", new Dictionary<string, string> { { "a", "0" }, { "b", "1" }, { "i", "0" } }, new List<string> { "Main()" }, ""));
                    frames.Add(new DebuggerFrameState(3, "    int temp = a + b;", new Dictionary<string, string> { { "a", "0" }, { "b", "1" }, { "temp", "1" } }, new List<string> { "Main()" }, ""));
                    frames.Add(new DebuggerFrameState(4, "    a = b; b = temp;", new Dictionary<string, string> { { "a", "1" }, { "b", "1" } }, new List<string> { "Main()" }, "Yield: 1\n"));
                    frames.Add(new DebuggerFrameState(2, "for (int i = 0; i < 3; i++) { [Iter 2]", new Dictionary<string, string> { { "a", "1" }, { "b", "1" }, { "i", "1" } }, new List<string> { "Main()" }, "Yield: 1\n"));
                    frames.Add(new DebuggerFrameState(3, "    int temp = a + b;", new Dictionary<string, string> { { "a", "1" }, { "b", "1" }, { "temp", "2" } }, new List<string> { "Main()" }, "Yield: 1\n"));
                    frames.Add(new DebuggerFrameState(4, "    a = b; b = temp;", new Dictionary<string, string> { { "a", "1" }, { "b", "2" } }, new List<string> { "Main()" }, "Yield: 1\nYield: 2\n"));
                    break;

                case "callstack":
                    frames.Add(new DebuggerFrameState(1, "void Main() { ProcessPayment(49.99); }", new Dictionary<string, string>(), new List<string> { "Main()" }, ""));
                    frames.Add(new DebuggerFrameState(5, "void ProcessPayment(double amount) { ValidateCard(); }", new Dictionary<string, string> { { "amount", "$49.99" } }, new List<string> { "Main()", "ProcessPayment(49.99)" }, "Calling bank gateway...\n"));
                    frames.Add(new DebuggerFrameState(10, "bool ValidateCard() { return true; }", new Dictionary<string, string> { { "cvvValid", "True" }, { "expiry", "2029" } }, new List<string> { "Main()", "ProcessPayment(49.99)", "ValidateCard()" }, "Calling bank gateway...\nCard validated!\n"));
                    frames.Add(new DebuggerFrameState(6, "/* Returning to ProcessPayment */", new Dictionary<string, string> { { "status", "Approved" } }, new List<string> { "Main()", "ProcessPayment(49.99)" }, "Payment approved.\n"));
                    frames.Add(new DebuggerFrameState(2, "/* Main Execution Complete */", new Dictionary<string, string> { { "exitCode", "0" } }, new List<string> { "Main()" }, "Payment approved.\nTransaction complete.\n"));
                    break;

                default:
                    frames.Add(new DebuggerFrameState(1, "int sum = 0;", new Dictionary<string, string> { { "sum", "0" } }, new List<string> { "Main()" }, ""));
                    frames.Add(new DebuggerFrameState(2, "sum += 10;", new Dictionary<string, string> { { "sum", "10" } }, new List<string> { "Main()" }, "sum=10\n"));
                    break;
            }

            return frames;
        }
    }

    // =========================================================================
    // 4. OFFLINE REST API CLIENT SERVICE
    // =========================================================================
    public class OfflineRestApiClientService
    {
        public static RestApiResponse SendRequest(RestApiRequest req)
        {
            var sw = Stopwatch.StartNew();
            var headers = new Dictionary<string, string>
            {
                { "Server", "Bhavani-SimEngine/2.0" },
                { "Content-Type", "application/json; charset=utf-8" },
                { "X-Powered-By", "BhavaniTech" }
            };

            string method = req.Method.ToUpperInvariant();
            string path = req.Url.Trim().ToLowerInvariant();

            if (path.Contains("/api/users") || path.Contains("/api/students") || path.Contains("/api/v1/students"))
            {
                if (method == "GET")
                {
                    sw.Stop();
                    return new RestApiResponse(200, "OK", headers,
                        "[\n  { \"id\": 1, \"name\": \"Dharmesh Varia\", \"role\": \"Founder & Architect\" },\n  { \"id\": 2, \"name\": \"Pratik\", \"role\": \"Lead Engineer\" }\n]", sw.ElapsedMilliseconds + 2);
                }
                else if (method == "POST")
                {
                    sw.Stop();
                    return new RestApiResponse(201, "Created", headers,
                        "{\n  \"status\": \"success\",\n  \"message\": \"User record created successfully\",\n  \"id\": 104\n}", sw.ElapsedMilliseconds + 5);
                }
            }
            else if (path.Contains("/api/courses") || path.Contains("/api/v1/courses"))
            {
                sw.Stop();
                return new RestApiResponse(200, "OK", headers,
                    "[\n  { \"id\": \"CS101\", \"title\": \"Computer Fundamentals\" },\n  { \"id\": \"PROG101\", \"title\": \"Polyglot Programming\" }\n]", sw.ElapsedMilliseconds + 2);
            }
            else if (path.Contains("/api/leaderboard") || path.Contains("/api/v1/leaderboard"))
            {
                sw.Stop();
                return new RestApiResponse(200, "OK", headers,
                    "[\n  { \"rank\": 1, \"student\": \"Dharmesh Varia\", \"xp\": 3500 }\n]", sw.ElapsedMilliseconds + 2);
            }
            else if (path.Contains("/api/health") || path.Contains("/healthz"))
            {
                sw.Stop();
                return new RestApiResponse(200, "OK", headers,
                    "{\n  \"uptime\": \"99.99%\",\n  \"memory_used_mb\": 44,\n  \"database\": \"connected (SQLite WAL)\"\n}", sw.ElapsedMilliseconds + 1);
            }
            else if (path.Contains("/api/auth/token"))
            {
                sw.Stop();
                return new RestApiResponse(200, "OK", headers,
                    "{\n  \"token_type\": \"Bearer\",\n  \"access_token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\",\n  \"expires_in\": 3600\n}", sw.ElapsedMilliseconds + 4);
            }

            sw.Stop();
            return new RestApiResponse(404, "Not Found", headers,
                "{\n  \"error\": \"Resource not found\",\n  \"path\": \"" + req.Url + "\"\n}", sw.ElapsedMilliseconds + 2);
        }
    }

    // =========================================================================
    // 5. REGEX LAB & PATTERN MATCHER SERVICE
    // =========================================================================
    public class RegexLabService
    {
        public static RegexResult Evaluate(string pattern, string input, RegexOptions options = RegexOptions.None)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern))
                    return new RegexResult(false, 0, new(), new(), "Enter a regular expression pattern.");

                var rx = new Regex(pattern, options, TimeSpan.FromSeconds(2));
                var matches = rx.Matches(input);

                var matchStrings = new List<string>();
                var groupList = new List<Dictionary<string, string>>();

                foreach (Match m in matches)
                {
                    matchStrings.Add(m.Value);
                    var gDict = new Dictionary<string, string>();
                    for (int i = 0; i < m.Groups.Count; i++)
                    {
                        string gName = rx.GroupNameFromNumber(i);
                        gDict[gName] = m.Groups[i].Value;
                    }
                    groupList.Add(gDict);
                }

                string expl = $"Pattern matched {matches.Count} instance(s) across {input.Length} characters.";
                return new RegexResult(matches.Count > 0, matches.Count, matchStrings, groupList, expl);
            }
            catch (Exception ex)
            {
                return new RegexResult(false, 0, new(), new(), $"Regex Syntax Error: {ex.Message}");
            }
        }
    }

    // =========================================================================
    // 6. SPACED REPETITION FLASHCARD SYSTEM (LEITNER 5-BOX)
    // =========================================================================
    public class SpacedRepetitionService
    {
        public static List<Flashcard> GetDefaultFlashcards()
        {
            return new List<Flashcard>
            {
                new(1, "Networking", "What is the standard port for HTTPS encryption?", "Port 443 (TLS/SSL)", 1, DateTime.UtcNow.ToString("o")),
                new(2, "Networking", "What is the standard port for DNS name resolution?", "Port 53 (UDP / TCP)", 1, DateTime.UtcNow.ToString("o")),
                new(3, "Networking", "What is the standard port for SSH secure shell?", "Port 22 (TCP)", 1, DateTime.UtcNow.ToString("o")),
                new(4, "Algorithms", "What is the worst-case time complexity of QuickSort?", "O(n^2) when poor pivot is chosen; average is O(n log n)", 1, DateTime.UtcNow.ToString("o")),
                new(5, "Algorithms", "What is the time complexity of Binary Search on a sorted array?", "O(log n)", 1, DateTime.UtcNow.ToString("o")),
                new(6, "Algorithms", "What is the amortized insertion complexity of a Hash Table?", "O(1) average; O(n) worst-case on severe collisions", 1, DateTime.UtcNow.ToString("o")),
                new(7, "Systems & CPU", "What are CPU registers, and how fast are they compared to RAM?", "Fastest on-chip memory storage, operating within 1 clock cycle (~0.5 ns)", 1, DateTime.UtcNow.ToString("o")),
                new(8, "Systems & CPU", "What is the purpose of the CPU Program Counter (PC)?", "Holds the memory address of the next instruction to be fetched and executed", 1, DateTime.UtcNow.ToString("o")),
                new(9, "Systems & CPU", "What is a Translation Lookaside Buffer (TLB)?", "High-speed hardware cache for MMU virtual-to-physical address translations", 1, DateTime.UtcNow.ToString("o")),
                new(10, "Cybersecurity", "What is the difference between Symmetric and Asymmetric encryption?", "Symmetric uses 1 shared key; Asymmetric uses public/private keypair", 1, DateTime.UtcNow.ToString("o")),
                new(11, "Cybersecurity", "What does the 'A' in the CIA triad stand for?", "Availability (ensuring systems and data are accessible to authorized users)", 1, DateTime.UtcNow.ToString("o")),
                new(12, "Cybersecurity", "What is SQL Injection and how is it prevented?", "Attackers manipulate SQL query logic; prevented using Parameterized Queries / Prepared Statements", 1, DateTime.UtcNow.ToString("o")),
                new(13, "Linux", "What does octal permission 'chmod 755' mean?", "User: rwx (7), Group: r-x (5), Others: r-x (5)", 1, DateTime.UtcNow.ToString("o")),
                new(14, "Linux", "What Linux signal is sent by 'kill -9' vs 'kill -15'?", "SIGKILL (9) is uncatchable immediate termination; SIGTERM (15) is graceful shutdown", 1, DateTime.UtcNow.ToString("o")),
                new(15, "Cloud & DevOps", "What is the difference between a Container and a Virtual Machine?", "Containers share the host OS kernel via namespaces/cgroups; VMs virtualize full hardware via a hypervisor", 1, DateTime.UtcNow.ToString("o")),
                new(16, "Databases", "What does the ACID acronym stand for?", "Atomicity, Consistency, Isolation, Durability", 1, DateTime.UtcNow.ToString("o")),
                new(17, "Databases", "Why do B-Trees excel as database disk indexes?", "Wide branching factor minimizes disk block reads (shallow tree height)", 1, DateTime.UtcNow.ToString("o")),
                new(18, "Artificial Intelligence", "What is the formula for the Sigmoid activation function?", "S(x) = 1 / (1 + e^(-x))", 1, DateTime.UtcNow.ToString("o"))
            };
        }

        public static (int NewBox, DateTime NextReview) PromoteCard(int currentBox)
        {
            int nextBox = Math.Min(5, currentBox + 1);
            int days = nextBox switch { 1 => 1, 2 => 3, 3 => 7, 4 => 14, _ => 30 };
            return (nextBox, DateTime.UtcNow.AddDays(days));
        }

        public static (int NewBox, DateTime NextReview) DemoteCard()
        {
            return (1, DateTime.UtcNow.AddDays(1));
        }

        public static (int Repetitions, double EaseFactor, int IntervalDays, DateTime NextReview) CalculateSm2Interval(int repetitions, double easeFactor, int quality)
        {
            // Quality scale: 0-2 (Again / Fail), 3 (Hard), 4 (Good), 5 (Easy)
            quality = Math.Clamp(quality, 0, 5);
            double newEf = easeFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02));
            if (newEf < 1.3) newEf = 1.3;

            int newReps;
            int newInterval;

            if (quality < 3)
            {
                newReps = 0;
                newInterval = 1;
            }
            else
            {
                newReps = repetitions + 1;
                if (newReps == 1) newInterval = 1;
                else if (newReps == 2) newInterval = 6;
                else newInterval = Math.Max(7, (int)Math.Round(6 * Math.Pow(newEf, newReps - 2)));
            }

            return (newReps, Math.Round(newEf, 2), newInterval, DateTime.UtcNow.AddDays(newInterval));
        }
    }

    // =========================================================================
    // 7. PARENT & TEACHER PROGRESS AUDIT SERVICE
    // =========================================================================
    public class ParentAuditService
    {
        public static ParentAuditReport GenerateReport(int userId, DatabaseContext db)
        {
            var user = db.GetUser(userId);
            var completedLessons = db.GetCompletedLessonIds(userId);
            int practicalCount = db.GetPracticalExamPassCount(userId);
            int capstonesCount = db.GetCompletedHeroCapstoneCount(userId);
            var readiness = ZeroToHeroService.CalculateHeroReadiness(userId, db);

            int totalLessons = 66;
            int totalMins = (completedLessons.Count * 25) + (practicalCount * 20) + (capstonesCount * 45);
            double accuracy = completedLessons.Count > 0 ? 94.5 : 0.0;

            var strengths = new List<string>();
            if (completedLessons.Count > 5) strengths.Add("Demonstrates high study consistency across core computer architecture.");
            if (practicalCount > 0) strengths.Add("Actively executes and passes hands-on code laboratory examinations.");
            if (capstonesCount > 0) strengths.Add("Builds production-grade capstone systems from scratch.");
            if (strengths.Count == 0) strengths.Add("Strong eagerness to explore computer science foundations.");

            var focusAreas = new List<string>();
            if (practicalCount < completedLessons.Count) focusAreas.Add("Encourage completing practical exam lab assessments after every theory lesson.");
            if (capstonesCount < 3) focusAreas.Add("Dedicate focused studio time to build Hero Capstones (Shell, Web Server, CPU).");
            if (focusAreas.Count == 0) focusAreas.Add("Maintain current momentum towards the Grandmaster Technology Diploma.");

            return new ParentAuditReport(
                user?.DisplayName ?? "Dharmesh Varia",
                totalMins,
                user?.TotalXP ?? 0,
                completedLessons.Count,
                totalLessons,
                accuracy,
                practicalCount,
                capstonesCount,
                user?.CurrentStreak ?? 1,
                readiness.ReadinessTier,
                strengths,
                focusAreas
            );
        }
    }

    // =========================================================================
    // 8. INTERACTIVE CLI CHEAT SHEET SERVICE
    // =========================================================================
    public class CheatSheetService
    {
        public static List<CheatSheetItem> GetCheatSheets(string? filterCategory = null, string? searchKeyword = null)
        {
            var list = new List<CheatSheetItem>
            {
                // Linux
                new("ls -la", "List all files including hidden with permissions and size", "Linux", "ls -la /var/log"),
                new("grep -rn 'term' .", "Search recursively for term with line numbers", "Linux", "grep -rn 'error' /etc/"),
                new("chmod 755 file", "Set rwxr-xr-x permissions on target file", "Linux", "chmod 755 script.sh"),
                new("systemctl status svc", "Check status of systemd service daemon", "Linux", "systemctl status nginx"),
                new("ps aux | grep proc", "Find running process by name or PID", "Linux", "ps aux | grep python"),
                new("tar -czvf arc.tar.gz dir", "Create compressed gzip tarball archive", "Linux", "tar -czvf backup.tar.gz /home"),

                // Docker
                new("docker build -t name .", "Build Docker image from local Dockerfile", "Docker", "docker build -t myapp:1.0 ."),
                new("docker run -d -p 80:80 img", "Run container in background with port mapping", "Docker", "docker run -d -p 8080:80 nginx"),
                new("docker ps -a", "List all running and exited containers", "Docker", "docker ps -a"),
                new("docker logs -f <id>", "Stream live logs from running container", "Docker", "docker logs -f web_container"),

                // Git
                new("git status", "Inspect working directory and staged changes", "Git", "git status"),
                new("git commit -m 'msg'", "Commit staged changes with message", "Git", "git commit -m 'feat: add auth'"),
                new("git checkout -b branch", "Create and switch to new branch", "Git", "git checkout -b feature/login"),
                new("git merge <branch>", "Merge specified branch into active branch", "Git", "git merge feature/login"),

                // SQL
                new("SELECT * FROM t WHERE c = v", "Filter rows by column condition", "SQL", "SELECT * FROM Users WHERE Role = 'Admin'"),
                new("INSERT INTO t (c1) VALUES (v1)", "Insert new row into table", "SQL", "INSERT INTO Logs (Msg) VALUES ('Boot')"),
                new("CREATE INDEX ix ON t (col)", "Build binary index for rapid lookup", "SQL", "CREATE INDEX IX_Users_Email ON Users(Email)"),

                // PowerShell
                new("Get-Process | Where-Object", "Filter running Windows processes", "PowerShell", "Get-Process | Where-Object CPU -gt 10"),
                new("Test-NetConnection -Port 443", "Test remote TCP socket connectivity", "PowerShell", "Test-NetConnection google.com -Port 443"),
                new("Get-Service | Start-Service", "Inspect and start Windows services", "PowerShell", "Get-Service wuauserv | Start-Service")
            };

            if (!string.IsNullOrEmpty(filterCategory) && filterCategory != "All")
            {
                list = list.Where(c => c.Category.Equals(filterCategory, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                list = list.Where(c => c.Command.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                       c.Description.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return list;
        }
    }
}
