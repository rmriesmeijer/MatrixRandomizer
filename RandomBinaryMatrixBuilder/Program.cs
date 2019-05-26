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
<<<<<<< HEAD
            int m = 10;
            int n = 10;
=======
            int n = 100;
            int m = 100;
>>>>>>> origin/master
            int[,] testmatrix = TestMatrixGenerator.GetMatrix(n, m);

            // Randomize matrix.
            int[,] randomizedmatrix = MatrixRandomizer.Randomize(testmatrix);

            // Count differences and row and column sums.
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
                    if (testmatrix[i, j] == 1)
                    {
                        columnsums[i]++;
                        rowsums[j]++;
                    }
                    if (randomizedmatrix[i, j] == 1)
                    {
                        columnsumsrandomized[i]++;
                        rowsumsrandomized[j]++;
                    }
                }

            }

<<<<<<< HEAD
            Graph g = new Graph(rowsums, columnsums);
            testmatrix = g.GetRandomMatrix();



            columnsumsrandomized = new int[n];
            rowsumsrandomized = new int[m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (randomizedmatrix[i, j] == 1)
                    {
                        columnsumsrandomized[i]++;
                        rowsumsrandomized[j]++;
                    }
                }

            }

=======
            // Check whether there is no difference in column and row sums.
>>>>>>> origin/master
            bool perfect = true;
            for (int i = 0; i < n; i++)
                if (columnsums[i] != columnsumsrandomized[i])
<<<<<<< HEAD
                    Console.WriteLine(columnsumsrandomized[i]);
            }
=======
                    perfect = false;
>>>>>>> origin/master
            for (int j = 0; j < m; j++)
                if (rowsums[j] != rowsumsrandomized[j])
                    perfect = false;

            // Output .dat files for matlab examination.
            List<string> lines = new List<string>();
            List<string> lines2 = new List<string>();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                {
                    if (testmatrix[i, j] == 1)
                    {
                        lines.Add((i+1) + "\t" + (j + 1) + "\t" + "1.000000000000000");
                    }

                    if (randomizedmatrix[i, j] == 1)
                    {
                        lines2.Add((i + 1) + "\t" + (j + 1) + "\t" + "1.000000000000000");
                    }
                }
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\rmrie\Desktop\data.dat"))
            {
                file.WriteLine(n + "\t" + m + "\t" + lines.Count);
                foreach (string line in lines)
                    file.WriteLine(line);
            }
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\rmrie\Desktop\datarand.dat"))
            {
                file.WriteLine(n + "\t" + m + "\t" + lines2.Count);
                foreach (string line in lines2)
                    file.WriteLine(line);
            }

            // Output result.
            if (perfect)
                Console.WriteLine("Perfect transformation with " + count + " value differences.");
            else
                Console.WriteLine("Not a perfect transformation");

            // Wait for key to exit.
            Console.ReadKey();
        }
    }
}
