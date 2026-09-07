using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace BhavaniTech.Core.Services
{
    public record SqlExecutionResult(
        bool Success,
        int RowsAffected,
        List<string> ColumnNames,
        List<List<string>> Rows,
        string FormattedTable,
        double ElapsedMs,
        string? ErrorMessage = null
    );

    public record JavaScriptExecutionResult(
        bool Success,
        string OutputConsole,
        string ReturnedValue,
        double ElapsedMs
    );

    public static class PolyglotExecutionService
    {
        // =====================================================================
        // IN-MEMORY SQL DATABASE RUNNER (SQLite)
        // =====================================================================
        public static SqlExecutionResult ExecuteSql(string sqlScript)
        {
            if (string.IsNullOrWhiteSpace(sqlScript))
                return new SqlExecutionResult(true, 0, new(), new(), "No SQL statements to execute.", 0);

            var sw = Stopwatch.StartNew();
            try
            {
                using var conn = new SqliteConnection("Data Source=:memory:;Mode=Memory;Cache=Shared");
                conn.Open();

                // Split multiple statements
                var statements = sqlScript.Split(';', StringSplitOptions.RemoveEmptyEntries);
                List<string> columns = new();
                List<List<string>> rows = new();
                int totalRowsAffected = 0;

                foreach (var rawStmt in statements)
                {
                    string stmt = rawStmt.Trim();
                    if (string.IsNullOrWhiteSpace(stmt)) continue;

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = stmt;

                    if (stmt.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                        stmt.StartsWith("PRAGMA", StringComparison.OrdinalIgnoreCase) ||
                        stmt.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
                    {
                        columns.Clear();
                        rows.Clear();
                        using var reader = cmd.ExecuteReader();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i));
                        }

                        while (reader.Read())
                        {
                            var row = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)?.ToString() ?? "");
                            }
                            rows.Add(row);
                        }
                    }
                    else
                    {
                        totalRowsAffected += cmd.ExecuteNonQuery();
                    }
                }

                sw.Stop();

                // Format Table
                string formatted = FormatAsciiTable(columns, rows, totalRowsAffected);
                return new SqlExecutionResult(true, totalRowsAffected, columns, rows, formatted, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new SqlExecutionResult(false, 0, new(), new(), $"SQL ERROR: {ex.Message}", sw.Elapsed.TotalMilliseconds, ex.Message);
            }
        }

        private static string FormatAsciiTable(List<string> columns, List<List<string>> rows, int rowsAffected)
        {
            if (columns.Count == 0 && rows.Count == 0)
            {
                return $"Query executed successfully. Rows affected: {rowsAffected}";
            }

            // Calculate widths
            var widths = new int[columns.Count];
            for (int i = 0; i < columns.Count; i++) widths[i] = columns[i].Length;

            foreach (var r in rows)
            {
                for (int i = 0; i < r.Count && i < widths.Length; i++)
                {
                    if (r[i].Length > widths[i]) widths[i] = r[i].Length;
                }
            }

            var sb = new StringBuilder();
            // Header separator
            string sep = "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";
            sb.AppendLine(sep);

            // Column names
            var headerRow = new List<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                headerRow.Add($" {columns[i].PadRight(widths[i])} ");
            }
            sb.AppendLine("|" + string.Join("|", headerRow) + "|");
            sb.AppendLine(sep);

            // Data rows
            foreach (var r in rows)
            {
                var rowCells = new List<string>();
                for (int i = 0; i < widths.Length; i++)
                {
                    string val = i < r.Count ? r[i] : "";
                    rowCells.Add($" {val.PadRight(widths[i])} ");
                }
                sb.AppendLine("|" + string.Join("|", rowCells) + "|");
            }
            sb.AppendLine(sep);
            sb.AppendLine($"({rows.Count} rows returned)");

            return sb.ToString();
        }

        // =====================================================================
        // LIGHTWEIGHT IN-MEMORY JAVASCRIPT EMULATOR
        // =====================================================================
        public static JavaScriptExecutionResult ExecuteJavaScript(string jsCode)
        {
            var sw = Stopwatch.StartNew();
            var consoleOutput = new StringBuilder();
            string returnedVal = "undefined";

            try
            {
                var lines = jsCode.Split(new[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries);
                var variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("//")) continue;

                    // console.log(...)
                    var logMatch = Regex.Match(trimmed, @"console\.log\((.*)\);?");
                    if (logMatch.Success)
                    {
                        string arg = logMatch.Groups[1].Value.Trim();
                        string evalArg = EvaluateSimpleExpression(arg, variables);
                        consoleOutput.AppendLine(evalArg);
                        continue;
                    }

                    // Variable declaration: let/const/var name = expression;
                    var varMatch = Regex.Match(trimmed, @"(?:let|const|var)\s+([a-zA-Z_]\w*)\s*=\s*(.+?);?$");
                    if (varMatch.Success)
                    {
                        string varName = varMatch.Groups[1].Value;
                        string expr = varMatch.Groups[2].Value;
                        string val = EvaluateSimpleExpression(expr, variables);
                        variables[varName] = val;
                        returnedVal = val;
                        continue;
                    }

                    // Variable assignment: name = expression;
                    var assignMatch = Regex.Match(trimmed, @"^([a-zA-Z_]\w*)\s*=\s*(.+?);?$");
                    if (assignMatch.Success && !trimmed.StartsWith("return"))
                    {
                        string varName = assignMatch.Groups[1].Value;
                        string expr = assignMatch.Groups[2].Value;
                        string val = EvaluateSimpleExpression(expr, variables);
                        variables[varName] = val;
                        returnedVal = val;
                        continue;
                    }

                    // return expression;
                    var retMatch = Regex.Match(trimmed, @"return\s+(.+?);?$");
                    if (retMatch.Success)
                    {
                        returnedVal = EvaluateSimpleExpression(retMatch.Groups[1].Value, variables);
                    }
                }

                sw.Stop();
                return new JavaScriptExecutionResult(true, consoleOutput.ToString(), returnedVal, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new JavaScriptExecutionResult(false, $"JS Runtime Error: {ex.Message}", "Error", sw.Elapsed.TotalMilliseconds);
            }
        }

        private static string EvaluateSimpleExpression(string expr, Dictionary<string, object> vars)
        {
            expr = expr.Trim();
            if (expr.StartsWith("\"") && expr.EndsWith("\"") || expr.StartsWith("'") && expr.EndsWith("'"))
            {
                return expr.Length >= 2 ? expr.Substring(1, expr.Length - 2) : "";
            }

            if (vars.ContainsKey(expr))
            {
                return vars[expr]?.ToString() ?? "null";
            }

            // String concatenation: "Hello " + name (only if quotes are present)
            if (expr.Contains("+") && (expr.Contains("\"") || expr.Contains("'")))
            {
                var parts = expr.Split('+');
                var sb = new StringBuilder();
                foreach (var p in parts)
                {
                    sb.Append(EvaluateSimpleExpression(p.Trim(), vars));
                }
                return sb.ToString();
            }

            // Arithmetic: using DataTable Compute
            try
            {
                string evalExpr = expr;
                foreach (var kvp in vars)
                {
                    evalExpr = Regex.Replace(evalExpr, $@"\b{kvp.Key}\b", kvp.Value?.ToString() ?? "0");
                }
                var dt = new DataTable();
                var result = dt.Compute(evalExpr, "");
                return result?.ToString() ?? expr;
            }
            catch
            {
                return expr;
            }
        }
    }
}
