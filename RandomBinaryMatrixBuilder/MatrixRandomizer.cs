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
            Stack<int>[] rowOnes = getRowStacks(n, m, matrix);

            // List of the zeros that lead to 1's.
            Tuple < Stack<int>[], HashSet<int>[]> colitems = getColumnStacks(n, m, matrix, rowOnes);
            Stack<int>[] feasableRowsForColumn = colitems.Item1;
            HashSet<int>[] feasableRowsForColumnSet = colitems.Item2;

            // Order randomization of all stacks.
            for (int i = 0; i < n; i++)
                feasableRowsForColumn[i].Shuffle();
            for (int j = 0; j < m; j++)
                rowOnes[j].Shuffle();

            // Copy input matrix.
            int[,] finalmatrix = new int[n, m];
            for(int i = 0; i < n; i++)
                for(int j = 0; j < m; j++)
                    finalmatrix[i, j] = matrix[i, j];

            // Get stacks ready for random choosing.
            List<int> nonemptystacks = new List<int>();
            for (int i = 0; i < n; i++)
                if (feasableRowsForColumn[i].Count > 0)
                    nonemptystacks.Add(i);

            // While we have not evaluated all values.
            bool[] zerosvisitedcolumn = new bool[n];
            Stack<Index> traversals = new Stack<Index>();
            List<Index> changes = new List<Index>();
            while (nonemptystacks.Count > 0)
            {
                // Get the column that should have the zero to start from.
                int colzero = 0;
                if (traversals.Count == 0)
                {
                    // Choose starting point for path.
                    colzero = nonemptystacks[random.Next(0, nonemptystacks.Count)];

                }
                else
                {
                    // regular traversal, go further where we last left off.
                    Index last = traversals.Pop();
                    colzero = last.i;
                    if (feasableRowsForColumnSet[colzero].Count == 0)
                    {
                        // We cannot traverse and shall backtrack.
                        zerosvisitedcolumn[traversals.Pop().i] = false;
                    }
                    else
                    {
                        // Undo the pop we did if we can traverse using this last index.
                        traversals.Push(last);
                    }
                }

                // If there is a valid traversal from our current endpoint we will compute it.
                if (feasableRowsForColumnSet[colzero].Count != 0)
                {
                    // Get the one.
                    int row = feasableRowsForColumn[colzero].Pop();
                    while (!feasableRowsForColumnSet[colzero].Contains(row)) // Used to correct for mismatch with the hashset and keep linear time.
                        row = feasableRowsForColumn[colzero].Pop();
                    int colone = rowOnes[row].Pop();
                    if (rowOnes[row].Count == 0)
                    {
                        // Don't use this row anymore.
                        for (int i = 0; i < n; i++)
                        {
                            feasableRowsForColumnSet[i].Remove(row);
                            if (feasableRowsForColumnSet[i].Count == 0)
                                nonemptystacks.Remove(i);
                        }
                    }
                    else
                    {
                        // Don't use this rowzero in combination with this 1 anymore.
                        feasableRowsForColumnSet[colzero].Remove(row);
                        if (feasableRowsForColumnSet[colzero].Count == 0)
                            nonemptystacks.Remove(colzero);
                    }

                    // Sample the fixing chance uniformly from all the other traversals that are possible.
                    if (random.Next(0, feasableRowsForColumnSet[colone].Count + 1) != 0)
                    {
                        // We decided not to make this a fixed point.

                        // We've got a cycle.
                        if (zerosvisitedcolumn[colone] == true)
                        {
                            changes.Add(new Index(colzero, row));
                            changes.Add(new Index(colone, row));

                            Index current = traversals.Pop();
                            while (traversals.Count > 0 && current.i != colone)
                            {
                                changes.Add(current);
                                zerosvisitedcolumn[current.i] = false;
                                current = traversals.Pop();
                            }
                            changes.Add(current);
                            zerosvisitedcolumn[current.i] = false;
                        }
                        else
                        {
                            // No cycle, go on with traversal.
                            traversals.Push(new Index(colzero, row));
                            traversals.Push(new Index(colone, row));
                            zerosvisitedcolumn[colzero] = true;
                        }
                    }
                    else
                    {
                        // We decided to fix this point.

                        // Backtrack.
                        if (traversals.Count > 1)
                        {
                            traversals.Pop();
                            colzero = traversals.Pop().i;
                            zerosvisitedcolumn[colzero] = false;
                        }
                    }
                }
            }

            // Make the computed changes.
            foreach(Index change in changes)
                finalmatrix[change.i, change.j] ^= 1;

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

        /// <summary>
        /// Gets stacks of indices that contain a one in a row of the matrix.
        /// </summary>
        /// <param name="n">Number of columns.</param>
        /// <param name="m">Number of rows.</param>
        /// <param name="matrix">The binary matrix we are evaluating.</param>
        /// <returns></returns>
        private static Stack<int>[] getRowStacks(int n, int m, int[,] matrix)
        {
            // Simply iterate through all rows and put the ones in the row on a stack.
            Stack<int>[] rowOnes = new Stack<int>[m];
            for (int j = 0; j < m; j++)
            {
                rowOnes[j] = new Stack<int>();
                for (int i = 0; i < n; i++)
                    if (matrix[i, j] == 1)
                        rowOnes[j].Push(i);
            }
            return rowOnes;
        }

        /// <summary>
        /// Gets rows that have a zero in this column but are not empty, usefull for traversal.
        /// </summary>
        /// <param name="n">Number of columns.</param>
        /// <param name="m">Number of rows.</param>
        /// <param name="matrix">The binary matrix we are evaluating.</param>
        /// <param name="rowOnes">Stacks of ones per row to indicate if rows are not empty.</param>
        /// <returns></returns>
        private static Tuple<Stack<int>[], HashSet<int>[]> getColumnStacks(int n, int m, int[,] matrix, Stack<int>[] rowOnes)
        {
            Stack<int>[] feasableRowsForColumn = new Stack<int>[n];
            HashSet<int>[] feasableRowsForColumnSet = new HashSet<int>[n];
            for (int i = 0; i < n; i++)
            {
                feasableRowsForColumn[i] = new Stack<int>();
                feasableRowsForColumnSet[i] = new HashSet<int>();
                for (int j = 0; j < m; j++)
                    if (matrix[i, j] == 0 && rowOnes[j].Count > 0)
                    {
                        if (!feasableRowsForColumnSet[i].Contains(j))
                            feasableRowsForColumn[i].Push(j);
                        feasableRowsForColumnSet[i].Add(j);
                    }
            }
            return new Tuple<Stack<int>[], HashSet<int>[]>(feasableRowsForColumn, feasableRowsForColumnSet);
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
