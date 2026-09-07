using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Models
{
    public enum UserRole
    {
        Student,
        Parent,
        Teacher,
        Administrator
    }

    public enum CourseCategory
    {
        Fundamentals,
        Programming,
        Hardware,
        Networking,
        Cybersecurity,
        Linux,
        Electronics,
        ArtificialIntelligence,
        Troubleshooting
    }

    public record User(
        int Id,
        string Username,
        string DisplayName,
        UserRole Role,
        int TotalXP,
        int CurrentLevel,
        int CurrentStreak,
        DateTime CreatedAt
    );

    public record Course(
        string Id,
        string Title,
        CourseCategory Category,
        string Description,
        string IconKey,
        int SortOrder
    );

    public record ModuleItem(
        string Id,
        string CourseId,
        string Title,
        int SortOrder
    );

    public record Lesson(
        string Id,
        string ModuleId,
        string Title,
        string Summary,
        string ContentMarkdown,
        int EstimatedMinutes,
        string Difficulty
    );

    public record QuizQuestion(
        int Id,
        string LessonId,
        string QuestionText,
        string OptionA,
        string OptionB,
        string OptionC,
        string OptionD,
        char CorrectOption,
        string Explanation
    );

    public record UserProgress(
        int Id,
        int UserId,
        string LessonId,
        string Status,
        int Score,
        int XpEarned,
        DateTime CompletedAt
    );

    public record Achievement(
        string Id,
        string Code,
        string Title,
        string Description,
        string IconKey,
        int XpReward
    );

    public record UserAchievement(
        int UserId,
        string AchievementId,
        DateTime UnlockedAt
    );

    public record SubnetResult(
        string NetworkAddress,
        string BroadcastAddress,
        string FirstUsableIP,
        string LastUsableIP,
        long UsableHosts,
        string Netmask,
        int CIDR,
        string WildcardMask
    );

    public record CodeExecutionResult(
        bool Success,
        string Output,
        string Error,
        double ExecutionTimeMs,
        long MemoryUsedBytes
    );

    public record NetworkPacketStep(
        string StepDescription,
        string SourceDevice,
        string DestinationDevice,
        string LayerInfo,
        string HeaderDetails,
        bool Success
    );

    public record TroubleshootingStep(
        string ActionText,
        string ResponseOutcome,
        bool Solved,
        List<string> NextAvailableActions
    );

    public record PracticalExam(
        int Id,
        string LessonId,
        string Title,
        string Scenario,
        string TaskInstructions,
        string StarterCode,
        string ExpectedKeywords,
        string EvaluationType,
        string Hint,
        int MaxScore,
        int XpReward
    );

    public record PracticalExamResult(
        bool Passed,
        int Score,
        int MaxScore,
        int XpEarned,
        string Summary,
        List<string> TestOutputLogs
    );

    public record MazeGameState(
        int Level,
        int GridWidth,
        int GridHeight,
        int BotX,
        int BotY,
        int BotDirection, // 0: Up, 1: Right, 2: Down, 3: Left
        int TargetX,
        int TargetY,
        List<(int X, int Y)> Walls,
        List<(int X, int Y)> Gems,
        int GemsCollected,
        bool Completed,
        string Message
    );

    public record NetworkPacket(
        int Id,
        string SourceIp,
        string DestIp,
        int Port,
        string Protocol,
        string Payload,
        bool IsMalicious,
        string ThreatName
    );

    public record CircuitGate(
        string Id,
        string GateType, // AND, OR, XOR, NOT, NAND, NOR
        bool InputA,
        bool InputB,
        bool Output
    );

    // =========================================================================
    // ZERO TO HERO IN COMPLETE TECHNOLOGY MODELS
    // =========================================================================
    public enum HeroStageTier
    {
        Stage0_GroundZero,         // Electricity, Transistors, Bits, Computer Architecture 101
        Stage1_ApprenticeCoder,    // Data Structures, Control Flow, Algorithms, Problem Solving
        Stage2_SystemsCraftsman,   // CPU Registers, Assembly, Memory Hierarchy, Virtual Memory, Kernels
        Stage3_FullStackArchitect, // Web Protocols, Relational DBs, REST APIs, Frontend DOM
        Stage4_DevOpsCloudSentinel,// Linux CLI, Sockets, Cryptography, Containers, Microservices
        Stage5_FrontierHero        // Neural Nets, Transformers, Embedded Silicon, System Design
    }

    public record HeroStageMilestone(
        HeroStageTier Tier,
        string TierTitle,
        string BadgeName,
        string Summary,
        List<string> KeyTopics,
        string CapstoneTitle,
        int RequiredXp
    );

    public record HeroCapstoneProject(
        string Id,
        string Title,
        string TechPillar,
        string Description,
        string ArchitectureSpecs,
        string StarterCode,
        string VerifierKey,
        int MaxScore,
        int XpReward
    );

    public record HeroDiagnosticQuestion(
        int Id,
        string Question,
        List<string> Options,
        int CorrectOptionIndex,
        string PillarAssessed,
        string Explanation
    );

    public record HeroReadinessReport(
        int CodingScore,
        int SystemsScore,
        int NetworksScore,
        int CyberScore,
        int DatabaseScore,
        int AiScore,
        int OverallPercentage,
        string ReadinessTier,
        List<string> RecommendedMilestones
    );

    public record HeroCapstoneResult(
        bool Passed,
        int Score,
        int XpEarned,
        string Summary,
        List<string> TestOutputLogs
    );

    // =========================================================================
    // ADVANCED WORKBENCHES, AUDITING & ACCESSIBILITY MODELS
    // =========================================================================
    public record Flashcard(
        int Id,
        string Category,
        string FrontPrompt,
        string BackAnswer,
        int BoxLevel,
        string NextReviewDate
    );

    public record GitNode(
        string CommitHash,
        string Message,
        string BranchName,
        string? ParentHash,
        DateTime Timestamp
    );

    public record DebuggerFrameState(
        int LineNumber,
        string CodeLine,
        Dictionary<string, string> Variables,
        List<string> CallStack,
        string ConsoleOutput
    );

    public record RestApiRequest(
        string Method,
        string Url,
        Dictionary<string, string> Headers,
        string Body
    );

    public record RestApiResponse(
        int StatusCode,
        string StatusText,
        Dictionary<string, string> Headers,
        string Body,
        long LatencyMs
    );

    public record RegexResult(
        bool IsMatch,
        int MatchCount,
        List<string> Matches,
        List<Dictionary<string, string>> Groups,
        string Explanation
    );

    public record ParentAuditReport(
        string StudentName,
        int TotalStudyMinutes,
        int TotalXp,
        int CompletedLessonsCount,
        int TotalCurriculumLessons,
        double QuizAccuracyPercent,
        int PracticalExamsPassed,
        int HeroCapstonesCompleted,
        int CurrentStreakDays,
        string HeroReadinessTier,
        List<string> Strengths,
        List<string> FocusAreas
    );

    public record CheatSheetItem(
        string Command,
        string Description,
        string Category,
        string Example
    );
}

