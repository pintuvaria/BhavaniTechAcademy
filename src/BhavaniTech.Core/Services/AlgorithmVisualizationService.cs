using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record AlgorithmStep(int StepNumber, string Description, string ArrayState, string Pointers);
    public record AlgorithmTraceResult(
        string AlgorithmName,
        string InitialData,
        string FinalData,
        int TotalComparisons,
        int TotalSwaps,
        List<AlgorithmStep> Steps,
        string ComplexityInsight
    );

    public record BstTraversalResult(
        string AsciiTree,
        List<int> InOrder,
        List<int> PreOrder,
        List<int> PostOrder,
        string EducationalInsight
    );

    public record GraphTraversalResult(
        string GraphRepresentation,
        List<string> BfsVisitOrder,
        List<string> DfsVisitOrder,
        string TraceExplanation
    );

    public static class AlgorithmVisualizationService
    {
        // =====================================================================
        // BUBBLE SORT VISUALIZER
        // =====================================================================
        public static AlgorithmTraceResult TraceBubbleSort(int[] input)
        {
            var arr = (int[])input.Clone();
            var steps = new List<AlgorithmStep>();
            int comparisons = 0;
            int swaps = 0;
            int stepNum = 1;

            steps.Add(new AlgorithmStep(stepNum++, "Initial Unsorted Array", $"[{string.Join(", ", arr)}]", "Start"));

            for (int i = 0; i < arr.Length - 1; i++)
            {
                bool swapped = false;
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    comparisons++;
                    if (arr[j] > arr[j + 1])
                    {
                        // Swap
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                        swaps++;
                        swapped = true;

                        steps.Add(new AlgorithmStep(
                            stepNum++,
                            $"Swap: arr[{j}] ({temp}) > arr[{j+1}] ({arr[j]})",
                            $"[{string.Join(", ", arr)}]",
                            $"Pointers: j={j}, j+1={j+1}"
                        ));
                    }
                }
                if (!swapped) break; // Optimized early exit
            }

            steps.Add(new AlgorithmStep(stepNum, "Array Fully Sorted", $"[{string.Join(", ", arr)}]", "Done"));

            string insight = "Bubble Sort Complexity:\n" +
                "- Best Case: O(N) when array is already sorted (early break flag).\n" +
                "- Worst & Average Case: O(N^2) quadratic time complexity with O(1) auxiliary space.";

            return new AlgorithmTraceResult("Bubble Sort", $"[{string.Join(", ", input)}]", $"[{string.Join(", ", arr)}]", comparisons, swaps, steps, insight);
        }

        // =====================================================================
        // QUICKSORT VISUALIZER
        // =====================================================================
        public static AlgorithmTraceResult TraceQuickSort(int[] input)
        {
            var arr = (int[])input.Clone();
            var steps = new List<AlgorithmStep>();
            int comparisons = 0;
            int swaps = 0;
            int stepNum = 1;

            steps.Add(new AlgorithmStep(stepNum++, "Initial Unsorted Array", $"[{string.Join(", ", arr)}]", "Start"));

            void QuickSort(int low, int high)
            {
                if (low < high)
                {
                    int pi = Partition(low, high);
                    QuickSort(low, pi - 1);
                    QuickSort(pi + 1, high);
                }
            }

            int Partition(int low, int high)
            {
                int pivot = arr[high];
                int i = low - 1;

                steps.Add(new AlgorithmStep(stepNum++, $"Partitioning range [{low}..{high}] with Pivot={pivot}", $"[{string.Join(", ", arr)}]", $"Pivot: arr[{high}]={pivot}"));

                for (int j = low; j < high; j++)
                {
                    comparisons++;
                    if (arr[j] < pivot)
                    {
                        i++;
                        int tmp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = tmp;
                        swaps++;
                        steps.Add(new AlgorithmStep(stepNum++, $"Swap smaller element arr[{j}] with arr[{i}]", $"[{string.Join(", ", arr)}]", $"i={i}, j={j}"));
                    }
                }

                // Swap pivot into correct position
                int temp1 = arr[i + 1];
                arr[i + 1] = arr[high];
                arr[high] = temp1;
                swaps++;
                steps.Add(new AlgorithmStep(stepNum++, $"Place Pivot {pivot} at final index {i + 1}", $"[{string.Join(", ", arr)}]", $"Pivot Index: {i + 1}"));

                return i + 1;
            }

            QuickSort(0, arr.Length - 1);
            steps.Add(new AlgorithmStep(stepNum, "QuickSort Complete", $"[{string.Join(", ", arr)}]", "Done"));

            string insight = "QuickSort (Divide and Conquer):\n" +
                "- Average Time: O(N log N) utilizing cache-friendly in-place partitioning.\n" +
                "- Space Complexity: O(log N) recursive call stack frames.";

            return new AlgorithmTraceResult("QuickSort", $"[{string.Join(", ", input)}]", $"[{string.Join(", ", arr)}]", comparisons, swaps, steps, insight);
        }

        // =====================================================================
        // BINARY SEARCH VISUALIZER
        // =====================================================================
        public static AlgorithmTraceResult TraceBinarySearch(int[] sortedArray, int target)
        {
            var steps = new List<AlgorithmStep>();
            int low = 0;
            int high = sortedArray.Length - 1;
            int comparisons = 0;
            int stepNum = 1;
            int foundIndex = -1;

            while (low <= high)
            {
                comparisons++;
                int mid = low + (high - low) / 2;
                int midVal = sortedArray[mid];

                steps.Add(new AlgorithmStep(
                    stepNum++,
                    $"Check mid index {mid} (value={midVal}) against target {target}",
                    $"[{string.Join(", ", sortedArray)}]",
                    $"low={low}, mid={mid}, high={high}"
                ));

                if (midVal == target)
                {
                    foundIndex = mid;
                    steps.Add(new AlgorithmStep(stepNum++, $"TARGET FOUND at index {mid}!", $"[{string.Join(", ", sortedArray)}]", $"Found: Index {mid}"));
                    break;
                }

                if (midVal < target)
                {
                    steps.Add(new AlgorithmStep(stepNum++, $"Value {midVal} < {target} -> Discard left half, search [{mid + 1}..{high}]", $"[{string.Join(", ", sortedArray)}]", $"low = mid + 1"));
                    low = mid + 1;
                }
                else
                {
                    steps.Add(new AlgorithmStep(stepNum++, $"Value {midVal} > {target} -> Discard right half, search [{low}..{mid - 1}]", $"[{string.Join(", ", sortedArray)}]", $"high = mid - 1"));
                    high = mid - 1;
                }
            }

            string outcome = foundIndex >= 0 ? $"Target {target} located at index {foundIndex}" : $"Target {target} not found";
            string insight = "Binary Search Complexity:\n" +
                "- Time Complexity: O(log N) logarithmic time (halves search space every step).\n" +
                $"- For 1,000,000 items, binary search requires at most 20 comparisons!";

            return new AlgorithmTraceResult("Binary Search", $"Target: {target}", outcome, comparisons, 0, steps, insight);
        }

        // =====================================================================
        // BINARY SEARCH TREE (BST)
        // =====================================================================
        private class BstNode
        {
            public int Value;
            public BstNode? Left;
            public BstNode? Right;
            public BstNode(int val) => Value = val;
        }

        public static BstTraversalResult BuildAndTraverseBst(List<int> values)
        {
            if (values == null || values.Count == 0) values = new List<int> { 50, 30, 70, 20, 40, 60, 80 };

            BstNode? root = null;
            BstNode Insert(BstNode? node, int val)
            {
                if (node == null) return new BstNode(val);
                if (val < node.Value) node.Left = Insert(node.Left, val);
                else if (val > node.Value) node.Right = Insert(node.Right, val);
                return node;
            }

            foreach (var v in values) root = Insert(root, v);

            var inOrder = new List<int>();
            var preOrder = new List<int>();
            var postOrder = new List<int>();

            void Traverse(BstNode? node)
            {
                if (node == null) return;
                preOrder.Add(node.Value);
                Traverse(node.Left);
                inOrder.Add(node.Value);
                Traverse(node.Right);
                postOrder.Add(node.Value);
            }
            Traverse(root);

            var ascii = new StringBuilder();
            ascii.AppendLine("       (50)");
            ascii.AppendLine("      /    \\");
            ascii.AppendLine("   (30)    (70)");
            ascii.AppendLine("   /  \\    /  \\");
            ascii.AppendLine(" (20)(40)(60)(80)");

            string insight = "Binary Search Tree Properties:\n" +
                "- In-Order Traversal ALWAYS produces sorted output: " + string.Join(" < ", inOrder) + "\n" +
                "- Search/Insert/Delete: O(log N) average, O(N) worst-case (unbalanced degenerating to linked list).\n" +
                "- Self-balancing variants (AVL, Red-Black Trees) guarantee O(log N) worst-case balance.";

            return new BstTraversalResult(ascii.ToString(), inOrder, preOrder, postOrder, insight);
        }

        // =====================================================================
        // GRAPH BREADTH-FIRST (BFS) & DEPTH-FIRST (DFS) TRAVERSAL
        // =====================================================================
        public static GraphTraversalResult TraverseSampleGraph()
        {
            // Sample Graph:
            // A -> B, C
            // B -> D, E
            // C -> F
            // E -> F
            var adj = new Dictionary<string, List<string>>
            {
                ["A"] = new() { "B", "C" },
                ["B"] = new() { "A", "D", "E" },
                ["C"] = new() { "A", "F" },
                ["D"] = new() { "B" },
                ["E"] = new() { "B", "F" },
                ["F"] = new() { "C", "E" }
            };

            // BFS (Queue FIFO)
            var bfsOrder = new List<string>();
            var bfsVisited = new HashSet<string>();
            var queue = new Queue<string>();

            queue.Enqueue("A");
            bfsVisited.Add("A");

            while (queue.Count > 0)
            {
                string curr = queue.Dequeue();
                bfsOrder.Add(curr);

                foreach (var neighbor in adj[curr])
                {
                    if (!bfsVisited.Contains(neighbor))
                    {
                        bfsVisited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // DFS (Recursion / Stack LIFO)
            var dfsOrder = new List<string>();
            var dfsVisited = new HashSet<string>();

            void Dfs(string u)
            {
                dfsVisited.Add(u);
                dfsOrder.Add(u);
                foreach (var v in adj[u])
                {
                    if (!dfsVisited.Contains(v)) Dfs(v);
                }
            }
            Dfs("A");

            var graphAscii = new StringBuilder();
            graphAscii.AppendLine("Graph Adjacency Topology:");
            graphAscii.AppendLine("  (A) ───► (B) ───► (D)");
            graphAscii.AppendLine("   │        │");
            graphAscii.AppendLine("   │        ▼");
            graphAscii.AppendLine("   ▼       (E) ───► (F)");
            graphAscii.AppendLine("  (C) ──────────────▲");

            string trace = "Graph Search Applications:\n" +
                "1. Breadth-First Search (BFS): Level-by-level queue traversal. Guarantees shortest path in unweighted networks (GPS routing, social network mutual connections).\n" +
                "2. Depth-First Search (DFS): Backtracking stack traversal. Used for topological sorting, cycle detection, and maze solving.";

            return new GraphTraversalResult(graphAscii.ToString(), bfsOrder, dfsOrder, trace);
        }
    }
}
