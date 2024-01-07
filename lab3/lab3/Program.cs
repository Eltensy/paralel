using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        int n = 1000;
        int k = 8;

        double[,] A = new double[n, n];
        double[] b = new double[n];
        InitializeSystem(A, b);

        Stopwatch sequentialSolverStopwatch = new Stopwatch();
        sequentialSolverStopwatch.Start();
        double[] resultSequential = SolveLinearSystemSequential(A, b);
        sequentialSolverStopwatch.Stop();
        Console.WriteLine($"seq run time: {sequentialSolverStopwatch.ElapsedMilliseconds} ms");

        // Solve linear system (parallel algorithm)
        Stopwatch parallelSolverStopwatch = new Stopwatch();
        parallelSolverStopwatch.Start();
        double[] resultParallel = SolveLinearSystemParallel(A, b, k);
        parallelSolverStopwatch.Stop();
        Console.WriteLine($"paral run time with {k} threads: {parallelSolverStopwatch.ElapsedMilliseconds} ms");

        double speedup = (double)sequentialSolverStopwatch.ElapsedMilliseconds / parallelSolverStopwatch.ElapsedMilliseconds;
        double efficiency = speedup / k;

        Console.WriteLine($"speedup: {speedup:F2}");
        Console.WriteLine($"efficiency: {efficiency:F2}");
        Console.ReadLine();
    }

    static void InitializeSystem(double[,] A, double[] b)
    {
        Random random = new Random();
        int n = A.GetLength(0);

        for (int i = 0; i < n; i++)
        {
            b[i] = random.NextDouble() * 10;

            for (int j = 0; j < n; j++)
            {
                A[i, j] = random.NextDouble() * 10;
            }
        }
    }

    static double[] SolveLinearSystemSequential(double[,] A, double[] b)
    {
        int n = A.GetLength(0);
        double[] result = new double[n];

        for (int k = 0; k < n - 1; k++)
        {
            for (int i = k + 1; i < n; i++)
            {
                double factor = A[i, k] / A[k, k];
                for (int j = k; j < n; j++)
                {
                    A[i, j] -= factor * A[k, j];
                }
                b[i] -= factor * b[k];
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            result[i] = b[i];
            for (int j = i + 1; j < n; j++)
            {
                result[i] -= A[i, j] * result[j];
            }
            result[i] /= A[i, i];
        }

        return result;
    }

    static double[] SolveLinearSystemParallel(double[,] A, double[] b, int k)
    {
        int n = A.GetLength(0);
        double[] result = new double[n];
        Parallel.For(0, n - 1, new ParallelOptions { MaxDegreeOfParallelism = k }, i =>
        {
            for (int j = i + 1; j < n; j++)
            {
                double factor = A[j, i] / A[i, i];
                for (int x = i; x < n; x++)
                {
                    A[j, x] -= factor * A[i, x];
                }
                b[j] -= factor * b[i];
            }
        });

        for (int i = n - 1; i >= 0; i--)
        {
            result[i] = b[i];
            for (int j = i + 1; j < n; j++)
            {
                result[i] -= A[i, j] * result[j];
            }
            result[i] /= A[i, i];
        }

        return result;
    }
}
