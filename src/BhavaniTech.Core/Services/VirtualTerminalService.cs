using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public class VirtualFile
    {
        public string Name { get; set; } = "";
        public bool IsDirectory { get; set; }
        public string Content { get; set; } = "";
        public string Permissions { get; set; } = "rwxr-xr-x";
        public string Owner { get; set; } = "root";
        public Dictionary<string, VirtualFile> Children { get; } = new();

        public VirtualFile(string name, bool isDir, string owner = "root", string perms = "rwxr-xr-x", string content = "")
        {
            Name = name;
            IsDirectory = isDir;
            Owner = owner;
            Permissions = perms;
            Content = content;
        }
    }

    public record TerminalCommandResult(
        string Output,
        string CurrentPath,
        string CurrentUser,
        bool FlagDiscovered = false,
        string? FlagValue = null,
        int XpEarned = 0
    );

    public class VirtualTerminalSession
    {
        public string CurrentUser { get; private set; } = "student";
        public string CurrentPath { get; private set; } = "/home/student";
        private readonly VirtualFile _root;
        private readonly HashSet<string> _discoveredFlags = new();

        public VirtualTerminalSession()
        {
            _root = BuildDefaultFileSystem();
        }

        private static VirtualFile BuildDefaultFileSystem()
        {
            var root = new VirtualFile("", true, "root", "rwxr-xr-x");

            // /bin
            var bin = new VirtualFile("bin", true);
            bin.Children["ls"] = new VirtualFile("ls", false, "root", "rwxr-xr-x", "[ELF 64-bit LSB executable, x86-64, version 1 (SYSV)]");
            bin.Children["cat"] = new VirtualFile("cat", false, "root", "rwxr-xr-x", "[ELF 64-bit LSB executable, x86-64, version 1 (SYSV)]");
            bin.Children["grep"] = new VirtualFile("grep", false, "root", "rwxr-xr-x", "[ELF 64-bit LSB executable, x86-64, version 1 (SYSV)]");
            bin.Children["strings"] = new VirtualFile("strings", false, "root", "rwxr-xr-x", "[ELF 64-bit LSB executable, x86-64, version 1 (SYSV)]");
            bin.Children["chmod"] = new VirtualFile("chmod", false, "root", "rwxr-xr-x", "[ELF 64-bit LSB executable, x86-64, version 1 (SYSV)]");
            bin.Children["su"] = new VirtualFile("su", false, "root", "rwsr-xr-x", "[SUID binary for switching user]");
            root.Children["bin"] = bin;

            // /etc
            var etc = new VirtualFile("etc", true);
            etc.Children["hostname"] = new VirtualFile("hostname", false, "root", "rw-r--r--", "bhavani-box\n");
            etc.Children["passwd"] = new VirtualFile("passwd", false, "root", "rw-r--r--",
                "root:x:0:0:root:/root:/bin/bash\n" +
                "daemon:x:1:1:daemon:/usr/sbin:/usr/sbin/nologin\n" +
                "student:x:1000:1000:Bhavani Student,,,:/home/student:/bin/bash\n");
            etc.Children["shadow"] = new VirtualFile("shadow", false, "root", "rw-------",
                "root:$6$bhavaniSalt$Z7xQ8y...HASH:19245:0:99999:7:::\n" +
                "student:$6$userSalt$Ab12Cd...HASH:19245:0:99999:7:::\n");
            root.Children["etc"] = etc;

            // /var/log
            var varDir = new VirtualFile("var", true);
            var logDir = new VirtualFile("log", true);
            logDir.Children["auth.log"] = new VirtualFile("auth.log", false, "root", "rw-r--r--",
                "Sep 05 08:01:10 bhavani-box sshd[1042]: Accepted password for student from 192.168.1.5 port 52310 ssh2\n" +
                "Sep 05 08:14:22 bhavani-box sudo: student : TTY=pts/0 ; PWD=/home/student ; USER=root ; COMMAND=/bin/ls\n" +
                "Sep 05 08:33:01 bhavani-box internal-daemon: ALERT secret leak in memory block: FLAG{Log_Forensics_Master_4401}\n" +
                "Sep 05 08:45:00 bhavani-box cron[198]: (CRON) STARTUP\n");
            varDir.Children["log"] = logDir;
            root.Children["var"] = varDir;

            // /home/student
            var home = new VirtualFile("home", true);
            var studentHome = new VirtualFile("student", true, "student", "rwxr-xr-x");
            studentHome.Children["readme.txt"] = new VirtualFile("readme.txt", false, "student", "rw-r--r--",
                "Welcome to Bhavani Linux CTF Arena!\n" +
                "Your mission:\n" +
                "1. Find hidden dotfiles with 'ls -a'\n" +
                "2. Search through /var/log/auth.log with 'grep'\n" +
                "3. Inspect binary strings in ./challenge_bin\n" +
                "4. Escalate to root using 'su' (Password: bhavani_root) and read /root/flag.txt!\n");
            studentHome.Children[".hidden_flag"] = new VirtualFile(".hidden_flag", false, "student", "rw-r--r--",
                "FLAG{Hidden_Dotfile_Explorer_1120}\n");
            studentHome.Children["challenge_bin"] = new VirtualFile("challenge_bin", false, "student", "rwxr-xr-x",
                "ELF_HEADER\0PADDING\0SomeGarbageOpcode\0FLAG{Strings_Binary_Extract_7733}\0MoreGarbageOpcodes\0END_OF_BINARY");
            home.Children["student"] = studentHome;
            root.Children["home"] = home;

            // /root
            var rootHome = new VirtualFile("root", true, "root", "rwx------");
            rootHome.Children["flag.txt"] = new VirtualFile("flag.txt", false, "root", "rw-------",
                "CONGRATULATIONS! You have attained root administrator privilege on BhavaniBox.\n" +
                "ROOT FLAG: FLAG{Linux_Kernel_Godmode_9999}\n");
            root.Children["root"] = rootHome;

            // /tmp
            var tmp = new VirtualFile("tmp", true, "root", "rwxrwxrwt");
            tmp.Children["scratch.txt"] = new VirtualFile("scratch.txt", false, "student", "rw-rw-rw-", "Temporary scratchpad.\n");
            root.Children["tmp"] = tmp;

            return root;
        }

        public TerminalCommandResult Execute(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return new TerminalCommandResult("", CurrentPath, CurrentUser);

            var tokens = commandLine.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return new TerminalCommandResult("", CurrentPath, CurrentUser);

            string cmd = tokens[0].ToLowerInvariant();
            var args = tokens.Skip(1).ToList();

            switch (cmd)
            {
                case "help":
                    return new TerminalCommandResult(
                        "Bhavani Linux Shell Commands:\n" +
                        "  ls [-a|-l] [path]  - List directory contents (use -a for hidden files)\n" +
                        "  cd [path]          - Change working directory (.. for parent, ~ for home)\n" +
                        "  pwd                - Print current working directory\n" +
                        "  cat <file>         - Display file contents\n" +
                        "  grep <term> <file> - Search for pattern inside a file\n" +
                        "  strings <file>     - Extract printable text strings from binary file\n" +
                        "  chmod <mode> <file>- Modify file permissions (e.g. 755, 777)\n" +
                        "  whoami             - Display active user account name\n" +
                        "  id                 - Display UID, GID, and security groups\n" +
                        "  su [root]          - Switch user (Try 'su root' with password 'bhavani_root')\n" +
                        "  clear              - Clear terminal console screen\n" +
                        "  echo <text>        - Print text string\n",
                        CurrentPath, CurrentUser
                    );

                case "pwd":
                    return new TerminalCommandResult(CurrentPath, CurrentPath, CurrentUser);

                case "whoami":
                    return new TerminalCommandResult(CurrentUser, CurrentPath, CurrentUser);

                case "id":
                    if (CurrentUser == "root")
                        return new TerminalCommandResult("uid=0(root) gid=0(root) groups=0(root)", CurrentPath, CurrentUser);
                    return new TerminalCommandResult("uid=1000(student) gid=1000(student) groups=1000(student),4(adm),27(sudo)", CurrentPath, CurrentUser);

                case "clear":
                    return new TerminalCommandResult("__CLEAR__", CurrentPath, CurrentUser);

                case "echo":
                    return new TerminalCommandResult(string.Join(" ", args), CurrentPath, CurrentUser);

                case "cd":
                    return HandleCd(args.Count > 0 ? args[0] : "~");

                case "ls":
                    return HandleLs(args);

                case "cat":
                    if (args.Count == 0) return new TerminalCommandResult("cat: missing operand", CurrentPath, CurrentUser);
                    return HandleCat(args[0]);

                case "strings":
                    if (args.Count == 0) return new TerminalCommandResult("strings: missing operand", CurrentPath, CurrentUser);
                    return HandleStrings(args[0]);

                case "grep":
                    if (args.Count < 2) return new TerminalCommandResult("grep: usage: grep <pattern> <file>", CurrentPath, CurrentUser);
                    return HandleGrep(args[0], args[1]);

                case "chmod":
                    if (args.Count < 2) return new TerminalCommandResult("chmod: usage: chmod <mode> <file>", CurrentPath, CurrentUser);
                    return HandleChmod(args[0], args[1]);

                case "su":
                    return HandleSu(args);

                default:
                    return new TerminalCommandResult($"bash: {cmd}: command not found. Type 'help' for valid commands.", CurrentPath, CurrentUser);
            }
        }

        private TerminalCommandResult HandleCd(string target)
        {
            string newPath = ResolvePath(target);
            var node = TraverseTo(newPath);

            if (node == null)
                return new TerminalCommandResult($"bash: cd: {target}: No such file or directory", CurrentPath, CurrentUser);

            if (!node.IsDirectory)
                return new TerminalCommandResult($"bash: cd: {target}: Not a directory", CurrentPath, CurrentUser);

            // Permission check
            if (node.Owner == "root" && CurrentUser != "root" && !node.Permissions.Contains("r-x") && !node.Permissions.Contains("rwx"))
            {
                return new TerminalCommandResult($"bash: cd: {target}: Permission denied", CurrentPath, CurrentUser);
            }

            CurrentPath = newPath;
            return new TerminalCommandResult("", CurrentPath, CurrentUser);
        }

        private TerminalCommandResult HandleLs(List<string> args)
        {
            bool showAll = args.Contains("-a") || args.Contains("-la") || args.Contains("-al");
            bool longFormat = args.Contains("-l") || args.Contains("-la") || args.Contains("-al");

            string targetArg = args.FirstOrDefault(a => !a.StartsWith("-")) ?? ".";
            string targetPath = ResolvePath(targetArg);
            var node = TraverseTo(targetPath);

            if (node == null)
                return new TerminalCommandResult($"ls: cannot access '{targetArg}': No such file or directory", CurrentPath, CurrentUser);

            if (!node.IsDirectory)
                return new TerminalCommandResult(node.Name, CurrentPath, CurrentUser);

            var sb = new StringBuilder();
            var entries = node.Children.Values
                .Where(c => showAll || !c.Name.StartsWith("."))
                .OrderBy(c => c.Name)
                .ToList();

            if (longFormat)
            {
                sb.AppendLine($"total {entries.Count}");
                foreach (var e in entries)
                {
                    string type = e.IsDirectory ? "d" : "-";
                    sb.AppendLine($"{type}{e.Permissions} 1 {e.Owner} {e.Owner} {e.Content.Length,5} Sep 05 10:00 {e.Name}");
                }
            }
            else
            {
                var names = entries.Select(e => e.IsDirectory ? e.Name + "/" : e.Name);
                sb.Append(string.Join("  ", names));
            }

            return new TerminalCommandResult(sb.ToString().TrimEnd(), CurrentPath, CurrentUser);
        }

        private TerminalCommandResult HandleCat(string target)
        {
            string targetPath = ResolvePath(target);
            var node = TraverseTo(targetPath);

            if (node == null)
                return new TerminalCommandResult($"cat: {target}: No such file or directory", CurrentPath, CurrentUser);

            if (node.IsDirectory)
                return new TerminalCommandResult($"cat: {target}: Is a directory", CurrentPath, CurrentUser);

            // Check permissions
            if (node.Owner == "root" && CurrentUser != "root" && node.Permissions == "rw-------")
            {
                return new TerminalCommandResult($"cat: {target}: Permission denied (Requires root privilege)", CurrentPath, CurrentUser);
            }

            // Flag detection
            bool foundFlag = false;
            string? flagVal = null;
            int xp = 0;

            if (node.Content.Contains("FLAG{"))
            {
                int start = node.Content.IndexOf("FLAG{");
                int end = node.Content.IndexOf("}", start);
                if (end > start)
                {
                    flagVal = node.Content.Substring(start, end - start + 1);
                    if (!_discoveredFlags.Contains(flagVal))
                    {
                        _discoveredFlags.Add(flagVal);
                        foundFlag = true;
                        xp = flagVal.Contains("Godmode") ? 250 : 100;
                    }
                }
            }

            return new TerminalCommandResult(node.Content.TrimEnd(), CurrentPath, CurrentUser, foundFlag, flagVal, xp);
        }

        private TerminalCommandResult HandleStrings(string target)
        {
            string targetPath = ResolvePath(target);
            var node = TraverseTo(targetPath);

            if (node == null)
                return new TerminalCommandResult($"strings: '{target}': No such file", CurrentPath, CurrentUser);

            var sb = new StringBuilder();
            var rawStrings = node.Content.Split(new[] { '\0', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            bool foundFlag = false;
            string? flagVal = null;
            int xp = 0;

            foreach (var s in rawStrings)
            {
                if (s.Length >= 4 && s.All(c => !char.IsControl(c)))
                {
                    sb.AppendLine(s);
                    if (s.Contains("FLAG{") && !_discoveredFlags.Contains(s))
                    {
                        int start = s.IndexOf("FLAG{");
                        int end = s.IndexOf("}", start);
                        if (end > start)
                        {
                            flagVal = s.Substring(start, end - start + 1);
                            _discoveredFlags.Add(flagVal);
                            foundFlag = true;
                            xp = 150;
                        }
                    }
                }
            }

            return new TerminalCommandResult(sb.ToString().TrimEnd(), CurrentPath, CurrentUser, foundFlag, flagVal, xp);
        }

        private TerminalCommandResult HandleGrep(string pattern, string target)
        {
            string targetPath = ResolvePath(target);
            var node = TraverseTo(targetPath);

            if (node == null)
                return new TerminalCommandResult($"grep: {target}: No such file or directory", CurrentPath, CurrentUser);

            var lines = node.Content.Split('\n');
            var matches = lines.Where(l => l.Contains(pattern, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matches.Count == 0)
                return new TerminalCommandResult("", CurrentPath, CurrentUser);

            bool foundFlag = false;
            string? flagVal = null;
            int xp = 0;

            string resultText = string.Join("\n", matches);
            if (resultText.Contains("FLAG{"))
            {
                int start = resultText.IndexOf("FLAG{");
                int end = resultText.IndexOf("}", start);
                if (end > start)
                {
                    flagVal = resultText.Substring(start, end - start + 1);
                    if (!_discoveredFlags.Contains(flagVal))
                    {
                        _discoveredFlags.Add(flagVal);
                        foundFlag = true;
                        xp = 150;
                    }
                }
            }

            return new TerminalCommandResult(resultText, CurrentPath, CurrentUser, foundFlag, flagVal, xp);
        }

        private TerminalCommandResult HandleChmod(string mode, string target)
        {
            string targetPath = ResolvePath(target);
            var node = TraverseTo(targetPath);

            if (node == null)
                return new TerminalCommandResult($"chmod: cannot access '{target}': No such file or directory", CurrentPath, CurrentUser);

            if (CurrentUser != "root" && node.Owner != CurrentUser)
                return new TerminalCommandResult($"chmod: changing permissions of '{target}': Operation not permitted", CurrentPath, CurrentUser);

            if (mode == "777") node.Permissions = "rwxrwxrwx";
            else if (mode == "755") node.Permissions = "rwxr-xr-x";
            else if (mode == "644") node.Permissions = "rw-r--r--";
            else if (mode == "600") node.Permissions = "rw-------";

            return new TerminalCommandResult("", CurrentPath, CurrentUser);
        }

        private TerminalCommandResult HandleSu(List<string> args)
        {
            string targetUser = args.Count > 0 ? args[0] : "root";
            string providedPass = args.Count > 1 ? args[1] : "";

            if (targetUser == "student")
            {
                CurrentUser = "student";
                CurrentPath = "/home/student";
                return new TerminalCommandResult("Switched to user student.", CurrentPath, CurrentUser);
            }

            if (targetUser == "root")
            {
                if (providedPass == "bhavani_root" || args.Count == 1 && args[0] == "bhavani_root")
                {
                    CurrentUser = "root";
                    CurrentPath = "/root";
                    return new TerminalCommandResult("Authentication SUCCESS! You are now ROOT administrator.\nCheck /root/flag.txt for the godmode flag!", CurrentPath, CurrentUser);
                }

                return new TerminalCommandResult("su: Authentication failure. Usage: 'su root bhavani_root'", CurrentPath, CurrentUser);
            }

            return new TerminalCommandResult($"su: user {targetUser} does not exist", CurrentPath, CurrentUser);
        }

        private string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path) || path == ".") return CurrentPath;
            if (path == "~") return CurrentUser == "root" ? "/root" : "/home/student";

            if (path.StartsWith("/"))
            {
                return NormalizePath(path);
            }

            return NormalizePath(CurrentPath + "/" + path);
        }

        private static string NormalizePath(string path)
        {
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var stack = new List<string>();

            foreach (var seg in segments)
            {
                if (seg == ".") continue;
                if (seg == "..")
                {
                    if (stack.Count > 0) stack.RemoveAt(stack.Count - 1);
                }
                else
                {
                    stack.Add(seg);
                }
            }

            return "/" + string.Join("/", stack);
        }

        private VirtualFile? TraverseTo(string normalizedPath)
        {
            if (normalizedPath == "/" || string.IsNullOrEmpty(normalizedPath)) return _root;

            var parts = normalizedPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var curr = _root;

            foreach (var part in parts)
            {
                if (!curr.IsDirectory || !curr.Children.ContainsKey(part))
                    return null;
                curr = curr.Children[part];
            }

            return curr;
        }
    }
}
