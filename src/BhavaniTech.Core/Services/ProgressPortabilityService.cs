using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BhavaniTech.Core.Services
{
    public class StudentProgressProfile
    {
        public string StudentName { get; set; } = "Dharmesh Varia";
        public int Level { get; set; } = 15;
        public int TotalXp { get; set; } = 4850;
        public List<string> CompletedLessonIds { get; set; } = new();
        public List<string> UnlockedBadgeIds { get; set; } = new();
        public List<TechnologyCertificate> Certificates { get; set; } = new();
        public DateTime LastSavedUtc { get; set; } = DateTime.UtcNow;
        public string AppVersion { get; set; } = "2.0.0-PRO";
    }

    public static class ProgressPortabilityService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static string ExportProgressToJson(StudentProgressProfile profile)
        {
            profile.LastSavedUtc = DateTime.UtcNow;
            return JsonSerializer.Serialize(profile, JsonOptions);
        }

        public static StudentProgressProfile ImportProgressFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Provided JSON string is empty.");
            }

            var profile = JsonSerializer.Deserialize<StudentProgressProfile>(json, JsonOptions);
            if (profile == null)
            {
                throw new InvalidOperationException("Failed to parse progress profile JSON.");
            }

            return profile;
        }

        public static StudentProgressProfile CreateDefaultProfile(string name = "Student")
        {
            return new StudentProgressProfile
            {
                StudentName = name,
                Level = 5,
                TotalXp = 1250,
                CompletedLessonIds = new List<string> { "os_intro", "crypto_basics", "logic_gates", "sql_ddl", "python_basics" },
                UnlockedBadgeIds = new List<string> { "badge_byte", "badge_kernel", "badge_polyglot" },
                Certificates = new List<TechnologyCertificate>()
            };
        }
    }
}
