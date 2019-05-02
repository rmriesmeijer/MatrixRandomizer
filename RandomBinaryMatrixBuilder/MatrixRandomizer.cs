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
            Stack<Index> ones = new Stack<Index>();
            HashSet<Index> onesSet = new HashSet<Index>();
            for (int j = 0; j < m; j++)
            {
                rowOnes[j] = new Stack<int>();
                for (int i = 0; i < n; i++)
                    if (matrix[i, j] == 1)
                    {
                        rowOnes[j].Push(i);
                        rowOnesSet[j].Add(i);
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

            // Get some random cycles.
            List<Stack<Index>> cycles = new List<Stack<Index>>();
            HashSet<int> visitedColumns = new HashSet<int>();
            

            // Apply transformations for final matrix.
            int[,] finalmatrix = new int[n, m];

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
