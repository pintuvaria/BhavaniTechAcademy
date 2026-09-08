using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public class TransformerVisualizerService
    {
        // 1. Self-Attention Visualizer
        public record AttentionResult(double[,] AttentionScores, double[,] SoftmaxScores, double[,] ContextMatrix, List<string> ComputationSteps);

        public AttentionResult CalculateSelfAttention(double[,] query, double[,] key, double[,] value)
        {
            var steps = new List<string>();
            int seqLen = query.GetLength(0);
            int dim = query.GetLength(1);

            steps.Add($"Sequence Length: {seqLen}, Dimension: {dim}");

            // Step 1: Q * K^T
            double[,] scores = new double[seqLen, seqLen];
            steps.Add("Step 1: Calculate Dot Products (Q * K^T)");
            for (int i = 0; i < seqLen; i++)
            {
                for (int j = 0; j < seqLen; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < dim; k++) sum += query[i, k] * key[j, k];
                    scores[i, j] = sum;
                }
            }

            // Step 2: Scale
            double scale = Math.Sqrt(dim);
            steps.Add($"Step 2: Scale by sqrt(d_k) = {scale:F2}");
            for (int i = 0; i < seqLen; i++)
                for (int j = 0; j < seqLen; j++)
                    scores[i, j] /= scale;

            // Step 3: Softmax
            steps.Add("Step 3: Apply Softmax along rows to get Attention Weights");
            double[,] softmax = new double[seqLen, seqLen];
            for (int i = 0; i < seqLen; i++)
            {
                double max = double.MinValue;
                for (int j = 0; j < seqLen; j++) if (scores[i, j] > max) max = scores[i, j];

                double sumExp = 0;
                for (int j = 0; j < seqLen; j++)
                {
                    softmax[i, j] = Math.Exp(scores[i, j] - max);
                    sumExp += softmax[i, j];
                }
                for (int j = 0; j < seqLen; j++) softmax[i, j] /= sumExp;
            }

            // Step 4: Multiply by V
            steps.Add("Step 4: Multiply Attention Weights by Value Matrix (Softmax * V)");
            double[,] context = new double[seqLen, dim];
            for (int i = 0; i < seqLen; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < seqLen; k++) sum += softmax[i, k] * value[k, j];
                    context[i, j] = sum;
                }
            }

            steps.Add("Self-Attention Calculation Complete.");
            return new AttentionResult(scores, softmax, context, steps);
        }

        // 2. Positional Encoding
        public double[,] GeneratePositionalEncoding(int seqLength, int dModel)
        {
            double[,] pe = new double[seqLength, dModel];
            for (int pos = 0; pos < seqLength; pos++)
            {
                for (int i = 0; i < dModel; i++)
                {
                    if (i % 2 == 0)
                        pe[pos, i] = Math.Sin(pos / Math.Pow(10000, (double)i / dModel));
                    else
                        pe[pos, i] = Math.Cos(pos / Math.Pow(10000, (double)(i - 1) / dModel));
                }
            }
            return pe;
        }

        // 3. Layer Normalization: LayerNorm(x) = (x - mean) / sqrt(var + eps) * gamma + beta
        public record LayerNormResult(double[,] Normalized, double[] Means, double[] Variances);

        public LayerNormResult CalculateLayerNorm(double[,] input, double epsilon = 1e-5)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            double[,] output = new double[rows, cols];
            double[] means = new double[rows];
            double[] vars = new double[rows];

            for (int r = 0; r < rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < cols; c++) sum += input[r, c];
                double mean = sum / cols;
                means[r] = mean;

                double varSum = 0;
                for (int c = 0; c < cols; c++)
                {
                    double diff = input[r, c] - mean;
                    varSum += diff * diff;
                }
                double variance = varSum / cols;
                vars[r] = variance;

                double std = Math.Sqrt(variance + epsilon);
                for (int c = 0; c < cols; c++)
                {
                    output[r, c] = (input[r, c] - mean) / std;
                }
            }

            return new LayerNormResult(output, means, vars);
        }

        // 4. Multi-Head Attention Simulator (Splitting heads, parallel attention, concat)
        public record MultiHeadAttentionResult(
            int NumHeads,
            List<AttentionResult> HeadResults,
            double[,] CombinedOutput,
            double[,] ResidualAddedNorm
        );

        public MultiHeadAttentionResult CalculateMultiHeadAttention(
            double[,] inputEmbedding,
            int numHeads = 2)
        {
            int seqLen = inputEmbedding.GetLength(0);
            int dModel = inputEmbedding.GetLength(1);
            int dK = dModel / numHeads;

            var headResults = new List<AttentionResult>();
            double[,] concatenated = new double[seqLen, dModel];

            for (int h = 0; h < numHeads; h++)
            {
                // Extract head slice
                double[,] qHead = new double[seqLen, dK];
                double[,] kHead = new double[seqLen, dK];
                double[,] vHead = new double[seqLen, dK];

                for (int i = 0; i < seqLen; i++)
                {
                    for (int j = 0; j < dK; j++)
                    {
                        int srcCol = h * dK + j;
                        qHead[i, j] = inputEmbedding[i, srcCol];
                        kHead[i, j] = inputEmbedding[i, srcCol];
                        vHead[i, j] = inputEmbedding[i, srcCol];
                    }
                }

                var headAttn = CalculateSelfAttention(qHead, kHead, vHead);
                headResults.Add(headAttn);

                // Concatenate head context back
                for (int i = 0; i < seqLen; i++)
                {
                    for (int j = 0; j < dK; j++)
                    {
                        concatenated[i, h * dK + j] = headAttn.ContextMatrix[i, j];
                    }
                }
            }

            // Residual connection: input + MHA(input)
            double[,] residual = new double[seqLen, dModel];
            for (int i = 0; i < seqLen; i++)
                for (int j = 0; j < dModel; j++)
                    residual[i, j] = inputEmbedding[i, j] + concatenated[i, j];

            // LayerNorm after residual: LayerNorm(x + Sublayer(x))
            var normRes = CalculateLayerNorm(residual);

            return new MultiHeadAttentionResult(numHeads, headResults, concatenated, normRes.Normalized);
        }
    }
}
