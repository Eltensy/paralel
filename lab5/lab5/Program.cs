using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


class Program
{
    public static void Main()
    {
        for (int k = 8; k >= 2; k = k - 2)
        {
            for (int n = 32; n <= 1024; n = n * 2)
            {

                int[,] random_graph = new int[n, n];

                var floyd = new Floyd();
                var random_V = new List<V>();

                var random = new Random();
                floyd.Threads(k);

                floyd.RandomV(n, random, random_V);


                Stopwatch seqtime = new Stopwatch();
                seqtime.Start();
                floyd.InitGraph(random_graph, n, random_V);
                floyd.FloydAlgorithm(random_graph, n);
                seqtime.Stop();

                Stopwatch paraltime = new Stopwatch();
                paraltime.Start();
                floyd.InitGraph(random_graph, n, random_V);
                floyd.FloydAlgorithm_Parallel(random_graph, n);
                paraltime.Stop();

                double speedup = (double)seqtime.ElapsedMilliseconds / paraltime.ElapsedMilliseconds;
                double efficiency = (speedup / k)*100;
                Console.WriteLine($"k: {k} || n: {n} || seq time: {seqtime.ElapsedMilliseconds} ms || paral time: {paraltime.ElapsedMilliseconds} ms || S: {speedup} || E: {efficiency}%");

            }
            Console.WriteLine('\t');
        }
        Console.ReadLine();
    }
}

class V
{
    public int A0 { get; set; }
    public int An { get; set; }
    public int Value { get; set; }

    public V(int a0, int an, int value)
    {
        A0 = a0;
        An = an;
        Value = value;
    }
}

class Floyd
{
    int threads;
    ParallelOptions parallelOptions = new ParallelOptions();

    public Floyd()
    {
        threads = Environment.ProcessorCount;
        parallelOptions.MaxDegreeOfParallelism = threads;
    }

    public ParallelOptions Threads(int threads)
    {
        try
        {
            if (threads > 0)
            {
                parallelOptions.MaxDegreeOfParallelism = threads;
            }
            else
            {
                throw new Exception("Threads must be more than 0");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            threads = Environment.ProcessorCount;
            parallelOptions.MaxDegreeOfParallelism = threads;
        }
        return parallelOptions;
    }

    public void InitGraph(int[,] graph, int n, List<V> V)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    graph[i, j] = 0;
                }
                else
                {
                    graph[i, j] = int.MaxValue;
                }
            }
        }
        foreach (var v in V)
        {
            graph[v.A0 - 1, v.An - 1] = v.Value;
        }
    }

    public void FloydAlgorithm(int[,] graph, int n)
    {
        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (graph[i, k] != int.MaxValue && graph[k, j] != int.MaxValue &&
                        graph[i, k] + graph[k, j] < graph[i, j])
                    {
                        graph[i, j] = graph[i, k] + graph[k, j];
                    }
                }
            }
        }
    }

    public void FloydAlgorithm_Parallel(int[,] graph, int n)
    {

        Parallel.For(0, n, parallelOptions, k =>
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (graph[i, k] != int.MaxValue && graph[k, j] != int.MaxValue &&
                        graph[i, k] + graph[k, j] < graph[i, j])
                    {
                        graph[i, j] = graph[i, k] + graph[k, j];
                    }
                }
            }
        });
    }

    public void RandomV(int n, Random random, List<V> random_V)
    {
        for (int i = 0; i < n; i++)
        {
            int a0 = random.Next(1, n + 1);
            int an = random.Next(1, n + 1);
            int value = random.Next(1, 10);

            var v = new V(a0, an, value);
            random_V.Add(v);
        }
    }
}
