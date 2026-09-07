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

                // Sanitize and replace display operators
                string sanitized = expression
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace("−", "-")
                    .Trim();

                steps.Add($"Normalized Math String: {sanitized}");

                // Evaluate standard arithmetic using DataTable / compute
                var dt = new DataTable();
                var computeResult = dt.Compute(sanitized, null);
                double val = Convert.ToDouble(computeResult, CultureInfo.InvariantCulture);

                string formatted = val % 1 == 0 ? val.ToString("N0", CultureInfo.InvariantCulture) : val.ToString("G10", CultureInfo.InvariantCulture);
                steps.Add($"Evaluation Complete: {sanitized} = {formatted}");

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

        public static double CalculateSquare(double x) => x * x;
        public static double CalculateSquareRoot(double x) => x < 0 ? double.NaN : Math.Sqrt(x);
        public static double CalculateReciprocal(double x) => x == 0 ? double.NaN : 1.0 / x;
        public static double CalculatePercentage(double baseVal, double percent) => (baseVal * percent) / 100.0;
    }
}
