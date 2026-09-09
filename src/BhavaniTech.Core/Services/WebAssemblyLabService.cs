using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record WasmStackFrame(
        int StepNumber,
        string Instruction,
        string Operand,
        List<int> StackSnapshot,
        string Description
    );

    public record WasmBinarySection(
        byte SectionId,
        string SectionName,
        int SizeBytes,
        string HexDump,
        string Details
    );

    public record WasmExecutionResult(
        int ReturnValue,
        List<WasmStackFrame> Trace,
        byte[] MemoryBytes,
        string OutputLog,
        bool IsSuccess
    );

    public class WebAssemblyLabService
    {
        private readonly byte[] _linearMemory = new byte[65536]; // 1 Page = 64 KB

        public WebAssemblyLabService()
        {
            // Initialize sample string in linear memory at offset 1024: "BhavaniTech WASM v1.0"
            WriteString(1024, "BhavaniTech WASM v1.0");
        }

        public void WriteByte(int address, byte val)
        {
            if (address >= 0 && address < _linearMemory.Length)
                _linearMemory[address] = val;
        }

        public byte ReadByte(int address)
        {
            return (address >= 0 && address < _linearMemory.Length) ? _linearMemory[address] : (byte)0;
        }

        public void WriteString(int address, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            for (int i = 0; i < bytes.Length && address + i < _linearMemory.Length; i++)
            {
                _linearMemory[address + i] = bytes[i];
            }
        }

        public string ReadString(int address, int maxLen = 64)
        {
            var bytes = new List<byte>();
            for (int i = 0; i < maxLen && address + i < _linearMemory.Length; i++)
            {
                byte b = _linearMemory[address + i];
                if (b == 0) break;
                bytes.Add(b);
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public string GetMemoryHexDump(int startAddress, int length = 64)
        {
            var sb = new StringBuilder();
            sb.AppendLine("OFFSET     00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F  ASCII");
            sb.AppendLine("------------------------------------------------------------------");

            int end = Math.Min(_linearMemory.Length, startAddress + length);
            for (int row = startAddress; row < end; row += 16)
            {
                sb.Append($"{row:X8}   ");
                var ascii = new StringBuilder();

                for (int col = 0; col < 16; col++)
                {
                    int addr = row + col;
                    if (addr < end)
                    {
                        byte b = _linearMemory[addr];
                        sb.Append($"{b:X2} ");
                        ascii.Append(b >= 32 && b <= 126 ? (char)b : '.');
                    }
                    else
                    {
                        sb.Append("   ");
                    }
                    if (col == 7) sb.Append(" ");
                }

                sb.AppendLine($" |{ascii}|");
            }
            return sb.ToString();
        }

        public List<WasmBinarySection> DisassembleToBinarySections(string watCode)
        {
            // Parse WAT into simulated WASM binary module structure
            var sections = new List<WasmBinarySection>
            {
                new(0x00, "Magic & Version Header", 8, "00 61 73 6D 01 00 00 00", "Preamble: '\\0asm' (0x6D736100) + WASM Binary Version 1 (0x00000001)"),
                new(0x01, "Type Section", 7, "01 05 01 60 02 7F 7F 01 7F", "Function signatures: (i32, i32) -> (i32)"),
                new(0x03, "Function Section", 2, "03 02 01 00", "Declares function index 0 references type index 0"),
                new(0x05, "Memory Section", 3, "05 03 01 00 01", "Allocates 1 page (64 KB) minimum linear memory, no max"),
                new(0x07, "Export Section", 9, "07 07 01 03 61 64 64 00 00", "Exports function 0 with external name 'exported_fn'"),
                new(0x0A, "Code Section", 14, "0A 0C 01 0A 00 20 00 20 01 6A 0B", "Function body bytecode: local.get 0 (0x20 00), local.get 1 (0x20 01), i32.add (0x6A), end (0x0B)")
            };
            return sections;
        }

        public WasmExecutionResult ExecuteWatScript(string watOrInstructions, Dictionary<string, int>? parameters = null)
        {
            var stack = new Stack<int>();
            var trace = new List<WasmStackFrame>();
            var locals = new Dictionary<string, int>(parameters ?? new Dictionary<string, int>());
            var sbLog = new StringBuilder();

            int step = 1;
            var lines = watOrInstructions.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var rawLine in lines)
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith(";;") || line.StartsWith("(") && !line.StartsWith("(i32.") && !line.StartsWith("(local."))
                        continue;

                    // Remove enclosing or trailing parentheses if present e.g. "(i32.const 42)" or "i32.add))"
                    while (line.StartsWith("(") && line.EndsWith(")"))
                        line = line[1..^1].Trim();
                    line = line.TrimEnd(')').Trim();

                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;

                    string op = parts[0].TrimEnd(')').ToLowerInvariant();
                    string arg = parts.Length > 1 ? parts[1].TrimEnd(')') : "";

                    string desc = "";

                    switch (op)
                    {
                        case "i32.const":
                            if (int.TryParse(arg, out int constVal))
                            {
                                stack.Push(constVal);
                                desc = $"Pushed constant {constVal} onto operand stack";
                            }
                            break;

                        case "local.get":
                            int localVal = locals.GetValueOrDefault(arg, 0);
                            stack.Push(localVal);
                            desc = $"Loaded local variable '{arg}' = {localVal} onto stack";
                            break;

                        case "local.set":
                            if (stack.Count > 0)
                            {
                                int val = stack.Pop();
                                locals[arg] = val;
                                desc = $"Popped {val} and stored into local variable '{arg}'";
                            }
                            break;

                        case "i32.add":
                            if (stack.Count >= 2)
                            {
                                int b = stack.Pop();
                                int a = stack.Pop();
                                int res = a + b;
                                stack.Push(res);
                                desc = $"Popped {b} and {a}, computed {a} + {b} = {res}, pushed {res}";
                            }
                            break;

                        case "i32.sub":
                            if (stack.Count >= 2)
                            {
                                int b = stack.Pop();
                                int a = stack.Pop();
                                int res = a - b;
                                stack.Push(res);
                                desc = $"Popped {b} and {a}, computed {a} - {b} = {res}, pushed {res}";
                            }
                            break;

                        case "i32.mul":
                            if (stack.Count >= 2)
                            {
                                int b = stack.Pop();
                                int a = stack.Pop();
                                int res = a * b;
                                stack.Push(res);
                                desc = $"Popped {b} and {a}, computed {a} * {b} = {res}, pushed {res}";
                            }
                            break;

                        case "i32.div_s":
                            if (stack.Count >= 2)
                            {
                                int b = stack.Pop();
                                int a = stack.Pop();
                                int res = b != 0 ? a / b : 0;
                                stack.Push(res);
                                desc = $"Popped {b} and {a}, computed integer divide {a} / {b} = {res}";
                            }
                            break;

                        case "i32.eqz":
                            if (stack.Count >= 1)
                            {
                                int a = stack.Pop();
                                int res = a == 0 ? 1 : 0;
                                stack.Push(res);
                                desc = $"Tested if {a} == 0 -> pushed {res}";
                            }
                            break;

                        case "i32.lt_s":
                            if (stack.Count >= 2)
                            {
                                int b = stack.Pop();
                                int a = stack.Pop();
                                int res = a < b ? 1 : 0;
                                stack.Push(res);
                                desc = $"Comparison: {a} < {b} -> pushed {res}";
                            }
                            break;

                        case "drop":
                            if (stack.Count > 0)
                            {
                                int dropped = stack.Pop();
                                desc = $"Dropped top operand {dropped}";
                            }
                            break;

                        case "i32.store8":
                            // Address is top-1, byte is top
                            if (stack.Count >= 2)
                            {
                                int byteVal = stack.Pop();
                                int addr = stack.Pop();
                                WriteByte(addr, (byte)byteVal);
                                desc = $"Stored byte 0x{byteVal:X2} into linear memory address 0x{addr:X4}";
                            }
                            break;

                        case "i32.load8_u":
                            if (stack.Count >= 1)
                            {
                                int addr = stack.Pop();
                                byte b = ReadByte(addr);
                                stack.Push(b);
                                desc = $"Loaded byte 0x{b:X2} ({b}) from memory address 0x{addr:X4} onto stack";
                            }
                            break;
                    }

                    trace.Add(new WasmStackFrame(step++, op, arg, stack.Reverse().ToList(), desc));
                }

                int ret = stack.Count > 0 ? stack.Peek() : 0;
                sbLog.AppendLine($"✅ Execution completed successfully. Stack frames evaluated: {trace.Count}. Final return: {ret}");
                return new WasmExecutionResult(ret, trace, _linearMemory, sbLog.ToString(), true);
            }
            catch (Exception ex)
            {
                sbLog.AppendLine($"❌ WASM Trap: {ex.Message}");
                return new WasmExecutionResult(0, trace, _linearMemory, sbLog.ToString(), false);
            }
        }
    }
}
