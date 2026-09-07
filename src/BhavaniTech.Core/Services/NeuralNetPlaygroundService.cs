using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record DataPoint2D(double X1, double X2, int Label); // Label 0 or 1
    public record GridPrediction(double X1, double X2, double Probability, int PredictedClass);

    public record NeuralNetTrainingStep(
        int Epoch,
        double Loss,
        double AccuracyPercent,
        string WeightSummary
    );

    public record PerceptronNetworkState(
        string ActivationName,
        double[,] W1, // [hiddenSize, 2]
        double[] B1,  // [hiddenSize]
        double[] W2,  // [hiddenSize]
        double B2,
        List<DataPoint2D> TrainingPoints,
        List<GridPrediction> DecisionBoundaryGrid,
        double CurrentLoss,
        double CurrentAccuracy
    );

    public static class NeuralNetPlaygroundService
    {
        public static double Activate(double z, string activation)
        {
            return activation.ToLowerInvariant() switch
            {
                "sigmoid" => 1.0 / (1.0 + Math.Exp(-Math.Clamp(z, -45.0, 45.0))),
                "tanh" => Math.Tanh(z),
                "relu" => Math.Max(0.0, z),
                "leaky_relu" => z > 0 ? z : 0.01 * z,
                _ => 1.0 / (1.0 + Math.Exp(-Math.Clamp(z, -45.0, 45.0)))
            };
        }

        public static List<DataPoint2D> GenerateDataset(string datasetType)
        {
            var points = new List<DataPoint2D>();
            var rnd = new Random(42);

            switch (datasetType.ToLowerInvariant())
            {
                case "xor":
                    // 4 quadrants XOR
                    points.Add(new DataPoint2D(1.0, 1.0, 0));
                    points.Add(new DataPoint2D(1.2, 0.9, 0));
                    points.Add(new DataPoint2D(-1.0, -1.0, 0));
                    points.Add(new DataPoint2D(-1.2, -0.8, 0));
                    points.Add(new DataPoint2D(-1.0, 1.0, 1));
                    points.Add(new DataPoint2D(-0.9, 1.3, 1));
                    points.Add(new DataPoint2D(1.0, -1.0, 1));
                    points.Add(new DataPoint2D(1.1, -1.1, 1));
                    break;

                case "circle":
                    // Inner circle 0, outer ring 1
                    for (int i = 0; i < 20; i++)
                    {
                        double angle = (2 * Math.PI * i) / 20;
                        // Inner ring radius ~0.6
                        points.Add(new DataPoint2D(0.6 * Math.Cos(angle), 0.6 * Math.Sin(angle), 0));
                        // Outer ring radius ~1.4
                        points.Add(new DataPoint2D(1.4 * Math.Cos(angle), 1.4 * Math.Sin(angle), 1));
                    }
                    break;

                case "linear":
                default:
                    // Linearly separable along x1 + x2 > 0
                    for (int i = 0; i < 15; i++)
                    {
                        double x1 = (rnd.NextDouble() * 1.5) + 0.2;
                        double x2 = (rnd.NextDouble() * 1.5) + 0.2;
                        points.Add(new DataPoint2D(Math.Round(x1, 2), Math.Round(x2, 2), 1));

                        double nx1 = -(rnd.NextDouble() * 1.5) - 0.2;
                        double nx2 = -(rnd.NextDouble() * 1.5) - 0.2;
                        points.Add(new DataPoint2D(Math.Round(nx1, 2), Math.Round(nx2, 2), 0));
                    }
                    break;
            }

            return points;
        }

        public static PerceptronNetworkState CreateNetwork(string datasetType, string activation = "sigmoid", int hiddenNeurons = 3)
        {
            var data = GenerateDataset(datasetType);

            // Initialize weights
            var rnd = new Random(101);
            var w1 = new double[hiddenNeurons, 2];
            var b1 = new double[hiddenNeurons];
            var w2 = new double[hiddenNeurons];
            double b2 = 0.1;

            for (int h = 0; h < hiddenNeurons; h++)
            {
                w1[h, 0] = (rnd.NextDouble() * 1.0) - 0.5;
                w1[h, 1] = (rnd.NextDouble() * 1.0) - 0.5;
                b1[h] = 0.05;
                w2[h] = (rnd.NextDouble() * 1.0) - 0.5;
            }

            // Adjust weights for demonstration if standard dataset
            if (datasetType == "linear")
            {
                w1[0, 0] = 1.2; w1[0, 1] = 1.1; b1[0] = 0.1;
                w2[0] = 1.5; b2 = 0.0;
            }

            return EvaluateNetwork(data, w1, b1, w2, b2, activation, hiddenNeurons);
        }

        public static double PredictSingle(double x1, double x2, double[,] w1, double[] b1, double[] w2, double b2, string activation, int hiddenNeurons)
        {
            var hAct = new double[hiddenNeurons];
            for (int h = 0; h < hiddenNeurons; h++)
            {
                double z1 = (w1[h, 0] * x1) + (w1[h, 1] * x2) + b1[h];
                hAct[h] = Activate(z1, activation);
            }

            double z2 = b2;
            for (int h = 0; h < hiddenNeurons; h++)
            {
                z2 += w2[h] * hAct[h];
            }

            return Activate(z2, "sigmoid"); // Output probability [0, 1]
        }

        public static PerceptronNetworkState EvaluateNetwork(
            List<DataPoint2D> data,
            double[,] w1,
            double[] b1,
            double[] w2,
            double b2,
            string activation,
            int hiddenNeurons)
        {
            // Evaluate on training points
            double totalLoss = 0;
            int correct = 0;

            foreach (var pt in data)
            {
                double prob = PredictSingle(pt.X1, pt.X2, w1, b1, w2, b2, activation, hiddenNeurons);
                int predClass = prob >= 0.5 ? 1 : 0;
                if (predClass == pt.Label) correct++;

                // Binary Cross Entropy Loss
                double pClamped = Math.Clamp(prob, 1e-7, 1.0 - 1e-7);
                totalLoss += -(pt.Label * Math.Log(pClamped) + (1 - pt.Label) * Math.Log(1.0 - pClamped));
            }

            double avgLoss = Math.Round(totalLoss / Math.Max(1, data.Count), 4);
            double accuracy = Math.Round((double)correct / Math.Max(1, data.Count) * 100.0, 1);

            // Generate 2D decision boundary grid (15x15 = 225 cells from -2 to +2)
            var grid = new List<GridPrediction>();
            for (double y = 2.0; y >= -2.0; y -= 0.3)
            {
                for (double x = -2.0; x <= 2.0; x += 0.3)
                {
                    double p = PredictSingle(x, y, w1, b1, w2, b2, activation, hiddenNeurons);
                    grid.Add(new GridPrediction(Math.Round(x, 2), Math.Round(y, 2), Math.Round(p, 3), p >= 0.5 ? 1 : 0));
                }
            }

            return new PerceptronNetworkState(
                ActivationName: activation,
                W1: w1,
                B1: b1,
                W2: w2,
                B2: b2,
                TrainingPoints: data,
                DecisionBoundaryGrid: grid,
                CurrentLoss: avgLoss,
                CurrentAccuracy: accuracy
            );
        }
    }
}
