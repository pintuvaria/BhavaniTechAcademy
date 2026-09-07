using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record ConvolutionStep(
        int TargetRow,
        int TargetCol,
        double SubregionSum,
        string MathExpression
    );

    public record ConvolutionResult(
        string KernelName,
        double[,] KernelMatrix,
        int[,] InputImage,
        int[,] OutputImage,
        int ImageWidth,
        int ImageHeight,
        List<ConvolutionStep> SampleSteps,
        int[,] PooledImage
    );

    public static class ComputerVisionLabService
    {
        public static double[,] GetKernel(string kernelName)
        {
            return kernelName.ToLowerInvariant() switch
            {
                "sobel_horizontal" => new double[,]
                {
                    { -1, -2, -1 },
                    {  0,  0,  0 },
                    {  1,  2,  1 }
                },
                "sobel_vertical" => new double[,]
                {
                    { -1,  0,  1 },
                    { -2,  0,  2 },
                    { -1,  0,  1 }
                },
                "sharpen" => new double[,]
                {
                    {  0, -1,  0 },
                    { -1,  5, -1 },
                    {  0, -1,  0 }
                },
                "ridge_laplacian" => new double[,]
                {
                    { -1, -1, -1 },
                    { -1,  8, -1 },
                    { -1, -1, -1 }
                },
                "gaussian_blur" => new double[,]
                {
                    { 1.0/16.0, 2.0/16.0, 1.0/16.0 },
                    { 2.0/16.0, 4.0/16.0, 2.0/16.0 },
                    { 1.0/16.0, 2.0/16.0, 1.0/16.0 }
                },
                _ => new double[,]
                {
                    { 0, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 }
                }
            };
        }

        public static int[,] GenerateSampleImage(string pattern = "box")
        {
            int size = 8;
            var img = new int[size, size];

            switch (pattern.ToLowerInvariant())
            {
                case "diagonal":
                    for (int r = 0; r < size; r++)
                    {
                        for (int c = 0; c < size; c++)
                        {
                            img[r, c] = (r == c || r == c + 1 || r == c - 1) ? 255 : 30;
                        }
                    }
                    break;

                case "cross":
                    for (int r = 0; r < size; r++)
                    {
                        for (int c = 0; c < size; c++)
                        {
                            img[r, c] = (r == 3 || r == 4 || c == 3 || c == 4) ? 240 : 20;
                        }
                    }
                    break;

                case "box":
                default:
                    // Central bright square
                    for (int r = 0; r < size; r++)
                    {
                        for (int c = 0; c < size; c++)
                        {
                            bool inside = (r >= 2 && r <= 5 && c >= 2 && c <= 5);
                            img[r, c] = inside ? 255 : 20;
                        }
                    }
                    break;
            }

            return img;
        }

        public static ConvolutionResult ApplyConvolution(string kernelName, string pattern = "box")
        {
            var kernel = GetKernel(kernelName);
            var input = GenerateSampleImage(pattern);
            int height = input.GetLength(0);
            int width = input.GetLength(1);
            var output = new int[height, width];
            var steps = new List<ConvolutionStep>();

            // 3x3 convolution with 0-padding
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    double sum = 0;
                    string expr = "";

                    for (int kr = -1; kr <= 1; kr++)
                    {
                        for (int kc = -1; kc <= 1; kc++)
                        {
                            int ir = r + kr;
                            int ic = c + kc;
                            int pixel = (ir >= 0 && ir < height && ic >= 0 && ic < width) ? input[ir, ic] : 0;
                            double kVal = kernel[kr + 1, kc + 1];
                            sum += pixel * kVal;
                        }
                    }

                    // Clamp to [0, 255]
                    int val = Math.Clamp((int)Math.Round(sum), 0, 255);
                    output[r, c] = val;

                    // Log sample calculation for center pixels
                    if (steps.Count < 3 && r >= 2 && c >= 2)
                    {
                        expr = $"Pos({r},{c}): Kernel Sum = {sum:F1} -> Clamped = {val}";
                        steps.Add(new ConvolutionStep(r, c, sum, expr));
                    }
                }
            }

            // Max Pooling 2x2 stride 2 downsampling (8x8 -> 4x4)
            int poolH = height / 2;
            int poolW = width / 2;
            var pooled = new int[poolH, poolW];
            for (int pr = 0; pr < poolH; pr++)
            {
                for (int pc = 0; pc < poolW; pc++)
                {
                    int m1 = Math.Max(output[pr * 2, pc * 2], output[pr * 2, (pc * 2) + 1]);
                    int m2 = Math.Max(output[(pr * 2) + 1, pc * 2], output[(pr * 2) + 1, (pc * 2) + 1]);
                    pooled[pr, pc] = Math.Max(m1, m2);
                }
            }

            return new ConvolutionResult(
                KernelName: kernelName,
                KernelMatrix: kernel,
                InputImage: input,
                OutputImage: output,
                ImageWidth: width,
                ImageHeight: height,
                SampleSteps: steps,
                PooledImage: pooled
            );
        }
    }
}
