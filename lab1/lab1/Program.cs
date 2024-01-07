using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        for (int k = 8; k >= 2; k = k - 2)
        {
            for (int n = 1024; n <= 8192; n = n * 2)
            {
                int m = n;

                int[,] A = new int[n, m];
                int[,] B = new int[m, n];
                InitializeMatrix(A, n, m);
                InitializeMatrix(B, n, m);

                Stopwatch sequentialStopwatch = new Stopwatch();
                sequentialStopwatch.Start();
                SumMatricesSequential(A, B, n, m);
                sequentialStopwatch.Stop();
                
                Stopwatch parallelStopwatch = new Stopwatch();
                parallelStopwatch.Start();
                SumMatricesParallel(A, B, k, n, m);
                parallelStopwatch.Stop();
                
                Stopwatch subseq = new Stopwatch();
                subseq.Start();
                SubtractMatricesSequential(A, B, n, m);
                subseq.Stop();
                
                Stopwatch subparal = new Stopwatch();
                subparal.Start();
                SubtractMatricesParallel(A, B, k, n, m);
                subparal.Stop();
                
                double speedup = (double)sequentialStopwatch.ElapsedMilliseconds / parallelStopwatch.ElapsedMilliseconds;
                double efficiency = (speedup / k) * 100;
                double mspeedup = (double)subseq.ElapsedMilliseconds / subparal.ElapsedMilliseconds;
                double mefficiency = (mspeedup / k) * 100;

                Console.WriteLine($"+ || k: {k} || n: {n} || seq time: {sequentialStopwatch.ElapsedMilliseconds} ms || paral time: {parallelStopwatch.ElapsedMilliseconds} ms || S: {speedup} || E: {efficiency}%");
                Console.WriteLine($"- || k: {k} || n: {n} || seq time: {subseq.ElapsedMilliseconds} ms || paral time: {subparal.ElapsedMilliseconds} ms || S: {mspeedup} || E: {mefficiency}%");
            }
            Console.WriteLine('\t');
        }
        Console.ReadLine();
    }

    static void InitializeMatrix(int[,] matrix, int n, int m)
    {
        Random random = new Random();
        int rows = n;
        int cols = m;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = random.Next(1, 10);
            }
        }
    }

    static int[,] SumMatricesSequential(int[,] A, int[,] B, int n, int m)
    {
        int rows = n;
        int cols = m;

        int[,] result = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = A[i, j] + B[i, j];
            }
        }

        return result;
    }
    static int[,] SumMatricesParallel(int[,] A, int[,] B, int k, int n, int m)
    {
        int rows = n;
        int cols = m;

        int[,] result = new int[rows, cols];

        Parallel.For(0, rows, new ParallelOptions { MaxDegreeOfParallelism = k }, i =>
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = A[i, j] + B[i, j];
            }
        });

        return result;
    }

    static int[,] SubtractMatricesSequential(int[,] A, int[,] B, int n, int m)
    {
        int rows = n;
        int cols = m;

        int[,] result = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = A[i, j] - B[i, j];
            }
        }

        return result;
    }

    static int[,] SubtractMatricesParallel(int[,] A, int[,] B, int k, int n, int m)
    {
        int rows = n;
        int cols = m;

        int[,] result = new int[rows, cols];

        Parallel.For(0, rows, new ParallelOptions { MaxDegreeOfParallelism = k }, i =>
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = A[i, j] - B[i, j];
            }
        });

        return result;
    }


}
