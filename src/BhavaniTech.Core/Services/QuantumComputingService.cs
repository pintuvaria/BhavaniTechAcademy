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

        public ComplexNumber[,] GetPauliYGate()
        {
            return new ComplexNumber[,] {
                { new ComplexNumber(0, 0), new ComplexNumber(0, -1) },
                { new ComplexNumber(0, 1), new ComplexNumber(0, 0) }
            };
        }

        public ComplexNumber[,] GetPauliZGate()
        {
            return new ComplexNumber[,] {
                { new ComplexNumber(1, 0), new ComplexNumber(0, 0) },
                { new ComplexNumber(0, 0), new ComplexNumber(-1, 0) }
            };
        }

        public ComplexNumber[,] GetPhaseGate()
        {
            return new ComplexNumber[,] {
                { new ComplexNumber(1, 0), new ComplexNumber(0, 0) },
                { new ComplexNumber(0, 0), new ComplexNumber(0, 1) }
            };
        }

        public ComplexNumber[,] GetTGate()
        {
            double invSqrt2 = 1.0 / Math.Sqrt(2);
            return new ComplexNumber[,] {
                { new ComplexNumber(1, 0), new ComplexNumber(0, 0) },
                { new ComplexNumber(0, 0), new ComplexNumber(invSqrt2, invSqrt2) }
            };
        }

        // 2-Qubit State Representation (|00>, |01>, |10>, |11>)
        public class TwoQubitSystem
        {
            public ComplexNumber[] State { get; set; } // Length 4

            public TwoQubitSystem(ComplexNumber amp00, ComplexNumber amp01, ComplexNumber amp10, ComplexNumber amp11)
            {
                State = new[] { amp00, amp01, amp10, amp11 };
            }

            public double Prob00 => Math.Pow(State[0].Magnitude, 2);
            public double Prob01 => Math.Pow(State[1].Magnitude, 2);
            public double Prob10 => Math.Pow(State[2].Magnitude, 2);
            public double Prob11 => Math.Pow(State[3].Magnitude, 2);

            public override string ToString() =>
                $"|00>: {Prob00:P1}, |01>: {Prob01:P1}, |10>: {Prob10:P1}, |11>: {Prob11:P1}";
        }

        // Applies 4x4 matrix to 2-qubit state
        public TwoQubitSystem ApplyTwoQubitGate(TwoQubitSystem system, ComplexNumber[,] gate4x4)
        {
            var newState = new ComplexNumber[4];
            for (int r = 0; r < 4; r++)
            {
                double real = 0, imag = 0;
                for (int c = 0; c < 4; c++)
                {
                    real += system.State[c].Real * gate4x4[r, c].Real - system.State[c].Imaginary * gate4x4[r, c].Imaginary;
                    imag += system.State[c].Real * gate4x4[r, c].Imaginary + system.State[c].Imaginary * gate4x4[r, c].Real;
                }
                newState[r] = new ComplexNumber(real, imag);
            }
            return new TwoQubitSystem(newState[0], newState[1], newState[2], newState[3]);
        }

        public ComplexNumber[,] GetCnotGate()
        {
            // CNOT matrix where qubit 0 is control, qubit 1 is target
            // |00> -> |00>, |01> -> |01>, |10> -> |11>, |11> -> |10>
            var cnot = new ComplexNumber[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    cnot[i, j] = new ComplexNumber(0, 0);

            cnot[0, 0] = new ComplexNumber(1, 0);
            cnot[1, 1] = new ComplexNumber(1, 0);
            cnot[2, 3] = new ComplexNumber(1, 0);
            cnot[3, 2] = new ComplexNumber(1, 0);
            return cnot;
        }

        // Generates maximally entangled Bell State |Phi+> = (|00> + |11>) / sqrt(2)
        public TwoQubitSystem CreateBellStatePhiPlus()
        {
            double invSqrt2 = 1.0 / Math.Sqrt(2);
            // Starts in |00>, H on qubit 0 gives (|00> + |10>)/sqrt(2), then CNOT gives (|00> + |11>)/sqrt(2)
            var initial = new TwoQubitSystem(
                new ComplexNumber(invSqrt2, 0),
                new ComplexNumber(0, 0),
                new ComplexNumber(invSqrt2, 0),
                new ComplexNumber(0, 0)
            );
            return ApplyTwoQubitGate(initial, GetCnotGate());
        }
    }
}

