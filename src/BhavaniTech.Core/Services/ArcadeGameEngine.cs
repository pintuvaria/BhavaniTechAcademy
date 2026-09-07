using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BhavaniTech.Core.Models;
using Microsoft.Data.Sqlite;

namespace BhavaniTech.Core.Services
{
    public class ArcadeGameEngine
    {
        // =========================================================================
        // 1. GAME 1: BYTEBOT CODE MAZE RUNNER
        // =========================================================================
        public static MazeGameState GetMazeLevel(int level)
        {
            return level switch
            {
                1 => new MazeGameState(
                    Level: 1, GridWidth: 6, GridHeight: 6,
                    BotX: 1, BotY: 1, BotDirection: 1, // Facing Right
                    TargetX: 4, TargetY: 1,
                    Walls: new List<(int, int)> { (2, 0), (2, 2), (2, 3), (3, 3) },
                    Gems: new List<(int, int)> { (2, 1), (3, 1) },
                    GemsCollected: 0, Completed: false, Message: "Level 1: Pipeline Corridor. Move forward to collect chips and reach the CPU!"
                ),
                2 => new MazeGameState(
                    Level: 2, GridWidth: 6, GridHeight: 6,
                    BotX: 1, BotY: 4, BotDirection: 0, // Facing Up
                    TargetX: 4, TargetY: 1,
                    Walls: new List<(int, int)> { (2, 4), (2, 3), (2, 2), (3, 2), (3, 3) },
                    Gems: new List<(int, int)> { (1, 2), (1, 1), (3, 1) },
                    GemsCollected: 0, Completed: false, Message: "Level 2: The Turn. Turn right and bypass the firewall wall."
                ),
                _ => new MazeGameState(
                    Level: 3, GridWidth: 7, GridHeight: 7,
                    BotX: 1, BotY: 5, BotDirection: 0,
                    TargetX: 5, TargetY: 1,
                    Walls: new List<(int, int)> { (2, 5), (2, 4), (2, 3), (4, 1), (4, 2), (4, 3), (3, 3) },
                    Gems: new List<(int, int)> { (1, 3), (3, 2), (5, 3) },
                    GemsCollected: 0, Completed: false, Message: "Level 3: Central Datacenter Labyrinth. Collect all 3 chips and ignite the mainframe!"
                )
            };
        }

        public static (MazeGameState State, List<string> ExecutionLog) RunBotCommands(MazeGameState initial, string commandsText)
        {
            var logs = new List<string>();
            var state = initial with { Walls = new List<(int, int)>(initial.Walls), Gems = new List<(int, int)>(initial.Gems) };
            logs.Add($"🤖 ByteBot initialized at ({state.BotX}, {state.BotY}), facing {GetDirectionName(state.BotDirection)}.");

            var rawTokens = commandsText.ToUpperInvariant()
                .Replace(";", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("(", " ")
                .Replace(")", " ")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var commands = ExpandLoops(rawTokens);

            foreach (var cmd in commands)
            {
                if (state.Completed) break;

                switch (cmd)
                {
                    case "FORWARD":
                    case "MOVE":
                    case "F":
                        int nextX = state.BotX;
                        int nextY = state.BotY;
                        if (state.BotDirection == 0) nextY--; // Up
                        else if (state.BotDirection == 1) nextX++; // Right
                        else if (state.BotDirection == 2) nextY++; // Down
                        else if (state.BotDirection == 3) nextX--; // Left

                        if (nextX < 0 || nextX >= state.GridWidth || nextY < 0 || nextY >= state.GridHeight)
                        {
                            logs.Add($"[WALL COLLISION ⚠️] Bot attempted to walk out of grid bounds at ({nextX}, {nextY})!");
                        }
                        else if (state.Walls.Contains((nextX, nextY)))
                        {
                            logs.Add($"[FIREWALL BLOCKED ⛔] Crash into firewall obstacle at ({nextX}, {nextY})!");
                        }
                        else
                        {
                            state = state with { BotX = nextX, BotY = nextY };
                            logs.Add($"Bot moved forward to ({nextX}, {nextY}).");

                            // Check gem collection
                            if (state.Gems.Contains((nextX, nextY)))
                            {
                                state.Gems.Remove((nextX, nextY));
                                state = state with { GemsCollected = state.GemsCollected + 1 };
                                logs.Add($"💎 Memory Chip Collected at ({nextX}, {nextY})! Total Chips: {state.GemsCollected}");
                            }

                            // Check target arrival
                            if (state.BotX == state.TargetX && state.BotY == state.TargetY)
                            {
                                state = state with { Completed = true, Message = "LEVEL CLEARED! ByteBot reached the Mainframe! (+100 XP)" };
                                logs.Add($"🎉 VICTORY! Central CPU reached at ({state.TargetX}, {state.TargetY})!");
                            }
                        }
                        break;

                    case "TURN_LEFT":
                    case "LEFT":
                    case "L":
                        int newDirL = (state.BotDirection + 3) % 4;
                        state = state with { BotDirection = newDirL };
                        logs.Add($"Bot turned left. Now facing {GetDirectionName(newDirL)}.");
                        break;

                    case "TURN_RIGHT":
                    case "RIGHT":
                    case "R":
                        int newDirR = (state.BotDirection + 1) % 4;
                        state = state with { BotDirection = newDirR };
                        logs.Add($"Bot turned right. Now facing {GetDirectionName(newDirR)}.");
                        break;
                }
            }

            if (!state.Completed)
            {
                logs.Add($"Execution ended. Target not reached yet. Current pos: ({state.BotX}, {state.BotY}). Target: ({state.TargetX}, {state.TargetY}).");
            }

            return (state, logs);
        }

        private static List<string> ExpandLoops(string[] tokens)
        {
            var result = new List<string>();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "REPEAT" && i + 2 < tokens.Length && int.TryParse(tokens[i + 1], out int count))
                {
                    string action = tokens[i + 2];
                    for (int c = 0; c < count; c++) result.Add(action);
                    i += 2;
                }
                else if (i + 1 < tokens.Length && int.TryParse(tokens[i + 1], out int actCount) &&
                         (tokens[i] == "FORWARD" || tokens[i] == "MOVE" || tokens[i] == "F" ||
                          tokens[i] == "LEFT" || tokens[i] == "RIGHT" || tokens[i] == "TURN_LEFT" || tokens[i] == "TURN_RIGHT" ||
                          tokens[i] == "L" || tokens[i] == "R"))
                {
                    string action = tokens[i];
                    for (int c = 0; c < actCount; c++) result.Add(action);
                    i += 1;
                }
                else
                {
                    result.Add(tokens[i]);
                }
            }
            return result;
        }

        private static string GetDirectionName(int dir) => dir switch { 0 => "NORTH (UP) ⬆️", 1 => "EAST (RIGHT) ➡️", 2 => "SOUTH (DOWN) ⬇️", _ => "WEST (LEFT) ⬅️" };

        // =========================================================================
        // 2. GAME 2: CYBER DEFENSE FIREWALL PACKET SENTRY
        // =========================================================================
        public static List<NetworkPacket> GeneratePacketWave(int waveNumber)
        {
            var rnd = new Random(waveNumber * 37);
            var templates = new (string ip, int port, string proto, string payload, bool evil, string threat)[]
            {
                ("185.220.101.5", 4444, "TCP", "Metasploit Meterpreter Reverse TCP Shell", true, "Reverse Shell"),
                ("103.24.120.1", 23, "TCP", "Mirai Telnet Brute Force Admin Login", true, "Telnet Botnet"),
                ("45.142.212.8", 3389, "TCP", "BlueKeep MS12-020 RDP Probe", true, "RDP Exploit"),
                ("192.168.1.50", 80, "TCP", "GET /api/user?id=1' OR 1=1--", true, "SQL Injection"),
                ("194.26.29.11", 80, "TCP", "GET /search?q=<script>alert(1)</script>", true, "Cross-Site Scripting"),
                ("142.250.190.46", 443, "TCP", "TLS 1.3 ClientHello (Google HTTPS)", false, "Legitimate HTTPS"),
                ("1.1.1.1", 53, "UDP", "Standard DNS Query A records", false, "Cloudflare DNS"),
                ("192.168.1.10", 22, "TCP", "SSH-2.0-OpenSSH_8.9 authorized_keys", false, "Internal Admin SSH"),
                ("10.0.0.15", 80, "TCP", "GET /index.html HTTP/1.1", false, "Internal Web Server"),
                ("172.16.0.4", 443, "TCP", "POST /api/v1/telemetry 200 OK", false, "Encrypted Telemetry")
            };

            var list = new List<NetworkPacket>();
            for (int i = 0; i < 6; i++)
            {
                var t = templates[rnd.Next(templates.Length)];
                list.Add(new NetworkPacket(i + 1, t.ip, "192.168.1.1", t.port, t.proto, t.payload, t.evil, t.threat));
            }
            return list;
        }

        public static (int ScoreDelta, int HealthDelta, string Feedback) EvaluateFirewallAction(NetworkPacket packet, bool userDropped)
        {
            if (packet.IsMalicious)
            {
                if (userDropped)
                {
                    return (+100, 0, $"🛡️ ATTACK BLOCKED! Dropped malicious {packet.ThreatName} packet on Port {packet.Port}! (+100 PTS)");
                }
                else
                {
                    return (-50, -25, $"💥 BREACH DETECTED! Allowed malicious {packet.ThreatName} into internal network! (-25% Health)");
                }
            }
            else
            {
                if (!userDropped)
                {
                    return (+50, 0, $"✅ LEGITIMATE TRAFFIC PASSED: Allowed valid {packet.Protocol} on Port {packet.Port}. (+50 PTS)");
                }
                else
                {
                    return (-30, -10, $"⚠️ FALSE POSITIVE! Erroneously dropped legitimate {packet.Protocol} packet on Port {packet.Port}! (-10% Health)");
                }
            }
        }

        // =========================================================================
        // 3. GAME 3: CIRCUIT LOGIC REACTOR
        // =========================================================================
        public static (bool CorePower, bool Out1, bool Out2, string Diagnostic) EvaluateReactorLogic(
            bool swA, bool swB, bool swC, bool swD,
            string gate1Type, string gate2Type, string gate3Type)
        {
            bool out1 = EvaluateGate(gate1Type, swA, swB);
            bool out2 = EvaluateGate(gate2Type, swC, swD);
            bool corePower = EvaluateGate(gate3Type, out1, out2);

            string diag = $"Stage 1 Gate ({gate1Type}): InA={swA}, InB={swB} => Out1={out1}\n" +
                          $"Stage 2 Gate ({gate2Type}): InC={swC}, InD={swD} => Out2={out2}\n" +
                          $"Stage 3 Gate ({gate3Type}): Out1={out1}, Out2={out2} => Core={corePower}\n";

            if (corePower)
                diag += "⚡ REACTOR ONLINE! Clean stable power flowing to CPU! (+100 XP)";
            else
                diag += "🔴 REACTOR OFFLINE. Core voltage is 0V. Adjust switches or gates to achieve 1.";

            return (corePower, out1, out2, diag);
        }

        private static bool EvaluateGate(string type, bool a, bool b) => type switch
        {
            "AND" => a && b,
            "OR" => a || b,
            "XOR" => a ^ b,
            "NAND" => !(a && b),
            "NOR" => !(a || b),
            "NOT_A" => !a,
            _ => a && b
        };

        // =========================================================================
        // 4. GAME 4: SQL DUNGEON QUEST
        // =========================================================================
        public static (bool Success, string OutcomeMessage, string RenderedGrid) ExecuteDungeonQuery(string userSql, int questLevel)
        {
            try
            {
                using var conn = new SqliteConnection("Data Source=:memory:");
                conn.Open();

                // Seed Dungeon Database
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE Monsters (Id INTEGER PRIMARY KEY, Name TEXT, Element TEXT, HP INTEGER, Weakness TEXT);
                        INSERT INTO Monsters VALUES 
                            (1, 'Flame Dragon', 'FIRE', 200, 'ICE'),
                            (2, 'Shadow Goblin', 'DARK', 50, 'LIGHT'),
                            (3, 'Ice Golem', 'ICE', 120, 'FIRE');

                        CREATE TABLE Inventory (Id INTEGER PRIMARY KEY, ItemName TEXT, Power INTEGER, Equipped INTEGER);
                        INSERT INTO Inventory VALUES 
                            (1, 'Wooden Staff', 10, 1),
                            (2, 'Frost Spellbook', 85, 0),
                            (3, 'Health Potion', 50, 0);

                        CREATE TABLE Doors (RoomId INTEGER PRIMARY KEY, DoorName TEXT, IsLocked INTEGER);
                        INSERT INTO Doors VALUES 
                            (1, 'Entrance Gate', 0),
                            (2, 'Dragon Chamber', 1);
                    ";
                    cmd.ExecuteNonQuery();
                }

                // Execute User Query
                using (var runCmd = conn.CreateCommand())
                {
                    runCmd.CommandText = userSql;
                    using var reader = runCmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(reader);

                    // Check Quest Victory Condition
                    switch (questLevel)
                    {
                        case 1: // Quest: Find Ice Golem or Flame Dragon weakness
                            if (dt.Rows.Count > 0 && userSql.IndexOf("Monsters", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return (true, "✨ QUEST 1 COMPLETE! You scouted monster weaknesses using SQL SELECT! (+50 XP)",
                                    "🐉 [Flame Dragon HP: 200]  <== Weak to ICE!\n⚔️ Hero cast Frost Bolt for Critical Hit!");
                            }
                            break;

                        case 2: // Quest: Equip Frost Spellbook
                            using (var checkCmd = conn.CreateCommand())
                            {
                                checkCmd.CommandText = "SELECT Equipped FROM Inventory WHERE ItemName LIKE '%Frost%'";
                                var val = checkCmd.ExecuteScalar();
                                if (val != null && Convert.ToInt32(val) == 1)
                                {
                                    return (true, "🗡️ QUEST 2 COMPLETE! You equipped the Frost Spellbook via SQL UPDATE! (+100 XP)",
                                        "🧙 Hero equipped [Frost Spellbook] (Power: 85)!\n💥 Dragon took 85 Cold Damage!");
                                }
                            }
                            break;

                        case 3: // Quest: Unlock Dragon Chamber
                            using (var checkCmd = conn.CreateCommand())
                            {
                                checkCmd.CommandText = "SELECT IsLocked FROM Doors WHERE RoomId = 2";
                                var val = checkCmd.ExecuteScalar();
                                if (val != null && Convert.ToInt32(val) == 0)
                                {
                                    return (true, "🚪 QUEST 3 COMPLETE! You unlocked the Dragon Chamber Door via SQL! (+100 XP)",
                                        "🔓 Door opened! You gained access to the Dungeon Hoard!");
                                }
                            }
                            break;
                    }

                    return (false, $"Query executed ({dt.Rows.Count} rows returned), but did not fulfill Quest #{questLevel} objective.",
                        "Inspect the quest instructions and database tables (Monsters, Inventory, Doors).");
                }
            }
            catch (Exception ex)
            {
                return (false, $"SQL Error: {ex.Message}", "Syntax error in SQL query execution.");
            }
        }

        // =========================================================================
        // 5. GAME 5: BINARY BLITZ BIT SHIFTER
        // =========================================================================
        public static (int TargetDecimal, string TargetHex, string TargetBinary) GenerateBinaryTarget()
        {
            var rnd = new Random();
            int val = rnd.Next(1, 256);
            return (val, $"0x{val:X2}", Convert.ToString(val, 2).PadLeft(8, '0'));
        }

        public static int CalculateByteFromBits(bool[] bits)
        {
            int val = 0;
            for (int i = 0; i < 8; i++)
            {
                if (bits[i]) val |= (1 << (7 - i));
            }
            return val;
        }

        // =========================================================================
        // 6. GAME 6: ASSEMBLY ARENA (MICRO-HACKER)
        // =========================================================================
        public static (bool Victory, int DroneHpRemaining, List<string> Logs) ExecuteAssemblyBot(string asmCode, int droneHp = 100)
        {
            var logs = new List<string>();
            logs.Add("🤖 Assembly Arena Execution Initiated...");
            var regs = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "R0", 0 }, { "R1", 0 }, { "R2", 0 }, { "R3", 0 },
                { "RAX", 0 }, { "RBX", 0 }, { "RCX", 0 }, { "RDX", 0 }
            };

            var lines = asmCode.Split(new[] { '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("//")).ToList();

            int currentHp = droneHp;
            int instructionsExecuted = 0;

            foreach (var line in lines)
            {
                instructionsExecuted++;
                var tokens = line.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0) continue;

                string op = tokens[0].ToUpperInvariant();
                string target = tokens.Length >= 2 ? tokens[1].ToUpperInvariant() : (regs.TryGetValue("RAX", out int raxVal) && raxVal > 0 ? "RAX" : "R0");

                switch (op)
                {
                    case "MOV":
                        if (tokens.Length >= 3)
                        {
                            int val = int.TryParse(tokens[2], out int num) ? num : (regs.TryGetValue(tokens[2], out int rVal) ? rVal : 0);
                            regs[target] = val;
                            logs.Add($"  [MOV] Set {target} = {val}");
                        }
                        break;

                    case "ADD":
                        if (tokens.Length >= 3)
                        {
                            int val = int.TryParse(tokens[2], out int num) ? num : (regs.TryGetValue(tokens[2], out int rVal) ? rVal : 0);
                            regs[target] = regs.GetValueOrDefault(target) + val;
                            logs.Add($"  [ADD] {target} is now {regs[target]}");
                        }
                        break;

                    case "FIRE":
                    case "OUT":
                    case "ATTACK":
                        int dmg = regs.GetValueOrDefault(target);
                        if (dmg <= 0 && int.TryParse(target, out int directDmg)) dmg = directDmg;
                        currentHp = Math.Max(0, currentHp - dmg);
                        logs.Add($"  💥 [FIRE] Robot unleashed laser strike using {target}! Dealt {dmg} damage to Malware Drone! (HP: {currentHp})");
                        break;
                }

                if (currentHp <= 0) break;
            }

            bool victory = currentHp <= 0;
            if (victory)
            {
                logs.Add($"🎉 MALWARE DRONE DESTROYED in {instructionsExecuted} instructions! Memory Sector Cleansed! (+100 XP)");
            }
            else
            {
                logs.Add($"⚠️ Drone survived with {currentHp} HP. Optimize your assembly weapon registers!");
            }

            return (victory, currentHp, logs);
        }

        // =========================================================================
        // 7. GAME 7: WEBCRAFT (CSS FLEXBOX & GRID RESCUER)
        // =========================================================================
        public static (bool Success, string Feedback) EvaluateWebcraftCss(string cssCode, int level)
        {
            string norm = cssCode.ToLowerInvariant().Replace(" ", "").Replace("\n", "").Replace("\r", "");

            switch (level)
            {
                case 1: // Rescue 1: Center Astronaut (Flexbox justify & align)
                    bool hasFlex = norm.Contains("display:flex");
                    bool hasJustify = norm.Contains("justify-content:center");
                    bool hasAlign = norm.Contains("align-items:center");
                    if (hasFlex && hasJustify && hasAlign)
                    {
                        return (true, "🚀 RESCUE 1 COMPLETE! The astronaut was centered perfectly in the lunar module using Flexbox! (+50 XP)");
                    }
                    return (false, "Module misalignment! Required: display: flex; justify-content: center; align-items: center;");

                case 2: // Rescue 2: Space Station Cargo Bay (CSS Grid 3-Column)
                    bool hasGrid = norm.Contains("display:grid");
                    bool hasCols = norm.Contains("grid-template-columns:repeat(3,1fr)") || norm.Contains("grid-template-columns:1fr1fr1fr") || norm.Contains("grid-template-columns:33%33%33%");
                    if (hasGrid && hasCols)
                    {
                        return (true, "🛸 RESCUE 2 COMPLETE! Cargo bay compartments aligned into a 3-column responsive grid! (+50 XP)");
                    }
                    return (false, "Cargo bay irregular! Required: display: grid; grid-template-columns: repeat(3, 1fr);");

                default:
                    return (true, "Layout looks valid!");
            }
        }

        // =========================================================================
        // 8. GAME 8: CTF ETHICAL HACKING ARENA
        // =========================================================================
        public static (bool Correct, string Feedback) EvaluateCtfFlag(int challengeId, string submittedAnswer)
        {
            string answer = submittedAnswer.Trim();

            switch (challengeId)
            {
                case 1: // Base64 Secret Flag: QkhBVkFOSV9DVEZfU0VDVVJF
                    if (answer.Equals("BHAVANI_CTF_SECURE", StringComparison.OrdinalIgnoreCase))
                    {
                        return (true, "🚩 FLAG CAPTURED! You decoded the Base64 cipher successfully! (+100 XP)");
                    }
                    return (false, "Incorrect flag. Hint: Decode the Base64 string 'QkhBVkFOSV9DVEZfU0VDVVJF'.");

                case 2: // Hex Memory Inspection: 0xDEADBEEF
                    if (answer.Equals("DEADBEEF", StringComparison.OrdinalIgnoreCase) || answer.Equals("0xDEADBEEF", StringComparison.OrdinalIgnoreCase))
                    {
                        return (true, "🚩 FLAG CAPTURED! Memory buffer overflow canary located! (+100 XP)");
                    }
                    return (false, "Incorrect canary. Inspect the memory dump for the classic 32-bit magic marker.");

                case 3: // SQLi Patching
                    if (answer.Contains("@") || answer.Contains("Parameters.Add", StringComparison.OrdinalIgnoreCase) || answer.Contains("PreparedStatement", StringComparison.OrdinalIgnoreCase))
                    {
                        return (true, "🛡️ VULNERABILITY PATCHED! Parameterized query prevents SQL injection attacks! (+100 XP)");
                    }
                    return (false, "Patch failed. To secure dynamic SQL, use parameterized queries (@param) rather than raw string concatenation.");

                default:
                    return (false, "Unknown challenge.");
            }
        }
    }
}
