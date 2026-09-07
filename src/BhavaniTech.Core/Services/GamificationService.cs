using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class GamificationService
    {
        // 1. Daily Streaks
        public record StreakResult(int CurrentStreak, int LongestStreak, double XpMultiplier, bool IsNewDay);

        public StreakResult CalculateStreak(DateTime lastLoginDate, DateTime currentLoginDate, int currentStreak, int longestStreak)
        {
            if (lastLoginDate == DateTime.MinValue)
            {
                return new StreakResult(1, Math.Max(1, longestStreak), 1.0, true);
            }

            var timeSinceLastLogin = currentLoginDate.Date - lastLoginDate.Date;
            
            if (timeSinceLastLogin.Days == 0)
            {
                // Already logged in today
                return new StreakResult(currentStreak, longestStreak, 1.0 + (currentStreak * 0.1), false);
            }
            else if (timeSinceLastLogin.Days == 1)
            {
                // Consecutive day!
                int newStreak = currentStreak + 1;
                int newLongest = Math.Max(newStreak, longestStreak);
                double multiplier = 1.0 + (Math.Min(newStreak, 10) * 0.1); // Max 2.0x multiplier
                return new StreakResult(newStreak, newLongest, multiplier, true);
            }
            else
            {
                // Streak broken
                return new StreakResult(1, longestStreak, 1.0, true);
            }
        }

        // 2. Endless Survival Mode
        public record SurvivalQuestion(int Id, string Question, string CorrectAnswer, string[] WrongOptions);

        public List<SurvivalQuestion> GenerateSurvivalRound(int roundNumber)
        {
            // In a real app this pulls from the DB. We'll generate a challenging set.
            var questions = new List<SurvivalQuestion>
            {
                new(1, "What port does HTTPS use?", "443", new[] { "80", "22", "21" }),
                new(2, "What is the Big O of Binary Search?", "O(log n)", new[] { "O(1)", "O(n)", "O(n^2)" }),
                new(3, "In Git, what merges branches?", "git merge", new[] { "git fork", "git join", "git append" }),
                new(4, "What is the CPU Program Counter?", "Address of next instruction", new[] { "Number of programs", "Memory size", "Clock speed" }),
                new(5, "What algorithm avoids deadlocks?", "Banker's Algorithm", new[] { "Round Robin", "Dijkstra's Shortest Path", "A*" }),
                new(6, "What is CAP theorem's 'P'?", "Partition Tolerance", new[] { "Performance", "Parallelism", "Protocol" })
            };

            // Shuffle and scale difficulty
            var rnd = new Random(roundNumber * 42);
            return questions.OrderBy(x => rnd.Next()).ToList();
        }

        public record SurvivalResult(int Score, bool IsNewHighScore);

        public SurvivalResult SubmitSurvivalScore(int currentScore, int previousHighScore)
        {
            bool isNew = currentScore > previousHighScore;
            return new SurvivalResult(currentScore, isNew);
        }

        // 3. Dynamic Hints
        public string GenerateDynamicHint(string studentAnswer, string expectedAnswer)
        {
            studentAnswer = studentAnswer.Trim();
            expectedAnswer = expectedAnswer.Trim();

            if (string.IsNullOrEmpty(studentAnswer))
                return "You didn't write anything! Give it a try.";

            if (expectedAnswer.EndsWith(";") && !studentAnswer.EndsWith(";"))
                return "Don't forget to end your statement with a semicolon (;).";

            if (expectedAnswer.Contains("{") && !studentAnswer.Contains("{"))
                return "Are you missing opening/closing curly braces { }?";

            if (expectedAnswer.StartsWith("def ") && !studentAnswer.StartsWith("def "))
                return "In Python, use the 'def' keyword to define a function.";

            if (studentAnswer.ToLower() == expectedAnswer.ToLower())
                return "Check your capitalization! It's case-sensitive.";

            return "Not quite right. Double check the syntax and try again.";
        }
    }
}
