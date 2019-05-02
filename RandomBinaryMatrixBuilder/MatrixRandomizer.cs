using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBinaryMatrixBuilder
{
    public static class MatrixRandomizer
    {
        /// <summary>
        /// Random generator for the randomization of the transformation.
        /// </summary>
        static Random random = new Random();

        /// <summary>
        /// Randomizes an input binary matrix.
        /// </summary>
        /// <param name="matrix">A matrix with 1 or 0 values per cell.</param>
        /// <returns></returns>
        public static int[,] Randomize(int[,] matrix)
        {
            // Get dimensions to work with from input matrix.
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            // List of the 1's in general and per row.
            Stack<int>[] rowOnes = new Stack<int>[m];
            HashSet<int>[] rowOnesSet = new HashSet<int>[m];
            int[] rowsums = new int[m];
            Stack<Index> ones = new Stack<Index>();
            HashSet<Index> onesSet = new HashSet<Index>();
            for (int j = 0; j < m; j++)
            {
                rowOnes[j] = new Stack<int>();
                rowOnesSet[j] = new HashSet<int>();
                for (int i = 0; i < n; i++)
                    if (matrix[i, j] == 1)
                    {
                        rowOnes[j].Push(i);
                        rowOnesSet[j].Add(i);
                        rowsums[j]++;
                        ones.Push(new Index(i, j));
                        onesSet.Add(new Index(i, j));
                    }
            }

            // List of the zeros that lead to 1's.
            Stack<int>[] feasableRowsForColumn = new Stack<int>[n];
            HashSet<int>[] feasableRowsForColumnSet = new HashSet<int>[n];
            for (int i = 0; i < n; i++)
            {
                feasableRowsForColumn[i] = new Stack<int>();
                feasableRowsForColumnSet[i] = new HashSet<int>();
                for (int j = 0; j < m; j++)
                    if (matrix[i, j] == 0 && rowOnes[j].Count > 0)
                    {
                        if(!feasableRowsForColumnSet[i].Contains(j))
                            feasableRowsForColumn[i].Push(j);
                        feasableRowsForColumnSet[i].Add(j);
                    }
            }

            // Order randomization of all stacks.
            for (int i = 0; i < n; i++)
                feasableRowsForColumn[i].Shuffle();
            for (int j = 0; j < m; j++)
                rowOnes[j].Shuffle();
            ones.Shuffle();

            // Get some random cycles.
            List<Stack<Index>> cycles = new List<Stack<Index>>();
            HashSet<int> visitedColumns = new HashSet<int>();
            HashSet<Index> visitedOnes = new HashSet<Index>();
            Stack<Index> currentPath = new Stack<Index>();
            while (onesSet.Count > 0)
            {
                Index current = ones.Pop();
                if (currentPath.Count == 0)
                {
                    visitedColumns = new HashSet<int>();
                    while (ones.Count > 0 && visitedOnes.Contains(current))
                        current = ones.Pop();

                    visitedColumns.Add(current.i);
                    currentPath.Push(current);
                }
                else
                {
                    Index last = currentPath.Pop();
                    if (random.Next(0, feasableRowsForColumnSet[last.i].Count + 1) != 0)
                    {
                        currentPath.Push(last);
                        while (feasableRowsForColumn[last.i].Count > 0 && visitedOnes.Contains(current))
                        {
                            int nextrow = feasableRowsForColumn[last.i].Pop();
                            while (feasableRowsForColumn[last.i].Count > 0 && !feasableRowsForColumn[last.i].Contains(current.j))
                                nextrow = feasableRowsForColumn[last.i].Pop();

                            int nextcolumn = rowOnes[nextrow].Pop();
                            if(rowOnes[nextrow].Count == 0)
                                for (int i = 0; i < n; i++)
                                    feasableRowsForColumnSet[i].Remove(nextrow);
                            current = new Index(nextrow, nextcolumn);
                        }
                        currentPath.Push(current);
                        if(!visitedColumns.Contains(current.i))
                            visitedColumns.Add(current.i);
                        else
                        {
                            // Build cycle and store it.
                            last = currentPath.Pop();
                            Stack<Index> cycle = new Stack<Index>();
                            cycle.Push(last);
                            Index top = currentPath.Pop();
                            while(top.i != last.i)
                            {
                                cycle.Push(top);
                                visitedColumns.Remove(top.i);
                                if (currentPath.Count > 0)
                                    top = currentPath.Pop();
                                else
                                    top.i = last.i;
                            }
                            cycles.Add(cycle);
                        }
                    }
                    else
                    {
                        visitedColumns.Remove(last.i);
                    }
                }
                visitedOnes.Add(current);
                onesSet.Remove(current);
            }
            

            // Apply transformations for final matrix.
            int[,] finalmatrix = new int[n, m];
            HashSet<Index> changes = new HashSet<Index>();
            foreach(Stack<Index> cycle in cycles)
            {
                Index first = cycle.Pop();
                changes.Add(first);
                Index last = first;
                Index current;
                while(cycle.Count > 0)
                {
                    current = cycle.Pop();
                    changes.Add(new Index(current.i, last.j));
                    changes.Add(current);
                    last = current;
                }
            }
            for(int i = 0; i < n; i++)
                for(int j = 0; j < m; j++)
                {
                    finalmatrix[i, j] = matrix[i, j];
                    if(changes.Contains(new Index(i, j)))
                        finalmatrix[i, j] ^= 1;
                }

            return finalmatrix;
        }

        // Stack shuffle extension.
        public static void Shuffle<T>(this Stack<T> stack)
        {
            var values = stack.ToArray();
            stack.Clear();
            foreach (var value in values.OrderBy(x => random.Next()))
                stack.Push(value);
        }

    }

    struct Index
    {
        /// <summary>
        /// First index.
        /// </summary>
        public int i;

        /// <summary>
        /// Second index.
        /// </summary>
        public int j;

        /// <summary>
        /// A two dimensional index datastructure.
        /// </summary>
        /// <param name="x">The first item of the index.</param>
        /// <param name="y">The second item of the index.</param>
        public Index(int x, int y)
        {
            i = x;
            j = y;
        }
    }
}
