using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BhavaniTech.Core.Services
{
    public record CalculatorHistoryEntry(
        string Expression,
        string Result,
        DateTime Timestamp
    );

    public record CalculationResult(
        bool Success,
        double Value,
        string FormattedResult,
        string Expression,
        List<string> Steps,
        string? ErrorMessage = null
    );

    public class CalculatorEngine
    {
        private readonly List<CalculatorHistoryEntry> _history = new();
        private double _memoryStore = 0.0;

        public IReadOnlyList<CalculatorHistoryEntry> History => _history;
        public double MemoryStore => _memoryStore;

        public void MemoryClear() => _memoryStore = 0.0;
        public void MemoryAdd(double val) => _memoryStore += val;
        public void MemorySubtract(double val) => _memoryStore -= val;

        public CalculationResult Evaluate(string expression)
        {
            var steps = new List<string>();
            if (string.IsNullOrWhiteSpace(expression))
            {
                return new CalculationResult(false, 0, "0", "", steps, "Empty expression.");
            }

            try
            {
                steps.Add($"Raw Expression: {expression}");

                // Sanitize and replace display operators and constants
                string sanitized = expression
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace("−", "-")
                    .Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)
                    .Trim();

                steps.Add($"Normalized Math String: {sanitized}");

                // Dijkstra Shunting-Yard Algorithm to RPN & Evaluation
                double val = EvaluateShuntingYard(sanitized, steps);

                string formatted = val % 1 == 0 ? val.ToString("N0", CultureInfo.InvariantCulture) : val.ToString("G10", CultureInfo.InvariantCulture);
                steps.Add($"Evaluation Complete: {expression} = {formatted}");

                var historyEntry = new CalculatorHistoryEntry(expression, formatted, DateTime.Now);
                _history.Insert(0, historyEntry);
                if (_history.Count > 50) _history.RemoveAt(_history.Count - 1);

                return new CalculationResult(true, val, formatted, expression, steps);
            }
            catch (Exception ex)
            {
                steps.Add($"Evaluation Error: {ex.Message}");
                return new CalculationResult(false, 0, "Error", expression, steps, ex.Message);
            }
        }

        private double EvaluateShuntingYard(string expr, List<string> steps)
        {
            // Tokenize
            var tokens = Tokenize(expr);
            steps.Add($"Tokens: [{string.Join(", ", tokens)}]");

            var outputQueue = new Queue<string>();
            var opStack = new Stack<string>();

            // Functions recognized
            var functions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "sin", "cos", "tan", "sqrt", "cbrt", "log", "ln", "abs"
            };

            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    outputQueue.Enqueue(token);
                }
                else if (functions.Contains(token))
                {
                    opStack.Push(token);
                }
                else if (token == "(")
                {
                    opStack.Push(token);
                }
                else if (token == ")")
                {
                    while (opStack.Count > 0 && opStack.Peek() != "(")
                    {
                        outputQueue.Enqueue(opStack.Pop());
                    }
                    if (opStack.Count > 0 && opStack.Peek() == "(")
                    {
                        opStack.Pop(); // pop '('
                    }
                    if (opStack.Count > 0 && functions.Contains(opStack.Peek()))
                    {
                        outputQueue.Enqueue(opStack.Pop());
                    }
                }
                else if (IsOperator(token))
                {
                    while (opStack.Count > 0 && IsOperator(opStack.Peek()) &&
                           ((IsLeftAssociative(token) && Precedence(token) <= Precedence(opStack.Peek())) ||
                            (!IsLeftAssociative(token) && Precedence(token) < Precedence(opStack.Peek()))))
                    {
                        outputQueue.Enqueue(opStack.Pop());
                    }
                    opStack.Push(token);
                }
            }

            while (opStack.Count > 0)
            {
                outputQueue.Enqueue(opStack.Pop());
            }

            steps.Add($"RPN (Postfix): [{string.Join(" ", outputQueue)}]");

            // Evaluate RPN
            var evalStack = new Stack<double>();
            while (outputQueue.Count > 0)
            {
                string item = outputQueue.Dequeue();
                if (double.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
                {
                    evalStack.Push(num);
                }
                else if (functions.Contains(item))
                {
                    if (evalStack.Count < 1) throw new InvalidOperationException($"Function {item} missing argument.");
                    double arg = evalStack.Pop();
                    double res = item.ToLowerInvariant() switch
                    {
                        "sin" => Math.Sin(arg),
                        "cos" => Math.Cos(arg),
                        "tan" => Math.Tan(arg),
                        "sqrt" => arg < 0 ? throw new ArgumentException("Square root of negative number.") : Math.Sqrt(arg),
                        "cbrt" => Math.Cbrt(arg),
                        "log" => arg <= 0 ? throw new ArgumentException("Log of non-positive number.") : Math.Log10(arg),
                        "ln" => arg <= 0 ? throw new ArgumentException("Ln of non-positive number.") : Math.Log(arg),
                        "abs" => Math.Abs(arg),
                        _ => throw new InvalidOperationException($"Unknown function: {item}")
                    };
                    evalStack.Push(res);
                }
                else if (IsOperator(item))
                {
                    if (evalStack.Count < 2) throw new InvalidOperationException($"Operator {item} requires two operands.");
                    double b = evalStack.Pop();
                    double a = evalStack.Pop();
                    double res = item switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => b == 0 ? throw new DivideByZeroException("Division by zero.") : a / b,
                        "%" => a % b,
                        "^" => Math.Pow(a, b),
                        _ => throw new InvalidOperationException($"Unknown operator: {item}")
                    };
                    evalStack.Push(res);
                }
            }

            if (evalStack.Count != 1) throw new InvalidOperationException("Malformed expression.");
            return evalStack.Pop();
        }

        private List<string> Tokenize(string expr)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < expr.Length)
            {
                char c = expr[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(c) || c == '.')
                {
                    int start = i;
                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.')) i++;
                    tokens.Add(expr[start..i]);
                }
                else if (char.IsLetter(c))
                {
                    int start = i;
                    while (i < expr.Length && char.IsLetter(expr[i])) i++;
                    string word = expr[start..i];
                    if (string.Equals(word, "e", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(Math.E.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        tokens.Add(word);
                    }
                }
                else if (c == '-' && (tokens.Count == 0 || tokens[^1] == "(" || IsOperator(tokens[^1])))
                {
                    // Unary minus: treat as 0 - operand or prepend to number
                    if (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        int start = i;
                        i++;
                        while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.')) i++;
                        tokens.Add(expr[start..i]);
                    }
                    else
                    {
                        tokens.Add("0");
                        tokens.Add("-");
                        i++;
                    }
                }
                else if ("+-*/%^()".Contains(c))
                {
                    tokens.Add(c.ToString());
                    i++;
                }
                else
                {
                    i++;
                }
            }
            return tokens;
        }

        private static bool IsOperator(string s) => s is "+" or "-" or "*" or "/" or "%" or "^";

        private static int Precedence(string op) => op switch
        {
            "+" or "-" => 1,
            "*" or "/" or "%" => 2,
            "^" => 3,
            _ => 0
        };

        private static bool IsLeftAssociative(string op) => op != "^";

        public static double CalculateSquare(double x) => x * x;
        public static double CalculateSquareRoot(double x) => x < 0 ? double.NaN : Math.Sqrt(x);
        public static double CalculateReciprocal(double x) => x == 0 ? double.NaN : 1.0 / x;
        public static double CalculatePercentage(double baseVal, double percent) => (baseVal * percent) / 100.0;
        public static double CalculateFactorial(int n)
        {
            if (n < 0 || n > 20) return double.NaN;
            double res = 1;
            for (int i = 2; i <= n; i++) res *= i;
            return res;
        }
    }
}
