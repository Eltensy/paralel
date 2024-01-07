using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        for (int k = 8; k >= 2; k = k - 2)
        {
            for (int n = 32; n <= 1024; n = n * 2)
            {
                int m = n;

                int[,] A = new int[n, m];
                int[,] B = new int[m, n];
                InitializeMatrix(A);
                InitializeMatrix(B);

                Stopwatch sequentialStopwatch = new Stopwatch();
                sequentialStopwatch.Start();
                MultiplyMatricesSequential(A, B);
                sequentialStopwatch.Stop();

                Stopwatch parallelStopwatch = new Stopwatch();
                parallelStopwatch.Start();
                MultiplyMatricesParallel(A, B, k);
                parallelStopwatch.Stop();

                double speedup = (double)sequentialStopwatch.ElapsedMilliseconds / parallelStopwatch.ElapsedMilliseconds;
                double efficiency = speedup / k;

                Console.WriteLine($"k: {k} || n: {n} || seq time: {sequentialStopwatch.ElapsedMilliseconds} ms || paral time: {parallelStopwatch.ElapsedMilliseconds} ms || S: {speedup} || E: {efficiency}");
            }
        }
        Console.ReadLine();
    }

    static void InitializeMatrix(int[,] matrix)
    {
        Random random = new Random();
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = random.Next(1, 10);
            }
        }
    }

    static int[,] MultiplyMatricesSequential(int[,] A, int[,] B)
    {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int colsB = B.GetLength(1);

        int[,] result = new int[rowsA, colsB];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                for (int k = 0; k < colsA; k++)
                {
                    result[i, j] += A[i, k] * B[k, j];
                }
            }
        }

        return result;
    }


    static int[,] MultiplyMatricesParallel(int[,] A, int[,] B, int k)
    {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int colsB = B.GetLength(1);

        int[,] result = new int[rowsA, colsB];

        Parallel.For(0, rowsA, new ParallelOptions { MaxDegreeOfParallelism = k }, i =>
        {
            for (int j = 0; j < colsB; j++)
            {
                for (int l = 0; l < colsA; l++)
                {
                    result[i, j] += A[i, l] * B[l, j];
                }
            }
        });

        return result;
    }

}
