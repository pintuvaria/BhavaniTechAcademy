using System;
using System.Collections.Generic;
using System.IO;
using BhavaniTech.Core.Database;
using BhavaniTech.Core.Hardware;
using BhavaniTech.Core.Models;
using BhavaniTech.Core.Services;

namespace VerifyApp
{
    class Program
    {
        static int Main()
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("BHAVANI TECH ACADEMY — COMPREHENSIVE VERIFICATION SUITE (ALL PHASES)");
            Console.WriteLine("=================================================================\n");

            int failed = 0;

            // 1. Database Initialization & Seeding Test
            Console.WriteLine("[TEST 1] Initializing SQLite Database Layer...");
            string tempDb = Path.Combine(Path.GetTempPath(), $"bhavani_test_{Guid.NewGuid():N}.db");
            try
            {
                var db = new DatabaseContext(tempDb);
                var courses = db.GetCourses();
                Console.WriteLine($" -> Courses Seeded: {courses.Count} (Expected: 18)");
                if (courses.Count != 18) { Console.WriteLine("    FAILED: Expected 18 courses"); failed++; }

                var secLessons = db.GetLessonsForCourse("SEC101");
                Console.WriteLine($" -> SEC101 Lessons: {secLessons.Count} (Expected: 6)");
                if (secLessons.Count != 6) { Console.WriteLine("    FAILED: Expected 6 lessons in SEC101"); failed++; }

                int totalLessons = 0;
                int totalQuizzes = 0;
                foreach (var c in courses)
                {
                    var lessons = db.GetLessonsForCourse(c.Id);
                    totalLessons += lessons.Count;
                    foreach (var l in lessons)
                    {
                        var q = db.GetQuizForLesson(l.Id);
                        if (q != null) totalQuizzes++;
                    }
                }
                Console.WriteLine($" -> Total Curriculum Lessons: {totalLessons} (Expected: 108)");
                Console.WriteLine($" -> Total Interactive Quizzes: {totalQuizzes} (Expected: 108)");
                if (totalLessons != 108 || totalQuizzes != 108)
                {
                    Console.WriteLine("    FAILED: Curriculum lessons or quizzes count mismatch");
                    failed++;
                }

                int newXp = db.AddUserXp(50);
                if (newXp != 200) { Console.WriteLine("    FAILED: User XP mismatch"); failed++; }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(tempDb)) File.Delete(tempDb); } catch { }
            }

            // 2. Cryptography Lab (Phase 1)
            Console.WriteLine("\n[TEST 2] Cryptography Lab (RSA, Diffie-Hellman, AES-GCM)...");
            var rsa = CryptographyLabService.SimulateRsa(17, 19, 42);
            Console.WriteLine($" -> RSA: n={rsa.N}, phi={rsa.Phi}, e={rsa.E}, d={rsa.D}");
            if (rsa.N != 323 || rsa.D <= 0) { Console.WriteLine("    FAILED: RSA key derivation"); failed++; }
            else Console.WriteLine("    RSA Key Derivation PASSED ✅");

            var dh = CryptographyLabService.SimulateDiffieHellman(23, 5, 6, 15);
            Console.WriteLine($" -> Diffie-Hellman: Shared Secret Alice={dh.AliceSharedSecret}, Bob={dh.BobSharedSecret}");
            if (dh.AliceSharedSecret != dh.BobSharedSecret) { Console.WriteLine("    FAILED: DH shared secrets do not match"); failed++; }
            else Console.WriteLine("    Diffie-Hellman Key Exchange PASSED ✅");

            var aes = CryptographyLabService.SimulateAesGcm("Confidential Data", false);
            if (!aes.TagVerified || string.IsNullOrEmpty(aes.AuthTagHex)) { Console.WriteLine("    FAILED: AES-GCM authentic encryption"); failed++; }
            else Console.WriteLine("    AES-256-GCM AEAD Simulation PASSED ✅");

            // 3. Virtual Terminal & In-Memory Filesystem (Phase 1)
            Console.WriteLine("\n[TEST 3] Virtual Terminal Unix Filesystem & CTF Commands...");
            var term = new VirtualTerminalSession();
            var pwdRes = term.Execute("pwd");
            var lsRes = term.Execute("ls");
            var suRes = term.Execute("su root bhavani_root");
            var whoRes = term.Execute("whoami");
            Console.WriteLine($" -> pwd: {pwdRes.Output}, whoami: {whoRes.Output}");
            if (!whoRes.Output.Contains("root")) { Console.WriteLine("    FAILED: Virtual terminal root switch"); failed++; }
            else Console.WriteLine("    Virtual Terminal Filesystem PASSED ✅");

            // 4. Polyglot Sandbox & Algorithm Visualizer (Phase 2)
            Console.WriteLine("\n[TEST 4] Polyglot SQL, JS & Algorithm Visualizer...");
            var sqlRes = PolyglotExecutionService.ExecuteSql("CREATE TABLE users (id INT, name TEXT); INSERT INTO users VALUES (1, 'Alice'); SELECT * FROM users;");
            if (!sqlRes.Success || !sqlRes.FormattedTable.Contains("Alice")) { Console.WriteLine("    FAILED: In-Memory SQL execution"); failed++; }
            else Console.WriteLine("    In-Memory SQLite Sandbox PASSED ✅");

            var jsRes = PolyglotExecutionService.ExecuteJavaScript("let x = 10; let y = 25; console.log('Sum=' + (x + y)); return x + y;");
            if (!jsRes.Success || !jsRes.ReturnedValue.Contains("35")) { Console.WriteLine("    FAILED: JavaScript evaluator"); failed++; }
            else Console.WriteLine("    Lightweight JavaScript Engine PASSED ✅");

            var sortRes = AlgorithmVisualizationService.TraceQuickSort(new[] { 5, 2, 9, 1, 7 });
            if (sortRes.TotalComparisons <= 0 || !sortRes.FinalData.Contains("1, 2, 5, 7, 9")) { Console.WriteLine("    FAILED: QuickSort trace"); failed++; }
            else Console.WriteLine("    QuickSort Step-by-Step Visualizer PASSED ✅");

            // 5. Hardware, Microarchitecture & Electronics (Phase 3)
            Console.WriteLine("\n[TEST 5] Hardware Breadboard, Microcontroller & RISC Pipeline (Phase 3)...");
            var bbComps = new List<BreadboardComponent>
            {
                new() { Name = "Switch", Type = CircuitComponentType.Switch, IsActive = true },
                new() { Name = "R1", Type = CircuitComponentType.Resistor, Value = 300 },
                new() { Name = "LED", Type = CircuitComponentType.Led, Value = 2.0 }
            };
            var bbRes = BreadboardSimulationService.SimulateSeriesCircuit(5.0, bbComps);
            Console.WriteLine($" -> Breadboard Series Current: {bbRes.TotalCurrentAmps * 1000:F2} mA (Expected: 10.0 mA)");
            if (Math.Abs(bbRes.TotalCurrentAmps - 0.010) > 0.001) { Console.WriteLine("    FAILED: Breadboard Ohm's law loop calculation"); failed++; }
            else Console.WriteLine("    Breadboard Circuit Simulator PASSED ✅");

            var uC = MicrocontrollerStudioService.ExecuteSketch("servo", 128, 512);
            Console.WriteLine($" -> Microcontroller Servo Angle: {uC.ServoAngleDegrees}° (Expected: ~90.4°)");
            if (Math.Abs(uC.ServoAngleDegrees - 90.4) > 1.0) { Console.WriteLine("    FAILED: Microcontroller servo angle calculation"); failed++; }
            else Console.WriteLine("    Microcontroller Studio PASSED ✅");

            var pipe = CpuPipelineService.SimulateProgram("forwarding_raw", true);
            Console.WriteLine($" -> 5-Stage Pipeline Cycles: {pipe.TotalCycles}, Forwarding Events: {pipe.ForwardingEventsCount}");
            if (pipe.ForwardingEventsCount != 2 || pipe.StallsCount != 0) { Console.WriteLine("    FAILED: Pipeline RAW forwarding simulation"); failed++; }
            else Console.WriteLine("    5-Stage RISC Pipeline Hazard Simulator PASSED ✅");

            // 6. Frontier AI & Computer Vision Labs (Phase 4)
            Console.WriteLine("\n[TEST 6] Neural Net, Computer Vision & Offline Vector RAG (Phase 4)...");
            var nn = NeuralNetPlaygroundService.CreateNetwork("linear", "sigmoid", 3);
            Console.WriteLine($" -> Neural Net Accuracy: {nn.CurrentAccuracy}%, Loss: {nn.CurrentLoss:F4}, Grid points: {nn.DecisionBoundaryGrid.Count}");
            if (nn.DecisionBoundaryGrid.Count < 100) { Console.WriteLine("    FAILED: Neural net decision boundary generation"); failed++; }
            else Console.WriteLine("    Neural Network Playground PASSED ✅");

            var cv = ComputerVisionLabService.ApplyConvolution("sobel_horizontal", "box");
            Console.WriteLine($" -> CV Kernel: {cv.KernelName}, Pooled Size: {cv.PooledImage.GetLength(0)}x{cv.PooledImage.GetLength(1)}");
            if (cv.PooledImage.GetLength(0) != 4 || cv.PooledImage.GetLength(1) != 4) { Console.WriteLine("    FAILED: CV convolution/pooling"); failed++; }
            else Console.WriteLine("    Computer Vision 3x3 Convolution Lab PASSED ✅");

            var rag = VectorRagService.ExecuteRagQuery("How does RSA public key encryption work?", 2);
            Console.WriteLine($" -> RAG Top Match: '{rag.TopMatches[0].Document.Title}' [Cosine: {rag.TopMatches[0].CosineSimilarity:P1}]");
            if (!rag.TopMatches[0].Document.Title.Contains("RSA") || rag.TopMatches[0].CosineSimilarity < 0.4) { Console.WriteLine("    FAILED: Vector RAG retrieval"); failed++; }
            else Console.WriteLine("    Offline Vector Search & RAG PASSED ✅");

            // 7. Networking Topology, DNS & Kubernetes (Phase 5)
            Console.WriteLine("\n[TEST 7] Networking Topology, DNS Resolution & Kubernetes Scheduler (Phase 5)...");
            var netTrace = NetworkTopologyService.TracePacket("192.168.1.10", "10.0.0.50", "HTTP", false);
            Console.WriteLine($" -> Network Packet Hops: {netTrace.Hops.Count}, Success: {netTrace.IsSuccessful}");
            if (!netTrace.IsSuccessful || netTrace.Hops.Count < 5) { Console.WriteLine("    FAILED: Network multi-hop packet tracing"); failed++; }
            else Console.WriteLine("    Visual Network Topology Packet Tracer PASSED ✅");

            var dnsTrace = DnsResolutionService.ResolveDomain("learn.bhavanitech.org", false);
            Console.WriteLine($" -> DNS Resolved: {dnsTrace.ResolvedIp} across {dnsTrace.Steps.Count} hierarchical steps");
            if (dnsTrace.Steps.Count < 5 || string.IsNullOrEmpty(dnsTrace.ResolvedIp)) { Console.WriteLine("    FAILED: Hierarchical DNS resolution"); failed++; }
            else Console.WriteLine("    DNS Resolution Tracer PASSED ✅");

            var k8sDecision = K8sSchedulerService.SchedulePod(new K8sPod { Name = "test-pod", RequestCpuCores = 2, RequestMemoryGb = 4 });
            Console.WriteLine($" -> K8s Pod Scheduled: {k8sDecision.IsScheduled} to {k8sDecision.TargetNode}");
            if (!k8sDecision.IsScheduled || !k8sDecision.TargetNode.Contains("worker-pool")) { Console.WriteLine("    FAILED: Kubernetes pod scheduling"); failed++; }
            else Console.WriteLine("    Kubernetes Pod Scheduler PASSED ✅");

            // 8. Gamification, Certification & Portability (Phase 6)
            Console.WriteLine("\n[TEST 8] Certification Exam, Verifiable Diploma & Progress Portability (Phase 6)...");
            var examAnswers = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 2 }, { 4, 2 }, { 5, 2 }, { 6, 1 }, { 7, 2 }, { 8, 1 } };
            var examRes = CertificationExamService.GradeExam("Dharmesh Varia", examAnswers);
            Console.WriteLine($" -> Exam Score: {examRes.ScorePercent}%, Passed: {examRes.Passed}, Cert ID: {examRes.Certificate?.CertificateId}");
            if (!examRes.Passed || examRes.Certificate == null || string.IsNullOrEmpty(examRes.Certificate.VerificationHash)) { Console.WriteLine("    FAILED: Certification exam grading/diploma"); failed++; }
            else Console.WriteLine("    Comprehensive Certification Exam & Verifiable Diploma PASSED ✅");

            var sampleProfile = ProgressPortabilityService.CreateDefaultProfile("Dharmesh Varia");
            string exportedJson = ProgressPortabilityService.ExportProgressToJson(sampleProfile);
            var importedProfile = ProgressPortabilityService.ImportProgressFromJson(exportedJson);
            Console.WriteLine($" -> Progress Portability Roundtrip: Name '{importedProfile.StudentName}', XP {importedProfile.TotalXp}, Badges {importedProfile.UnlockedBadgeIds.Count}");
            if (importedProfile.StudentName != "Dharmesh Varia" || importedProfile.TotalXp != 1250) { Console.WriteLine("    FAILED: Progress portability JSON roundtrip"); failed++; }
            else Console.WriteLine("    Progress Portability JSON Export/Import PASSED ✅");

            // 9. Memory Footprint Validation (< 150 MB)
            Console.WriteLine("\n[TEST 9] Low-Hardware Memory Footprint Budget Test (< 150 MB)...");
            double ramMB = SystemDiagnostics.GetCurrentMemoryUsageMB();
            Console.WriteLine($" -> Current In-Memory Working Set: {ramMB:F1} MB (Budget Limit: < 150.0 MB)");
            if (ramMB > 150)
            {
                Console.WriteLine("    WARNING: RAM footprint exceeded 150 MB");
                failed++;
            }
            else
            {
                Console.WriteLine("    Low-Hardware RAM Budget (<150 MB) PASSED ✅");
            }

            // 10. Course Details & Blueprint Verification (Prerequisites, Toolchain, Setup)
            Console.WriteLine("\n[TEST 10] Course Details & Filter Search Engine...");
            var expectedCourseIds = new[] { "CS101", "PROG101", "SWE101", "HW101", "NET101", "SEC101", "AI101", "ELEC101", "LNX101", "TS101", "MAST101" };
            int detailsCount = 0;
            foreach (var cid in expectedCourseIds)
            {
                var det = CourseDetailsProvider.GetDetails(cid);
                if (!string.IsNullOrEmpty(det.Prerequisites) &&
                    !string.IsNullOrEmpty(det.InstallationSteps) &&
                    !string.IsNullOrEmpty(det.EnvironmentVerification))
                {
                    detailsCount++;
                }
            }
            Console.WriteLine($" -> Course Details & Setup Blueprints Verified: {detailsCount}/{expectedCourseIds.Length}");
            if (detailsCount != expectedCourseIds.Length)
            {
                Console.WriteLine("    FAILED: Incomplete course details blueprints");
                failed++;
            }
            else
            {
                Console.WriteLine("    Course Details & Setup Blueprints PASSED ✅");
            }

            // 11. Multi-Language Expansion (10 Languages with Logos) & Project Studio (Calculator & Web)
            Console.WriteLine("\n[TEST 11] Multi-Language Suite (10 Languages) & Project Studio Engines...");
            var allLangs = LanguageRegistryService.GetAllLanguages();
            Console.WriteLine($" -> Registered Languages: {allLangs.Count} (Expected: 10)");
            if (allLangs.Count != 10)
            {
                Console.WriteLine("    FAILED: Expected 10 registered languages");
                failed++;
            }

            int executedLangs = 0;
            foreach (var lang in allLangs)
            {
                if (string.IsNullOrEmpty(lang.LogoEmoji) || string.IsNullOrEmpty(lang.StarterTemplateCode))
                {
                    Console.WriteLine($"    FAILED: Missing logo or template for {lang.DisplayName}");
                    failed++;
                }

                var execRes = CodeExecutionService.ExecuteCode(lang.Id, lang.StarterTemplateCode);
                if (execRes.Success && !string.IsNullOrEmpty(execRes.Output))
                {
                    executedLangs++;
                }
            }
            Console.WriteLine($" -> Code Execution Across 10 Languages: {executedLangs}/10 Succeeded");
            if (executedLangs != 10)
            {
                Console.WriteLine("    FAILED: Not all 10 languages executed successfully");
                failed++;
            }

            // Calculator Engine Verification
            var calc = new CalculatorEngine();
            var cRes1 = calc.Evaluate("15 + 25 * 2");
            Console.WriteLine($" -> Calculator Order of Operations: '15 + 25 * 2' = {cRes1.FormattedResult} (Expected: 65)");
            if (cRes1.Value != 65)
            {
                Console.WriteLine("    FAILED: Operator precedence calculation in CalculatorEngine");
                failed++;
            }

            double sqrtVal = CalculatorEngine.CalculateSquareRoot(144);
            double sqrVal = CalculatorEngine.CalculateSquare(12);
            Console.WriteLine($" -> Scientific Math: sqrt(144)={sqrtVal}, sqr(12)={sqrVal}");
            if (sqrtVal != 12 || sqrVal != 144)
            {
                Console.WriteLine("    FAILED: Scientific math operations");
                failed++;
            }
            else
            // 12. Student Learning Lifecycle & Compounding Progress
            Console.WriteLine("\n[TEST 12] Student Learning Lifecycle (Open App -> Learn -> Close -> Reopen Continuity)...");
            string lifecycleDb = Path.Combine(Path.GetTempPath(), $"bhavani_lifecycle_{Guid.NewGuid():N}.db");
            try
            {
                // Session 1: Student opens app for first time
                var dbSession1 = new DatabaseContext(lifecycleDb);
                var initialCompleted = dbSession1.GetCompletedLessonIds(1);
                Console.WriteLine($" -> Initial Completed Lessons: {initialCompleted.Count} (Expected: 0)");
                if (initialCompleted.Count != 0) { Console.WriteLine("    FAILED: Expected 0 completed lessons on fresh start"); failed++; }

                var (recCourse1, recLesson1) = dbSession1.GetNextRecommendedLesson(1);
                Console.WriteLine($" -> Recommended Starting Point: [{recCourse1?.Id}] {recLesson1?.Title}");
                if (recCourse1 == null || recLesson1 == null) { Console.WriteLine("    FAILED: No initial recommendation returned"); failed++; }

                // Student completes Lesson 1 and earns XP
                dbSession1.MarkLessonCompleted(recLesson1!.Id, 1);
                var afterL1 = dbSession1.GetCompletedLessonIds(1);
                if (afterL1.Count != 1 || !afterL1.Contains(recLesson1.Id))
                {
                    Console.WriteLine("    FAILED: MarkLessonCompleted did not persist");
                    failed++;
                }

                // Next recommendation advances to Lesson 2
                var (recCourse2, recLesson2) = dbSession1.GetNextRecommendedLesson(1);
                Console.WriteLine($" -> Next Milestone After Lesson 1: [{recCourse2?.Id}] {recLesson2?.Title}");
                if (recLesson2?.Id == recLesson1.Id)
                {
                    Console.WriteLine("    FAILED: Recommendation did not advance past completed lesson");
                    failed++;
                }

                // STUDENT CLOSES APPLICATION (dbSession1 disposed / closed)
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

                // NEXT DAY: STUDENT REOPENS APPLICATION (dbSession2 connects to same database)
                var dbSession2 = new DatabaseContext(lifecycleDb);
                var resumedCompleted = dbSession2.GetCompletedLessonIds(1);
                var (resumedCourse, resumedLesson) = dbSession2.GetNextRecommendedLesson(1);
                Console.WriteLine($" -> Resumed Session Completed Count: {resumedCompleted.Count} (Expected: 1)");
                Console.WriteLine($" -> Resumed Session Continues At: [{resumedCourse?.Id}] {resumedLesson?.Title}");
                if (resumedCompleted.Count != 1 || resumedLesson?.Id != recLesson2?.Id)
                {
                    Console.WriteLine("    FAILED: Continuous compounding session resumption failed");
                    failed++;
                }
                else
                {
                    Console.WriteLine("    Student Compounding Progress & Next-Session Resume PASSED ✅");
                }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(lifecycleDb)) File.Delete(lifecycleDb); } catch { }
            }

            // 13. Practical Examinations for All 108 Lessons
            Console.WriteLine("\n[TEST 13] Practical Examination System for All 108 Lessons...");
            var allExams = PracticalExamService.GetAllExams();
            Console.WriteLine($" -> Total Practical Exams Defined: {allExams.Count} (Expected: 108)");
            if (allExams.Count != 108)
            {
                Console.WriteLine($"    FAILED: Expected 108 exams, found {allExams.Count}");
                failed++;
            }
            else
            {
                Console.WriteLine("    108 Practical Exams Catalog Complete ✅");
            }

            // Test multi-paradigm grading
            var examL1Def = allExams.First(e => e.LessonId == "L_CS_1");
            var evalCode = PracticalExamService.Evaluate("L_CS_1", examL1Def.StarterCode);
            if (!evalCode.Passed || evalCode.Score != 100)
            {
                Console.WriteLine($"    FAILED: L_CS_1 CodeExecution exam grader failed: {evalCode.Summary}");
                failed++;
            }
            else
            {
                Console.WriteLine("    CodeExecution Grader PASSED ✅");
            }

            var evalCmd = PracticalExamService.Evaluate("L_LNX_1", "cd /var/log\ntail -n 20 auth.log");
            if (!evalCmd.Passed || evalCmd.Score != 100)
            {
                Console.WriteLine($"    FAILED: L_LNX_1 CommandSimulation exam grader failed: {evalCmd.Summary}");
                failed++;
            }
            else
            {
                Console.WriteLine("    CommandSimulation Grader PASSED ✅");
            }

            var evalCmd2 = PracticalExamService.Evaluate("L_SEC_2", "nmap -sS -Pn 192.168.1.100");
            if (!evalCmd2.Passed)
            {
                Console.WriteLine($"    FAILED: L_SEC_2 CommandSimulation exam grader failed: {evalCmd2.Summary}");
                failed++;
            }
            else
            {
                Console.WriteLine("    Security SYN Port Scanner Grader PASSED ✅");
            }

            // Test Practical Exam Database Seeding & Student Progress Persistence
            string examTestDb = Path.Combine(Path.GetTempPath(), $"bhavani_exam_test_{Guid.NewGuid():N}.db");
            try
            {
                var dbExam = new DatabaseContext(examTestDb);
                var examL1 = dbExam.GetPracticalExamForLesson("L_CS_1");
                if (examL1 == null || examL1.LessonId != "L_CS_1")
                {
                    Console.WriteLine("    FAILED: GetPracticalExamForLesson did not return seeded exam");
                    failed++;
                }
                else
                {
                    dbExam.SavePracticalExamProgress(1, "L_CS_1", 100, 50);
                    int passedCount = dbExam.GetPracticalExamPassCount(1);
                    if (passedCount != 1)
                    {
                        Console.WriteLine($"    FAILED: Practical exam pass count expected 1, got {passedCount}");
                        failed++;
                    }
                    else
                    {
                        Console.WriteLine("    Practical Exam DB Seeding & Progress Persistence PASSED ✅");
                    }
                }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(examTestDb)) File.Delete(examTestDb); } catch { }
            }

            // 14. Learn with Gaming Arcade Engines
            Console.WriteLine("\n[TEST 14] Learn with Gaming Arcade Engines (5 Games)...");

            // Game 1: ByteBot Code Maze Runner
            var maze = ArcadeGameEngine.GetMazeLevel(1);
            var (mazeRes, mazeLogs) = ArcadeGameEngine.RunBotCommands(maze, "FORWARD 3");
            Console.WriteLine($" -> Maze Runner Level 1 Execution: Completed={mazeRes.Completed}, Gems={mazeRes.GemsCollected}");
            if (!mazeRes.Completed || mazeRes.GemsCollected != 2)
            {
                Console.WriteLine("    FAILED: ByteBot Maze Runner simulation failed");
                failed++;
            }
            else
            {
                Console.WriteLine("    ByteBot Code Maze Runner Engine PASSED ✅");
            }

            // Game 2: Cyber Defense Firewall Packet Sentry
            var packets = ArcadeGameEngine.GeneratePacketWave(1);
            if (packets.Count != 6)
            {
                Console.WriteLine($"    FAILED: Expected 6 packets in wave, got {packets.Count}");
                failed++;
            }
            var evilPacket = new NetworkPacket(99, "185.220.101.5", "192.168.1.1", 4444, "TCP", "Reverse TCP Shell", true, "Reverse Shell");
            var (fwScore, fwHealth, fwMsg) = ArcadeGameEngine.EvaluateFirewallAction(evilPacket, true);
            if (fwScore <= 0 || fwHealth != 0)
            {
                Console.WriteLine("    FAILED: Firewall packet drop scoring failed");
                failed++;
            }
            else
            {
                Console.WriteLine("    Cyber Defense Firewall Packet Sentry PASSED ✅");
            }

            // Game 3: Circuit Logic Reactor
            var (reactorOnline, out1, out2, diag) = ArcadeGameEngine.EvaluateReactorLogic(true, true, true, false, "AND", "OR", "AND");
            if (!reactorOnline || !out1 || !out2)
            {
                Console.WriteLine("    FAILED: Circuit logic reactor failed to activate");
                failed++;
            }
            else
            {
                Console.WriteLine("    Circuit Logic Reactor Engine PASSED ✅");
            }

            // Game 4: SQL Dungeon Quest
            var (sqlSuccess, sqlMsg, sqlGrid) = ArcadeGameEngine.ExecuteDungeonQuery("SELECT * FROM Monsters WHERE Element = 'FIRE'", 1);
            if (!sqlSuccess)
            {
                Console.WriteLine($"    FAILED: SQL Dungeon Quest Query Failed: {sqlMsg}");
                failed++;
            }
            else
            {
                Console.WriteLine("    SQL Dungeon Quest In-Memory SQLite Engine PASSED ✅");
            }

            // Game 5: Binary Blitz Bit Shifter
            var bits = new bool[] { true, false, false, false, false, false, false, true }; // 128 + 1 = 129
            int byteVal = ArcadeGameEngine.CalculateByteFromBits(bits);
            if (byteVal != 129)
            {
                Console.WriteLine($"    FAILED: Binary Blitz bit calculation expected 129, got {byteVal}");
                failed++;
            }
            else
            {
                Console.WriteLine("    Binary Blitz Bit Shifter PASSED ✅");
            }

            // 15. Zero-to-Hero 6-Stage Tech Roadmap & Placement Diagnostic Engine
            Console.WriteLine("\n[TEST 15] Zero-to-Hero 6-Stage Roadmap & Diagnostic Engine...");
            var stages = ZeroToHeroService.GetStageMilestones();
            Console.WriteLine($" -> Roadmap Stages Defined: {stages.Count} (Expected: 6)");
            if (stages.Count != 6) { Console.WriteLine("    FAILED: Expected 6 stages in ZeroToHero roadmap"); failed++; }
            else Console.WriteLine("    6-Stage Complete Technology Roadmap PASSED ✅");

            var diagQuestions = ZeroToHeroService.GetDiagnosticQuestions();
            Console.WriteLine($" -> Diagnostic Questions: {diagQuestions.Count} (Expected: 10)");
            if (diagQuestions.Count != 10) { Console.WriteLine("    FAILED: Expected 10 diagnostic questions"); failed++; }

            // Test low-score diagnosis -> GroundZero
            var diagLow = ZeroToHeroService.EvaluateDiagnostic(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            if (diagLow.RecommendedTier != HeroStageTier.Stage0_GroundZero) { Console.WriteLine("    FAILED: Low diagnostic score must map to Ground Zero"); failed++; }

            // Test high-score diagnosis -> DevOps / Frontier
            var diagHigh = ZeroToHeroService.EvaluateDiagnostic(new int[] { 1, 2, 1, 1, 1, 1, 1, 1, 0, 1 });
            if (diagHigh.RecommendedTier != HeroStageTier.Stage4_DevOpsCloudSentinel) { Console.WriteLine("    FAILED: High diagnostic score mapping"); failed++; }
            else Console.WriteLine("    Adaptive Placement Diagnostic Engine PASSED ✅");

            // 16. 6 Build-from-Scratch Hero Capstone Systems & Multi-Assertion Verifiers
            Console.WriteLine("\n[TEST 16] 6 Build-from-Scratch Hero Capstone Systems & Verifiers...");
            var allCaps = ZeroToHeroService.GetAllHeroCapstones();
            Console.WriteLine($" -> Total Hero Capstones Defined: {allCaps.Count} (Expected: 6)");
            if (allCaps.Count != 6) { Console.WriteLine("    FAILED: Expected 6 capstone projects"); failed++; }

            // Verify each capstone with starter code
            foreach (var cap in allCaps)
            {
                var capRes = ZeroToHeroService.EvaluateCapstone(cap.Id, cap.StarterCode);
                if (!capRes.Passed || capRes.Score != 100)
                {
                    Console.WriteLine($"    FAILED: Capstone [{cap.Id}] verification failed: {capRes.Summary}");
                    failed++;
                }
                else
                {
                    Console.WriteLine($"    Capstone [{cap.Id}] {cap.TechPillar} Verified PASSED ✅");
                }
            }

            // 17. Hero Readiness Radar & Database Persistence
            Console.WriteLine("\n[TEST 17] Hero Readiness Radar & Database Roundtrip...");
            string z2hDb = Path.Combine(Path.GetTempPath(), $"bhavani_z2h_test_{Guid.NewGuid():N}.db");
            try
            {
                var dbZ = new DatabaseContext(z2hDb);
                var capsInDb = dbZ.GetHeroCapstones();
                if (capsInDb.Count != 6) { Console.WriteLine($"    FAILED: Expected 6 capstones seeded in DB, found {capsInDb.Count}"); failed++; }

                // Save capstone completion
                dbZ.SaveHeroCapstoneSubmission(1, "HERO_CAP_1", 100, 200, "// shell code");
                int completedCapCount = dbZ.GetCompletedHeroCapstoneCount(1);
                if (completedCapCount != 1) { Console.WriteLine($"    FAILED: Completed capstone count expected 1, got {completedCapCount}"); failed++; }

                // Calculate Hero Readiness Report
                var report = ZeroToHeroService.CalculateHeroReadiness(1, dbZ);
                Console.WriteLine($" -> Hero Readiness Score: {report.OverallPercentage}%, Tier: {report.ReadinessTier}");
                if (report.OverallPercentage < 0 || report.OverallPercentage > 100 || string.IsNullOrEmpty(report.ReadinessTier))
                {
                    Console.WriteLine("    FAILED: Hero readiness report metrics invalid");
                    failed++;
                }
                else
                {
                    Console.WriteLine("    Hero Readiness 6-Pillar Radar & Portfolio PASSED ✅");
                }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(z2hDb)) File.Delete(z2hDb); } catch { }
            }

            // 18. Visual Git Simulator
            Console.WriteLine("\n[TEST 18] Visual Git Repository & DAG Simulator...");
            var git = new VisualGitSimulator();
            var c1 = git.Commit("feat: initial scaffolding");
            bool bCreated = git.CreateBranch("feature/auth");
            bool bSwitched = git.Checkout("feature/auth");
            var c2 = git.Commit("feat: implement token auth");
            var mergeRes = git.Merge("main");
            Console.WriteLine($" -> Git Branches: {git.GetBranches().Count}, Commits: {git.GetCommitGraph().Count}, Merge: {mergeRes.Success}");
            if (!bCreated || !bSwitched || git.GetCommitGraph().Count < 3 || !mergeRes.Success)
            {
                Console.WriteLine("    FAILED: Git Simulator branching or merge failed");
                failed++;
            }
            else
            {
                Console.WriteLine("    Visual Git Repository Simulator PASSED ✅");
            }

            // 19. Code Step-Debugger
            Console.WriteLine("\n[TEST 19] Code Step-Debugger (Execution Trace & Frame Variables)...");
            var fibFrames = CodeStepDebuggerService.TraceExecution("fibonacci");
            var stackFrames = CodeStepDebuggerService.TraceExecution("callstack");
            Console.WriteLine($" -> Fibonacci Trace Frames: {fibFrames.Count}, Callstack Trace Frames: {stackFrames.Count}");
            if (fibFrames.Count < 5 || stackFrames.Count < 5 || !fibFrames[2].Variables.ContainsKey("temp"))
            {
                Console.WriteLine("    FAILED: Debugger execution tracing failed");
                failed++;
            }
            else
            {
                Console.WriteLine("    Code Step-Debugger Service PASSED ✅");
            }

            // 20. Offline REST API Client & Regex Lab
            Console.WriteLine("\n[TEST 20] Offline REST API Client & Regex Studio...");
            var restReq = new RestApiRequest("GET", "/api/v1/students", new Dictionary<string, string>(), "");
            var restRes = OfflineRestApiClientService.SendRequest(restReq);
            Console.WriteLine($" -> REST Response: {restRes.StatusCode} {restRes.StatusText} in {restRes.LatencyMs}ms");

            var rxRes = RegexLabService.Evaluate(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", "test@bhavanitech.org");
            Console.WriteLine($" -> Regex Evaluation: MatchCount={rxRes.MatchCount}, IsMatch={rxRes.IsMatch}");
            if (restRes.StatusCode != 200 || !rxRes.IsMatch || rxRes.MatchCount != 1)
            {
                Console.WriteLine("    FAILED: REST API Client or Regex Lab failed");
                failed++;
            }
            else
            {
                Console.WriteLine("    Offline REST API & Regex Studio PASSED ✅");
            }

            // 21. Arcade Games: Assembly Bot, Webcraft CSS, CTF Ethical Hacking Arena
            Console.WriteLine("\n[TEST 21] Arcade Games: Assembly Bot, Webcraft CSS & CTF Ethical Hacking...");
            var asmRes = ArcadeGameEngine.ExecuteAssemblyBot("MOV RAX, 50\nMOV RBX, 50\nADD RAX, RBX\nFIRE", 100);
            Console.WriteLine($" -> Assembly Bot Victory: {asmRes.Victory}, Drone HP: {asmRes.DroneHpRemaining}");

            var webRes = ArcadeGameEngine.EvaluateWebcraftCss("display: flex; justify-content: center; align-items: center;", 1);
            Console.WriteLine($" -> Webcraft CSS Rescuer: {webRes.Success} - {webRes.Feedback}");

            var ctfRes = ArcadeGameEngine.EvaluateCtfFlag(1, "BHAVANI_CTF_SECURE");
            Console.WriteLine($" -> CTF Flag 1 Captured: {ctfRes.Correct} - {ctfRes.Feedback}");

            if (!asmRes.Victory || !webRes.Success || !ctfRes.Correct)
            {
                Console.WriteLine("    FAILED: Arcade Games execution verification failed");
                failed++;
            }
            else
            {
                Console.WriteLine("    Arcade Games Suite PASSED ✅");
            }

            // 22. Spaced Repetition Flashcards & Leitner 5-Box Engine
            Console.WriteLine("\n[TEST 22] Spaced Repetition Flashcards (Leitner 5-Box)...");
            string srsDb = Path.Combine(Path.GetTempPath(), $"bhavani_srs_test_{Guid.NewGuid():N}.db");
            try
            {
                var dbSrs = new DatabaseContext(srsDb);
                var cards = dbSrs.GetFlashcardsForReview(1);
                Console.WriteLine($" -> Seeded Flashcards in DB: {cards.Count} (Expected >= 15)");
                var (newBox, nextDate) = SpacedRepetitionService.PromoteCard(1);
                dbSrs.UpdateFlashcardProgress(1, cards[0].Id, newBox, nextDate.ToString("o"));
                var updatedCards = dbSrs.GetFlashcardsForReview(1);
                if (cards.Count < 15 || updatedCards[0].BoxLevel != 2)
                {
                    Console.WriteLine("    FAILED: Flashcards SRS engine verification failed");
                    failed++;
                }
                else
                {
                    Console.WriteLine("    Spaced Repetition Flashcards PASSED ✅");
                }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(srsDb)) File.Delete(srsDb); } catch { }
            }

            // 23. Parent & Teacher Progress Audit & Database Snapshot Backup Engine
            Console.WriteLine("\n[TEST 23] Parent & Teacher Progress Audit & Database Snapshot Backup...");
            string auditDb = Path.Combine(Path.GetTempPath(), $"bhavani_audit_test_{Guid.NewGuid():N}.db");
            string backupDest = Path.Combine(Path.GetTempPath(), $"bhavani_backup_test_{Guid.NewGuid():N}.db");
            try
            {
                var dbAudit = new DatabaseContext(auditDb);
                var auditReport = ParentAuditService.GenerateReport(1, dbAudit);
                Console.WriteLine($" -> Audit Report for: {auditReport.StudentName}, Study Mins: {auditReport.TotalStudyMinutes}, Strengths: {auditReport.Strengths.Count}");

                bool backupOk = dbAudit.BackupDatabaseToFile(backupDest);
                bool backupExists = File.Exists(backupDest) && new FileInfo(backupDest).Length > 0;
                Console.WriteLine($" -> SQLite Snapshot Backup: Status={backupOk}, FileExists={backupExists}");

                if (string.IsNullOrEmpty(auditReport.StudentName) || !backupOk || !backupExists)
                {
                    Console.WriteLine("    FAILED: Parent Audit or Database Snapshot Backup failed");
                    failed++;
                }
                else
                {
                    Console.WriteLine("    Parent Audit & Database Snapshot Backup PASSED ✅");
                }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(auditDb)) File.Delete(auditDb); } catch { }
                try { if (File.Exists(backupDest)) File.Delete(backupDest); } catch { }
            }

            // 24. Advanced Learnings: Phase 7 OS, Distributed, AI, Quantum
            Console.WriteLine("\n[TEST 24] Advanced Learnings (Phase 7 OS, Distributed, AI, Quantum)...");
            var osSvc = new OsSimulationService();
            var lru = osSvc.SimulatePagingLRU(new[] { 1, 2, 3, 4, 1, 2, 5, 1, 2, 3, 4, 5 }, 3);
            Console.WriteLine($" -> Paging LRU: Faults={lru.PageFaults}, Hits={lru.PageHits}");
            
            var dsSvc = new DistributedSystemsService();
            var raft = dsSvc.SimulateLeaderElection(new List<DistributedSystemsService.RaftNode> { 
                new DistributedSystemsService.RaftNode { Id = "N1" }, 
                new DistributedSystemsService.RaftNode { Id = "N2" }, 
                new DistributedSystemsService.RaftNode { Id = "N3" } 
            }, "N1");
            Console.WriteLine($" -> Raft Leader Election: New Leader={raft.NewLeader}, Term={raft.Term}");
            
            var trSvc = new TransformerVisualizerService();
            double[,] qMat = { { 1, 0 }, { 0, 1 } };
            double[,] kMat = { { 1, 0 }, { 0, 1 } };
            double[,] vMat = { { 1, 0 }, { 0, 1 } };
            var attn = trSvc.CalculateSelfAttention(qMat, kMat, vMat);
            Console.WriteLine($" -> Transformer Attention: Scores[{attn.SoftmaxScores.GetLength(0)}x{attn.SoftmaxScores.GetLength(1)}]");
            
            var qcSvc = new QuantumComputingService();
            var qb = new QuantumComputingService.Qubit(new QuantumComputingService.ComplexNumber(1, 0), new QuantumComputingService.ComplexNumber(0, 0));
            var hGate = qcSvc.GetHadamardGate();
            var qbH = qcSvc.ApplyGate(qb, hGate);
            Console.WriteLine($" -> Quantum Hadamard Gate: {qbH}");

            if (lru.PageFaults == 0 || raft.NewLeader != "N1" || attn.SoftmaxScores.Length == 0 || qbH.ProbabilityZero < 0.49)
            {
                Console.WriteLine("    FAILED: Phase 7 Advanced Learnings");
                failed++;
            }
            else
            {
                Console.WriteLine("    Phase 7 Advanced Learnings PASSED ✅");
            }

            // 25. Phase 8: Gamification & Omni-Domain Improvements
            Console.WriteLine("\n[TEST 25] Phase 8 Omni-Domain (Gamification Streaks & Survival)...");
            var gamification = new GamificationService();
            var streak = gamification.CalculateStreak(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, 3, 3);
            Console.WriteLine($" -> Streak Calc: Current={streak.CurrentStreak}, Multiplier={streak.XpMultiplier}x");

            var survival = gamification.GenerateSurvivalRound(1);
            Console.WriteLine($" -> Endless Survival: Generated {survival.Count} questions.");
            var hint = gamification.GenerateDynamicHint("print('hello')", "print('hello');");
            Console.WriteLine($" -> Dynamic Hint Engine: {hint}");

            if (streak.CurrentStreak != 4 || survival.Count < 5 || !hint.Contains("semicolon"))
            {
                Console.WriteLine("    FAILED: Gamification engine logic");
                failed++;
            }
            else
            {
                Console.WriteLine("    Phase 8 Gamification & Omni-Domain PASSED ✅");
            }

            // 26. Phase 9: Professional Grey Hat & Advanced Offensive Security
            Console.WriteLine("\n[TEST 26] Phase 9 Grey Hat (Web Sec, PCAP, Malware Sandbox)...");
            
            var webSec = new WebSecuritySimulatorService();
            var sqli = webSec.SimulateLogin("admin", "' OR '1'='1", false);
            Console.WriteLine($" -> SQLi Sandbox: Bypassed={sqli.IsBypassed}, Reason: {sqli.Explanation}");

            var forensics = new NetworkForensicsService();
            var pcap = forensics.AnalyzeSimulatedPcap("FTP", "USER admin\r\nPASS secret123\r\n");
            Console.WriteLine($" -> PCAP Analyzer: Found Auth={pcap.FoundCleartextAuth}, Bytes: {pcap.HexDump.Length}");

            var malware = new MalwareAnalysisSandboxService();
            var buffer = malware.SimulateBufferOverflow("AAAAAAAAAAAAAAAAAAAAAA", 12);
            Console.WriteLine($" -> Stack Visualizer: EIP Overwritten={buffer.EipOverwritten}, Written={buffer.BytesWritten}");

            if (!sqli.IsBypassed || !pcap.FoundCleartextAuth || !buffer.EipOverwritten)
            {
                Console.WriteLine("    FAILED: Phase 9 Grey Hat Security Labs");
                failed++;
            }
            else
            {
                Console.WriteLine("    Phase 9 Grey Hat Security Labs PASSED ✅");
            }

            // 27. OS Mastery, Registry & GPEDIT Curriculum
            Console.WriteLine("\n[TEST 27] OS Mastery (Registry, GPEDIT, OS Types)...");
            var osCurriculum = new WindowsInternalsService().GetOsCurriculum();
            Console.WriteLine($" -> OS Curriculum Loaded: {osCurriculum.Count} advanced lessons.");
            foreach (var lesson in osCurriculum)
            {
                Console.WriteLine($"    - {lesson.Title} (Precautions length: {lesson.Precautions.Length})");
            }

            if (osCurriculum.Count < 3)
            {
                Console.WriteLine("    FAILED: OS Curriculum");
                failed++;
            }
            else
            {
                Console.WriteLine("    OS Mastery Curriculum PASSED ✅");
            }

            // 28. CCNA Complete Tutorials & Exams
            Console.WriteLine("\n[TEST 28] CCNA Certification Mastery (Tutorials & Exams)...");
            var ccna = new CcnaCurriculumService();
            var tutorials = ccna.GetTutorials();
            Console.WriteLine($" -> CCNA Modules Loaded: {tutorials.Count}");
            
            // Simulate a perfect exam run
            var answers = new List<int> { 2, 2, 2, 1, 2 }; 
            var grade = ccna.GradeExam(answers);
            Console.WriteLine($" -> CCNA Final Exam: Scored {grade.Score}/{grade.TotalQuestions} ({grade.Percentage}%) - Passed: {grade.Passed}");

            if (tutorials.Count < 6 || !grade.Passed)
            {
                Console.WriteLine("    FAILED: CCNA Curriculum & Exams");
                failed++;
            }
            else
            {
                Console.WriteLine("    CCNA Mastery Curriculum PASSED ✅");
            }

            Console.WriteLine("\n=================================================================");
            if (failed == 0)
            {
                Console.WriteLine("ALL 28 COMPREHENSIVE TEST SUITES PASSED PERFECTLY! (0 Failures) ✅");
                Console.WriteLine("=================================================================");
                return 0;
            }
            else
            {
                Console.WriteLine($"TEST SUITE FAILED: {failed} errors detected ❌");
                Console.WriteLine("=================================================================");
                return 1;
            }
        }
    }
}
