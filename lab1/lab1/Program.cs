using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        int n = 512;
        int m = 512;
        int k = 8;

        int[,] A = new int[n, m];
        int[,] B = new int[m, n];
        InitializeMatrix(A, n, m);
        InitializeMatrix(B, n, m);

        Stopwatch sequentialStopwatch = new Stopwatch();
        sequentialStopwatch.Start();
        int[,] resultSequential = SumMatricesSequential(A, B, n, m);
        sequentialStopwatch.Stop();
        Console.WriteLine($"+ seq run time = {sequentialStopwatch.ElapsedMilliseconds} ms");

        Stopwatch parallelStopwatch = new Stopwatch();
        parallelStopwatch.Start();
        int[,] resultParallel = SumMatricesParallel(A, B, k, n, m);
        parallelStopwatch.Stop();
        Console.WriteLine($"+ paral run time with {k} threads: {parallelStopwatch.ElapsedMilliseconds} ms");

        Stopwatch subseq = new Stopwatch();
        subseq.Start();
        int[,] ressubseq = SubtractMatricesSequential(A, B, n, m);
        subseq.Stop();
        Console.WriteLine($"- seq run time: {subseq.ElapsedMilliseconds} ms");

        Stopwatch subparal = new Stopwatch();
        subparal.Start();
        int[,] ressubparal = SubtractMatricesParallel(A, B, k, n, m);
        subparal.Stop();
        Console.WriteLine($"- paral run time with {k} threads: {subparal.ElapsedMilliseconds} ms");

        double speedup = (double)sequentialStopwatch.ElapsedMilliseconds / parallelStopwatch.ElapsedMilliseconds;
        double efficiency = speedup / k;

        Console.WriteLine($"speedup = {speedup:F2}");
        Console.WriteLine($"efficiency = {efficiency:F2}");
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
    public double Efficiency(double acceleration, int threads)
    {
        double efficiency = acceleration / threads * 100;
        return efficiency;
    }
    public double Acceleration(Stopwatch stopwatch1, Stopwatch stopwatch2)
    {
        double time1 = (double)stopwatch1.ElapsedMilliseconds;
        double time2 = (double)stopwatch2.ElapsedMilliseconds;
        double boost = time1 / time2;
        return boost;
    }


}
