using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RandomBinaryMatrixBuilder
{
    static class TestMatrixGenerator
    {
        /// <summary>
        /// The random generator we use for all the data.
        /// </summary>
        static Random random = new Random();

        /// <summary>
        /// Gets matrix with given width and height of binary values.
        /// </summary>
        /// <param name="width">Width of the matrix.</param>
        /// <param name="height">Height of the matrix.</param>
        /// <returns></returns>
        public static int[,] GetMatrix(int width, int height)
        {
            // Get datastructure for matrix.
            int[,] result = new int[width, height];

            // Fill matrix with binary random values.
            for(int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                    result[i, j] = random.Next(0, 2);
            
            return result;
        }
    }
}
