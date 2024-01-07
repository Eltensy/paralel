using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


class Program
{
    static void Main(string[] args)
    {
        for (int k = 8; k >= 2; k = k - 2)
        {
            for (int n = 128; n <= 8192; n = n * 2)
            {
                int startVertex = 0;

                var prim = new PrimAlgorithm();
                prim.Threads(k);
                int[,] graph;
                graph = prim.RandomGraph(n);

                Stopwatch seqtime = new Stopwatch();
                seqtime.Start();
                prim.Prim(graph, startVertex);
                seqtime.Stop();

                
                Stopwatch paraltime = new Stopwatch();
                paraltime.Start();
                prim.PrimParallel(graph, startVertex);
                paraltime.Stop();

                
                double speedup = (double)seqtime.ElapsedMilliseconds / paraltime.ElapsedMilliseconds;
                double efficiency = (speedup / k) * 100;

                Console.WriteLine($"k: {k} || n: {n} || seq time: {seqtime.ElapsedMilliseconds} ms || paral time: {paraltime.ElapsedMilliseconds} ms || S: {speedup} || E: {efficiency}%");
            }
            Console.WriteLine('\t');
        }
        Console.ReadLine();
    }
}
class PrimAlgorithm
{
    private int V;
    private int startVertex;
    ParallelOptions parallelOptions = new ParallelOptions();

    public PrimAlgorithm() { }
    public ParallelOptions Threads(int threads)
    {
        parallelOptions.MaxDegreeOfParallelism = threads;
        return parallelOptions;
    }

    private int MinDistance(int[] distance, bool[] shortestPath)
    {
        int minDistance = int.MaxValue;
        int minIndex = int.MaxValue;

        for (int v = 0; v < V; v++)
        {
            if (!shortestPath[v] && distance[v] <= minDistance)
            {
                minDistance = distance[v];
                minIndex = v;
            }
        }
        if (minIndex != int.MaxValue)
        {
            shortestPath[minIndex] = true;
        }

        return minIndex;
    }

    public void Prim(int[,] graph, int startVertex)
    {
        V = graph.GetLength(0);
        this.startVertex = startVertex;

        int[] parent = new int[V];
        int[] distance = new int[V];
        bool[] shortestPath = new bool[V];

        for (int i = 0; i < V; i++)
        {
            distance[i] = int.MaxValue;
            shortestPath[i] = false;
        }

        distance[startVertex] = 0;
        parent[startVertex] = -1;

        for (int count = 0; count < V - 1; count++)
        {
            int minIndex = MinDistance(distance, shortestPath);

            for (int v = 0; v < V; v++)
            {
                if (graph[minIndex, v] != 0 && !shortestPath[v] && graph[minIndex, v] < distance[v])
                {
                    parent[v] = minIndex;
                    distance[v] = graph[minIndex, v];
                }
            }
        }

    }

    public void PrimParallel(int[,] graph, int startVertex)
    {
        V = graph.GetLength(0);
        this.startVertex = startVertex;

        int[] parent = new int[V];
        int[] distance = new int[V];
        bool[] shortestPath = new bool[V];

        Parallel.For(0, V, parallelOptions, i =>
        {
            distance[i] = int.MaxValue;
            shortestPath[i] = false;
        });

        distance[startVertex] = 0;
        parent[startVertex] = -int.MaxValue;

        Parallel.For(0, V - 1, parallelOptions, count =>
        {
            int minIndex = MinDistance(distance, shortestPath);

            for (int v = 0; v < V; v++)
            {
                if (graph[minIndex, v] != 0 && !shortestPath[v] && graph[minIndex, v] < distance[v])
                {
                    parent[v] = minIndex;
                    distance[v] = graph[minIndex, v];
                }
            }
        });

    }

    public int[,] RandomGraph(int vertices)
    {
        Random random = new Random();
        int[,] graph = new int[vertices, vertices];

        for (int i = 0; i < vertices; i++)
        {
            for (int j = 0; j < vertices; j++)
            {
                if (i == j)
                {
                    graph[i, j] = 0;
                }
                else
                {
                    int value = random.Next(1, 100);
                    graph[i, j] = value;
                    graph[j, i] = value;
                }
            }
        }

        return graph;
    }

    public double Acceleration(Stopwatch stopwatch1, Stopwatch stopwatch2)
    {
        double time1 = (double)stopwatch1.ElapsedMilliseconds;
        double time2 = (double)stopwatch2.ElapsedMilliseconds;
        double boost = time1 / time2;
        return boost;
    }

    public double Efficiency(double acceleration, int threads)
    {
        double efficiency = acceleration / threads * 100;
        return efficiency;
    }
}