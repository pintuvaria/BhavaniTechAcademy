using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public class QuantumComputingService
    {
        public struct ComplexNumber
        {
            public double Real { get; set; }
            public double Imaginary { get; set; }
            public ComplexNumber(double r, double i) { Real = r; Imaginary = i; }
            public double Magnitude => Math.Sqrt(Real * Real + Imaginary * Imaginary);
            public override string ToString() => $"{Real:F2} + {Imaginary:F2}i";
        }

        public class Qubit
        {
            public ComplexNumber Alpha { get; set; } // Probability amplitude of |0>
            public ComplexNumber Beta { get; set; }  // Probability amplitude of |1>

            public Qubit(ComplexNumber alpha, ComplexNumber beta)
            {
                Alpha = alpha;
                Beta = beta;
            }

            public double ProbabilityZero => Math.Pow(Alpha.Magnitude, 2);
            public double ProbabilityOne => Math.Pow(Beta.Magnitude, 2);

            public override string ToString() => $"[Alpha: {Alpha}, Beta: {Beta}] | P(0): {ProbabilityZero:P2} P(1): {ProbabilityOne:P2}";
        }

        // Applies a 2x2 unitary matrix gate
        public Qubit ApplyGate(Qubit q, ComplexNumber[,] gate)
        {
            var newAlpha = new ComplexNumber(
                q.Alpha.Real * gate[0, 0].Real - q.Alpha.Imaginary * gate[0, 0].Imaginary +
                q.Beta.Real * gate[0, 1].Real - q.Beta.Imaginary * gate[0, 1].Imaginary,
                q.Alpha.Real * gate[0, 0].Imaginary + q.Alpha.Imaginary * gate[0, 0].Real +
                q.Beta.Real * gate[0, 1].Imaginary + q.Beta.Imaginary * gate[0, 1].Real
            );

            var newBeta = new ComplexNumber(
                q.Alpha.Real * gate[1, 0].Real - q.Alpha.Imaginary * gate[1, 0].Imaginary +
                q.Beta.Real * gate[1, 1].Real - q.Beta.Imaginary * gate[1, 1].Imaginary,
                q.Alpha.Real * gate[1, 0].Imaginary + q.Alpha.Imaginary * gate[1, 0].Real +
                q.Beta.Real * gate[1, 1].Imaginary + q.Beta.Imaginary * gate[1, 1].Real
            );

            return new Qubit(newAlpha, newBeta);
        }

        public ComplexNumber[,] GetHadamardGate()
        {
            double h = 1.0 / Math.Sqrt(2);
            return new ComplexNumber[,] {
                { new ComplexNumber(h, 0), new ComplexNumber(h, 0) },
                { new ComplexNumber(h, 0), new ComplexNumber(-h, 0) }
            };
        }

        public ComplexNumber[,] GetPauliXGate()
        {
            return new ComplexNumber[,] {
                { new ComplexNumber(0, 0), new ComplexNumber(1, 0) },
                { new ComplexNumber(1, 0), new ComplexNumber(0, 0) }
            };
        }
    }
}
