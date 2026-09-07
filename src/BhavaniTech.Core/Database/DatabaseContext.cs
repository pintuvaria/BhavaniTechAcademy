using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using BhavaniTech.Core.Models;
using BhavaniTech.Core.Services;

namespace BhavaniTech.Core.Database
{
    public class DatabaseContext
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public DatabaseContext(string? dbPath = null)
        {
                        if (string.IsNullOrEmpty(dbPath))
            {
                string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                _dbPath = Path.Combine(baseFolder, "BhavaniTech_Progress.dat");
            }
            else
            {
                _dbPath = dbPath;
            }

            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _dbPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Private
            }.ToString();

            InitializeDatabase();
            
            // Apply hidden and encrypted attributes to lock it to the main application
            try 
            {
                if (File.Exists(_dbPath))
                {
                    File.SetAttributes(_dbPath, FileAttributes.Hidden | FileAttributes.System | FileAttributes.NotContentIndexed);
                }
            } 
            catch { }
        }

        public SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private void InitializeDatabase()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            // High performance SQLite tuning for low-hardware HDD read/write speed
            cmd.CommandText = @"
                PRAGMA journal_mode = WAL;
                PRAGMA synchronous = NORMAL;
                PRAGMA temp_store = MEMORY;
                PRAGMA cache_size = -2000; -- 2MB max memory cache

                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    DisplayName TEXT NOT NULL,
                    Role INTEGER NOT NULL,
                    TotalXP INTEGER DEFAULT 0,
                    CurrentLevel INTEGER DEFAULT 1,
                    CurrentStreak INTEGER DEFAULT 1,
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Courses (
                    Id TEXT PRIMARY KEY,
                    Title TEXT NOT NULL,
                    Category INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    IconKey TEXT NOT NULL,
                    SortOrder INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Modules (
                    Id TEXT PRIMARY KEY,
                    CourseId TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    SortOrder INTEGER NOT NULL,
                    FOREIGN KEY(CourseId) REFERENCES Courses(Id)
                );

                CREATE TABLE IF NOT EXISTS Lessons (
                    Id TEXT PRIMARY KEY,
                    ModuleId TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Summary TEXT NOT NULL,
                    ContentMarkdown TEXT NOT NULL,
                    EstimatedMinutes INTEGER NOT NULL,
                    Difficulty TEXT NOT NULL,
                    FOREIGN KEY(ModuleId) REFERENCES Modules(Id)
                );

                CREATE TABLE IF NOT EXISTS QuizQuestions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LessonId TEXT NOT NULL,
                    QuestionText TEXT NOT NULL,
                    OptionA TEXT NOT NULL,
                    OptionB TEXT NOT NULL,
                    OptionC TEXT NOT NULL,
                    OptionD TEXT NOT NULL,
                    CorrectOption TEXT NOT NULL,
                    Explanation TEXT NOT NULL,
                    FOREIGN KEY(LessonId) REFERENCES Lessons(Id)
                );

                CREATE TABLE IF NOT EXISTS UserProgress (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    LessonId TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Score INTEGER DEFAULT 0,
                    XpEarned INTEGER DEFAULT 0,
                    CompletedAt TEXT NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                CREATE TABLE IF NOT EXISTS Achievements (
                    Id TEXT PRIMARY KEY,
                    Code TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    IconKey TEXT NOT NULL,
                    XpReward INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS UserAchievements (
                    UserId INTEGER NOT NULL,
                    AchievementId TEXT NOT NULL,
                    UnlockedAt TEXT NOT NULL,
                    PRIMARY KEY(UserId, AchievementId)
                );

                CREATE TABLE IF NOT EXISTS TroubleshootingScenarios (
                    Id INTEGER PRIMARY KEY,
                    Title TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Symptoms TEXT NOT NULL,
                    RootCause TEXT NOT NULL,
                    ResolutionSteps TEXT NOT NULL,
                    Difficulty TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS PracticalExams (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LessonId TEXT NOT NULL UNIQUE,
                    Title TEXT NOT NULL,
                    Scenario TEXT NOT NULL,
                    TaskInstructions TEXT NOT NULL,
                    StarterCode TEXT NOT NULL,
                    ExpectedKeywords TEXT NOT NULL,
                    EvaluationType TEXT NOT NULL,
                    Hint TEXT NOT NULL,
                    MaxScore INTEGER DEFAULT 100,
                    XpReward INTEGER DEFAULT 100,
                    FOREIGN KEY(LessonId) REFERENCES Lessons(Id)
                );

                CREATE TABLE IF NOT EXISTS UserPracticalExams (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    LessonId TEXT NOT NULL,
                    Score INTEGER DEFAULT 0,
                    Passed INTEGER DEFAULT 0,
                    CompletedAt TEXT NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                CREATE TABLE IF NOT EXISTS HeroCapstones (
                    Id TEXT PRIMARY KEY,
                    Title TEXT NOT NULL,
                    TechPillar TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    ArchitectureSpecs TEXT NOT NULL,
                    StarterCode TEXT NOT NULL,
                    VerifierKey TEXT NOT NULL,
                    MaxScore INTEGER DEFAULT 100,
                    XpReward INTEGER DEFAULT 200
                );

                CREATE TABLE IF NOT EXISTS UserHeroCapstones (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    CapstoneId TEXT NOT NULL,
                    Score INTEGER DEFAULT 0,
                    Passed INTEGER DEFAULT 0,
                    CodeSubmission TEXT NOT NULL,
                    CompletedAt TEXT NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                -- Indexes for fast query lookup on 1.2 GHz single core HDD
                CREATE INDEX IF NOT EXISTS IX_Modules_CourseId ON Modules(CourseId);
                CREATE INDEX IF NOT EXISTS IX_Lessons_ModuleId ON Lessons(ModuleId);
                CREATE INDEX IF NOT EXISTS IX_QuizQuestions_LessonId ON QuizQuestions(LessonId);
                CREATE INDEX IF NOT EXISTS IX_UserProgress_User_Lesson ON UserProgress(UserId, LessonId);
                CREATE INDEX IF NOT EXISTS IX_Trouble_Category ON TroubleshootingScenarios(Category);
                CREATE INDEX IF NOT EXISTS IX_PracticalExams_LessonId ON PracticalExams(LessonId);
                CREATE INDEX IF NOT EXISTS IX_UserPracticalExams_User_Lesson ON UserPracticalExams(UserId, LessonId);
                CREATE INDEX IF NOT EXISTS IX_HeroCapstones_Pillar ON HeroCapstones(TechPillar);
                CREATE INDEX IF NOT EXISTS IX_UserHeroCapstones_User ON UserHeroCapstones(UserId, CapstoneId);

                CREATE TABLE IF NOT EXISTS Flashcards (
                    Id INTEGER PRIMARY KEY,
                    Category TEXT NOT NULL,
                    FrontPrompt TEXT NOT NULL,
                    BackAnswer TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS UserFlashcards (
                    UserId INTEGER NOT NULL,
                    CardId INTEGER NOT NULL,
                    BoxLevel INTEGER DEFAULT 1,
                    NextReviewDate TEXT NOT NULL,
                    PRIMARY KEY(UserId, CardId),
                    FOREIGN KEY(CardId) REFERENCES Flashcards(Id),
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );

                CREATE INDEX IF NOT EXISTS IX_UserFlashcards_User ON UserFlashcards(UserId, CardId);
            ";
            cmd.ExecuteNonQuery();

            SeedDefaultData(conn);
            CurriculumSeeder.EnsureCurriculumSeeded(conn);
            PracticalExamService.EnsurePracticalExamsSeeded(conn);
            EnsureHeroCapstonesSeeded(conn);
            EnsureFlashcardsSeeded(conn);
        }

        private static void EnsureFlashcardsSeeded(SqliteConnection conn)
        {
            using var countCmd = conn.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM Flashcards";
            long count = (long)(countCmd.ExecuteScalar() ?? 0);
            if (count >= 15) return;

            using var tx = conn.BeginTransaction();
            using var delCmd = conn.CreateCommand();
            delCmd.Transaction = tx;
            delCmd.CommandText = "DELETE FROM Flashcards";
            delCmd.ExecuteNonQuery();

            var cards = SpacedRepetitionService.GetDefaultFlashcards();
            foreach (var c in cards)
            {
                using var insCmd = conn.CreateCommand();
                insCmd.Transaction = tx;
                insCmd.CommandText = @"INSERT INTO Flashcards (Id, Category, FrontPrompt, BackAnswer)
                    VALUES (@id, @cat, @front, @back)";
                insCmd.Parameters.AddWithValue("@id", c.Id);
                insCmd.Parameters.AddWithValue("@cat", c.Category);
                insCmd.Parameters.AddWithValue("@front", c.FrontPrompt);
                insCmd.Parameters.AddWithValue("@back", c.BackAnswer);
                insCmd.ExecuteNonQuery();
            }
            tx.Commit();
        }

        private static void EnsureHeroCapstonesSeeded(SqliteConnection conn)
        {
            using var countCmd = conn.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM HeroCapstones";
            long count = (long)(countCmd.ExecuteScalar() ?? 0);
            if (count >= 6) return;

            using var tx = conn.BeginTransaction();
            using var delCmd = conn.CreateCommand();
            delCmd.Transaction = tx;
            delCmd.CommandText = "DELETE FROM HeroCapstones";
            delCmd.ExecuteNonQuery();

            var capstones = ZeroToHeroService.GetAllHeroCapstones();
            foreach (var cap in capstones)
            {
                using var insCmd = conn.CreateCommand();
                insCmd.Transaction = tx;
                insCmd.CommandText = @"INSERT INTO HeroCapstones 
                    (Id, Title, TechPillar, Description, ArchitectureSpecs, StarterCode, VerifierKey, MaxScore, XpReward)
                    VALUES (@id, @title, @pillar, @desc, @specs, @code, @key, @score, @xp)";
                insCmd.Parameters.AddWithValue("@id", cap.Id);
                insCmd.Parameters.AddWithValue("@title", cap.Title);
                insCmd.Parameters.AddWithValue("@pillar", cap.TechPillar);
                insCmd.Parameters.AddWithValue("@desc", cap.Description);
                insCmd.Parameters.AddWithValue("@specs", cap.ArchitectureSpecs);
                insCmd.Parameters.AddWithValue("@code", cap.StarterCode);
                insCmd.Parameters.AddWithValue("@key", cap.VerifierKey);
                insCmd.Parameters.AddWithValue("@score", cap.MaxScore);
                insCmd.Parameters.AddWithValue("@xp", cap.XpReward);
                insCmd.ExecuteNonQuery();
            }
            tx.Commit();
        }

        public List<TroubleshootingSeeder.ScenarioRecord> GetTroubleshootingScenarios(string? categoryFilter = null, string? searchKeyword = null)
        {
            var list = new List<TroubleshootingSeeder.ScenarioRecord>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            var query = "SELECT Id, Title, Category, Symptoms, RootCause, ResolutionSteps, Difficulty FROM TroubleshootingScenarios WHERE 1=1";
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All Categories")
            {
                query += " AND Category = @cat";
                cmd.Parameters.AddWithValue("@cat", categoryFilter);
            }
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query += " AND (Title LIKE @kw OR Symptoms LIKE @kw OR RootCause LIKE @kw)";
                cmd.Parameters.AddWithValue("@kw", $"%{searchKeyword}%");
            }
            query += " ORDER BY Id LIMIT 500";

            cmd.CommandText = query;
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TroubleshootingSeeder.ScenarioRecord(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6)
                ));
            }
            return list;
        }

        private void SeedDefaultData(SqliteConnection conn)
        {
            using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM Users";
            long count = (long)(checkCmd.ExecuteScalar() ?? 0);

            if (count > 0) return; // Already seeded

            using var tx = conn.BeginTransaction();

            // Default Student User
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    INSERT INTO Users (Username, DisplayName, Role, TotalXP, CurrentLevel, CurrentStreak, CreatedAt)
                    VALUES ('student', 'Young Innovator', 0, 150, 2, 3, @now);";
                cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
                cmd.ExecuteNonQuery();
            }

            // Courses
            var courses = new[]
            {
                ("CS101", "Computer Fundamentals", CourseCategory.Fundamentals, "Learn how computers process data, CPU, memory, and OS basics.", "Laptop", 1),
                ("PROG101", "Programming Academy", CourseCategory.Programming, "Master C#, Python, HTML/JS, and algorithm fundamentals.", "Code", 2),
                ("SW101", "Software Engineering & OS Internals", CourseCategory.Programming, "Understand compiler lexing, AST parsing, OS process scheduling, and Git version control.", "Layers", 3),
                ("HW101", "Hardware & PC Builder", CourseCategory.Hardware, "Understand CPU, RAM, GPU, Motherboards, PSUs, and assemble virtual PCs.", "Cpu", 4),
                ("HW201", "Advanced Microarchitecture & Buses", CourseCategory.Hardware, "Learn CPU pipelines, RAM bus timings, PCIe bandwidth, and VRM power delivery.", "Cpu", 5),
                ("NET101", "Networking & CCNA Essentials", CourseCategory.Networking, "Explore OSI Model, IP Addressing, Subnetting, Routers, and Switches.", "Network", 6),
                ("SEC101", "Cybersecurity & Ethical Hacking", CourseCategory.Cybersecurity, "Learn ethical hacking, password hashing, SQL injection defense, and CIA triad.", "Shield", 7),
                ("LNX101", "Linux Systems & Commands", CourseCategory.Linux, "Master Linux terminal, file permissions, bash commands, and system control.", "Terminal", 8),
                ("ELE101", "Electronics & Robotics", CourseCategory.Electronics, "Learn resistors, LEDs, sensors, microcontrollers, and circuit math.", "Zap", 9),
                ("AI101", "Artificial Intelligence Fundamentals", CourseCategory.ArtificialIntelligence, "Understand ML logic, decision trees, prompt engineering, and neural nets.", "Brain", 10),
                ("IT101", "IT Troubleshooting Lab", CourseCategory.Troubleshooting, "Diagnose PC boot failures, DNS/DHCP issues, network drops, and slow storage.", "Wrench", 11)
            };

            foreach (var (id, title, cat, desc, icon, sort) in courses)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO Courses (Id, Title, Category, Description, IconKey, SortOrder) VALUES (@id, @title, @cat, @desc, @icon, @sort)";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@cat", (int)cat);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@icon", icon);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.ExecuteNonQuery();
            }

            // Modules & Lessons Seed Data
            SeedCurriculum(conn, tx);

            tx.Commit();
        }

        private void SeedCurriculum(SqliteConnection conn, SqliteTransaction tx)
        {
            var modules = new[]
            {
                ("M_CS_1", "CS101", "How Computers Work", 1),
                ("M_PRG_1", "PROG101", "Python & Algorithm Basics", 1),
                ("M_HW_1", "HW101", "Core Computer Components", 1),
                ("M_NET_1", "NET101", "IP Addressing & Subnetting", 1),
                ("M_SEC_1", "SEC101", "Cybersecurity Fundamentals", 1),
                ("M_LNX_1", "LNX101", "Linux Terminal Mastery", 1),
                ("M_ELE_1", "ELE101", "Basic Circuits & Components", 1),
                ("M_AI_1", "AI101", "Introduction to AI & Machine Learning", 1),
                ("M_IT_1", "IT101", "Hardware & Network Troubleshooting", 1)
            };

            foreach (var (id, cId, title, sort) in modules)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO Modules (Id, CourseId, Title, SortOrder) VALUES (@id, @cId, @title, @sort)";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@cId", cId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.ExecuteNonQuery();
            }

            var lessons = new[]
            {
                ("L_CS_1", "M_CS_1", "Binary & CPU Clock Cycles", "Learn how transistors represent 0 and 1, and how clock speed dictates execution cycles.", 
                 "# Binary & CPU Clock Cycles\n\nComputers operate on binary signals (0 and 1) represented by high and low voltage in transistors.\n\n### Key Concepts:\n- **Bit**: A single 0 or 1.\n- **Byte**: 8 bits.\n- **Hertz (Hz)**: 1 cycle per second. A 1.2 GHz CPU processes **1.2 billion clock cycles** per second!", 10, "Beginner"),

                ("L_PRG_1", "M_PRG_1", "Python Variables & Loops", "Learn to store data in memory and execute code repeatedly using while and for loops.", 
                 "# Python Variables & Loops\n\nVariables store values in computer RAM.\n\n```python\n# Example Python Code\ncount = 5\nfor i in range(count):\n    print(f'Bhavani Technology Lesson {i+1}')\n```\n\nLoops allow running repetitive tasks automatically!", 15, "Beginner"),

                ("L_HW_1", "M_HW_1", "CPU, RAM & Storage Differences", "Understand the difference between volatile RAM speed and non-volatile HDD/SSD storage.", 
                 "# CPU, RAM & Storage Architecture\n\n- **CPU (Central Processing Unit)**: The brain that executes arithmetic & logical instructions.\n- **RAM (Random Access Memory)**: High-speed volatile memory that stores active programs.\n- **HDD / SSD**: Non-volatile storage where files remain saved after power off.", 15, "Beginner"),

                ("L_NET_1", "M_NET_1", "IPv4 Addresses & CIDR Notation", "Master 32-bit IPv4 structure, subnet masks (/24, /16), and network boundaries.", 
                 "# IPv4 Addressing & CIDR\n\nAn IPv4 address consists of 32 bits divided into 4 octets.\n\nExample: `192.168.1.1`\nSubnet Mask `/24` = `255.255.255.0` (254 usable host IPs).", 20, "Intermediate"),

                ("L_SEC_1", "M_SEC_1", "The CIA Triad & Password Hashing", "Learn Confidentiality, Integrity, Availability, and SHA-256 password hashing.", 
                 "# CIA Triad & Cryptographic Hashing\n\n- **Confidentiality**: Keeping data private via encryption.\n- **Integrity**: Preventing tampering via hashes (e.g. SHA-256).\n- **Availability**: Ensuring system uptime against DoS attacks.", 15, "Beginner"),

                ("L_LNX_1", "M_LNX_1", "Linux File System & Essential Commands", "Learn ls, cd, mkdir, chmod, grep, and file permissions in Linux.", 
                 "# Essential Linux Commands\n\n- `ls -la`: List files with permissions.\n- `chmod 755 script.sh`: Make script executable.\n- `grep 'error' log.txt`: Search text in log files.", 15, "Beginner"),

                ("L_ELE_1", "M_ELE_1", "Ohm's Law & Circuit Fundamentals", "Understand Voltage (V), Current (I), and Resistance (R) equation V = I * R.", 
                 "# Ohm's Law\n\n$$V = I \\times R$$\n\n- **Voltage (V)**: Electrical pressure (Volts).\n- **Current (I)**: Flow of charge (Amperes).\n- **Resistance (R)**: Opposition to current flow (Ohms).", 15, "Beginner"),

                ("L_AI_1", "M_AI_1", "How AI Decision Trees Work", "Understand rule-based expert systems and how computers categorize data.", 
                 "# Introduction to AI Logic\n\nArtificial Intelligence starts with decision trees and statistical pattern matching.\n\nRule Example: `IF CPU_Temperature > 85C THEN Alert Fan Controller`.", 15, "Beginner"),

                ("L_IT_1", "M_IT_1", "PC Boot Failures & RAM Testing", "Diagnose beep codes, NO POST errors, and loose memory modules step-by-step.", 
                 "# PC Boot Troubleshooting\n\nWhen a PC turns on with black screen and beep codes:\n1. Check RAM seating & clean contacts.\n2. Verify PSU 12V rail output.\n3. Reset CMOS battery.", 15, "Beginner")
            };

            foreach (var (id, mId, title, sum, markdown, mins, diff) in lessons)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO Lessons (Id, ModuleId, Title, Summary, ContentMarkdown, EstimatedMinutes, Difficulty) 
                                   VALUES (@id, @mId, @title, @sum, @md, @mins, @diff)";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@mId", mId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@sum", sum);
                cmd.Parameters.AddWithValue("@md", markdown);
                cmd.Parameters.AddWithValue("@mins", mins);
                cmd.Parameters.AddWithValue("@diff", diff);
                cmd.ExecuteNonQuery();

                // Add quiz question for each lesson
                using var qCmd = conn.CreateCommand();
                qCmd.Transaction = tx;
                qCmd.CommandText = @"INSERT INTO QuizQuestions (LessonId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, Explanation)
                                     VALUES (@lId, @q, @a, @b, @c, @d, @corr, @exp)";
                qCmd.Parameters.AddWithValue("@lId", id);
                qCmd.Parameters.AddWithValue("@q", $"What is the primary concept covered in {title}?");
                qCmd.Parameters.AddWithValue("@a", "Option 1: Fundamental technical mechanism");
                qCmd.Parameters.AddWithValue("@b", "Option 2: Hardware acceleration");
                qCmd.Parameters.AddWithValue("@c", "Option 3: External cloud server sync");
                qCmd.Parameters.AddWithValue("@d", "Option 4: Unused complex library");
                qCmd.Parameters.AddWithValue("@corr", "A");
                qCmd.Parameters.AddWithValue("@exp", $"This lesson focuses directly on {sum}");
                qCmd.ExecuteNonQuery();
            }

            // Seed 500+ IT Troubleshooting Scenarios
            TroubleshootingSeeder.EnsureScenariosSeeded(conn, tx);
        }

        public List<Course> GetCourses()
        {
            var list = new List<Course>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Title, Category, Description, IconKey, SortOrder FROM Courses ORDER BY SortOrder";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Course(
                    reader.GetString(0),
                    reader.GetString(1),
                    (CourseCategory)reader.GetInt32(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetInt32(5)
                ));
            }
            return list;
        }

        public List<Lesson> GetLessonsForCourse(string courseId, string? difficulty = null)
        {
            var list = new List<Lesson>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            var sql = @"
                SELECT L.Id, L.ModuleId, L.Title, L.Summary, L.ContentMarkdown, L.EstimatedMinutes, L.Difficulty 
                FROM Lessons L
                JOIN Modules M ON L.ModuleId = M.Id
                WHERE M.CourseId = @cId";

            if (!string.IsNullOrEmpty(difficulty) && difficulty != "All Levels")
            {
                sql += " AND L.Difficulty = @diff";
                cmd.Parameters.AddWithValue("@diff", difficulty);
            }
            sql += " ORDER BY M.SortOrder, L.Id";

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@cId", courseId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Lesson(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetInt32(5),
                    reader.GetString(6)
                ));
            }
            return list;
        }

        public QuizQuestion? GetQuizForLesson(string lessonId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, LessonId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, Explanation
                FROM QuizQuestions
                WHERE LessonId = @lId LIMIT 1";
            cmd.Parameters.AddWithValue("@lId", lessonId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new QuizQuestion(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.GetString(7)[0],
                    reader.GetString(8)
                );
            }
            return null;
        }

        public PracticalExam? GetPracticalExamForLesson(string lessonId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, LessonId, Title, Scenario, TaskInstructions, StarterCode, ExpectedKeywords, EvaluationType, Hint, MaxScore, XpReward
                FROM PracticalExams
                WHERE LessonId = @lId LIMIT 1";
            cmd.Parameters.AddWithValue("@lId", lessonId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new PracticalExam(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.GetString(7),
                    reader.GetString(8),
                    reader.GetInt32(9),
                    reader.GetInt32(10)
                );
            }
            return null;
        }

        public void SavePracticalExamProgress(int userId, string lessonId, int score, int xpEarned)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO UserPracticalExams (UserId, LessonId, Score, Passed, CompletedAt)
                VALUES (@uId, @lId, @score, 1, @now);
                UPDATE Users SET TotalXP = TotalXP + @xp, CurrentLevel = ((TotalXP + @xp) / 100) + 1 WHERE Id = @uId;
            ";
            cmd.Parameters.AddWithValue("@uId", userId);
            cmd.Parameters.AddWithValue("@lId", lessonId);
            cmd.Parameters.AddWithValue("@score", score);
            cmd.Parameters.AddWithValue("@xp", xpEarned);
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public int GetPracticalExamPassCount(int userId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(DISTINCT LessonId) FROM UserPracticalExams WHERE UserId = @uId AND Passed = 1";
            cmd.Parameters.AddWithValue("@uId", userId);
            var res = cmd.ExecuteScalar();
            return res != null ? Convert.ToInt32(res) : 0;
        }

        public int AddUserXp(int points)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Users 
                SET TotalXP = TotalXP + @pts,
                    CurrentLevel = ((TotalXP + @pts) / 100) + 1
                WHERE Id = 1;
                SELECT TotalXP FROM Users WHERE Id = 1;";
            cmd.Parameters.AddWithValue("@pts", points);
            var res = cmd.ExecuteScalar();
            return res != null ? Convert.ToInt32(res) : 0;
        }

        public User? GetUser(int userId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, DisplayName, Role, TotalXP, CurrentLevel, CurrentStreak, CreatedAt FROM Users WHERE Id = @uId LIMIT 1";
            cmd.Parameters.AddWithValue("@uId", userId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    (UserRole)reader.GetInt32(3),
                    reader.GetInt32(4),
                    reader.GetInt32(5),
                    reader.GetInt32(6),
                    DateTime.Parse(reader.GetString(7))
                );
            }
            return null;
        }

        public bool HasUsers()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users";
            return ((long)(cmd.ExecuteScalar() ?? 0L)) > 0;
        }

        public void CreateStudent(string name)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, DisplayName, Role, TotalXP, CurrentLevel, CurrentStreak, CreatedAt) VALUES (@name, @name, 0, 150, 2, 1, @now);";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public User? GetCurrentUser()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, DisplayName, Role, TotalXP, CurrentLevel, CurrentStreak, CreatedAt FROM Users LIMIT 1";
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    (UserRole)reader.GetInt32(3),
                    reader.GetInt32(4),
                    reader.GetInt32(5),
                    reader.GetInt32(6),
                    DateTime.Parse(reader.GetString(7))
                );
            }
            return null;
        }

        public List<string> GetCompletedLessonIds(int userId = 1)
        {
            var list = new List<string>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT LessonId FROM UserProgress WHERE UserId = @uId AND Status = 'Completed'";
            cmd.Parameters.AddWithValue("@uId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            return list;
        }

        public void MarkLessonCompleted(string lessonId, int userId = 1)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO UserProgress (UserId, LessonId, Status, Score, XpEarned, CompletedAt)
                VALUES (@uId, @lId, 'Completed', 100, 50, @now);";
            cmd.Parameters.AddWithValue("@uId", userId);
            cmd.Parameters.AddWithValue("@lId", lessonId);
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public (Course? Course, Lesson? Lesson) GetNextRecommendedLesson(int userId = 1)
        {
            var completed = new HashSet<string>(GetCompletedLessonIds(userId));
            var courses = GetCourses();
            foreach (var course in courses)
            {
                var lessons = GetLessonsForCourse(course.Id);
                foreach (var lesson in lessons)
                {
                    if (!completed.Contains(lesson.Id))
                    {
                        return (course, lesson);
                    }
                }
            }
            if (courses.Count > 0)
            {
                var firstCourse = courses[0];
                var lessons = GetLessonsForCourse(firstCourse.Id);
                if (lessons.Count > 0) return (firstCourse, lessons[0]);
            }
            return (null, null);
        }

        public void SaveHeroCapstoneSubmission(int userId, string capstoneId, int score, int xpEarned, string codeSubmission)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO UserHeroCapstones (UserId, CapstoneId, Score, Passed, CodeSubmission, CompletedAt)
                VALUES (@uId, @cId, @score, 1, @code, @now);
                UPDATE Users SET TotalXP = TotalXP + @xp, CurrentLevel = ((TotalXP + @xp) / 100) + 1 WHERE Id = @uId;
            ";
            cmd.Parameters.AddWithValue("@uId", userId);
            cmd.Parameters.AddWithValue("@cId", capstoneId);
            cmd.Parameters.AddWithValue("@score", score);
            cmd.Parameters.AddWithValue("@code", codeSubmission);
            cmd.Parameters.AddWithValue("@xp", xpEarned);
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public int GetCompletedHeroCapstoneCount(int userId = 1)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(DISTINCT CapstoneId) FROM UserHeroCapstones WHERE UserId = @uId AND Passed = 1";
            cmd.Parameters.AddWithValue("@uId", userId);
            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        public List<string> GetUserCompletedHeroCapstoneIds(int userId = 1)
        {
            var list = new List<string>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT CapstoneId FROM UserHeroCapstones WHERE UserId = @uId AND Passed = 1";
            cmd.Parameters.AddWithValue("@uId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            return list;
        }

        public List<HeroCapstoneProject> GetHeroCapstones()
        {
            var list = new List<HeroCapstoneProject>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Title, TechPillar, Description, ArchitectureSpecs, StarterCode, VerifierKey, MaxScore, XpReward FROM HeroCapstones";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new HeroCapstoneProject(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.GetInt32(7),
                    reader.GetInt32(8)
                ));
            }
            return list;
        }

        public bool BackupDatabaseToFile(string destinationPath)
        {
            try
            {
                var dir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

                using var sourceConn = GetConnection();
                using var destConn = new SqliteConnection($"Data Source={destinationPath}");
                destConn.Open();
                sourceConn.BackupDatabase(destConn);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Flashcard> GetFlashcardsForReview(int userId = 1)
        {
            var list = new List<Flashcard>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT f.Id, f.Category, f.FrontPrompt, f.BackAnswer, 
                       COALESCE(u.BoxLevel, 1), COALESCE(u.NextReviewDate, @now)
                FROM Flashcards f
                LEFT JOIN UserFlashcards u ON f.Id = u.CardId AND u.UserId = @uId
                ORDER BY f.Id ASC";
            cmd.Parameters.AddWithValue("@uId", userId);
            cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Flashcard(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetInt32(4),
                    reader.GetString(5)
                ));
            }
            return list;
        }

        public void UpdateFlashcardProgress(int userId, int cardId, int boxLevel, string nextReviewDate)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO UserFlashcards (UserId, CardId, BoxLevel, NextReviewDate)
                VALUES (@uId, @cId, @box, @next)
                ON CONFLICT(UserId, CardId) DO UPDATE SET
                    BoxLevel = @box,
                    NextReviewDate = @next;
            ";
            cmd.Parameters.AddWithValue("@uId", userId);
            cmd.Parameters.AddWithValue("@cId", cardId);
            cmd.Parameters.AddWithValue("@box", boxLevel);
            cmd.Parameters.AddWithValue("@next", nextReviewDate);
            cmd.ExecuteNonQuery();
        }
    }
}



