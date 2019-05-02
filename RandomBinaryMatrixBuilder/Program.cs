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
            int n = 100;
            int m = 10000;
            int[,] testmatrix = TestMatrixGenerator.GetMatrix(n, m);

            // Randomize matrix.
            int[,] randomizedmatrix = MatrixRandomizer.Randomize(testmatrix);

            // Count differences.
            int count = 0;
            for(int i = 0; i < n; i++)
                for(int j = 0; j < m; j++)
                    count = testmatrix[i, j] == randomizedmatrix[i, j] ? count : count + 1;

            // Output result.
            Console.WriteLine(count);

            // Wait for key to exit.
            Console.ReadKey();
        }
    }
}
