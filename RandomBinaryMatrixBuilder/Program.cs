using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBinaryMatrixBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build matrix to test with randomly.
            int n = 500;
            int m = 500;
            int[,] testmatrix = TestMatrixGenerator.GetMatrix(n, m);

            // Randomize matrix.
            int[,] randomizedmatrix = MatrixRandomizer.Randomize(testmatrix);

            // Count differences.
            int count = 0;
            int[] columnsums = new int[n];
            int[] rowsums = new int[m];
            int[] columnsumsrandomized = new int[n];
            int[] rowsumsrandomized = new int[m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    count = testmatrix[i, j] == randomizedmatrix[i, j] ? count : count + 1;
                    if(testmatrix[i,j] == 1)
                    {
                        columnsums[i]++;
                        rowsums[j]++;
                        columnsumsrandomized[i]++;
                        rowsumsrandomized[j]++;
                    }

                }

            }


            for (int i = 0; i < n; i++)
            {
                if (columnsums[i] != columnsumsrandomized[i])
                    Console.WriteLine("Auwas row");
                else
                {
                    Console.WriteLine("hmm" + i);
                }
            }
            for (int j = 0; j < m; j++)
            {
                if (rowsums[j] != rowsumsrandomized[j])
                    Console.WriteLine("Auwas column");
            }

            // Output result.
            Console.WriteLine(count);

            // Wait for key to exit.
            Console.ReadKey();
        }
    }
}
