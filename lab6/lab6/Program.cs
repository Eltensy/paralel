using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        for (int k = 8; k >= 2; k = k - 2)
        {
            for (int n = 128; n <= 8192; n = n * 2)
            {
                var startVertex = 2;

                var dijkstra = new DijkstraAlgorithm();
                dijkstra.Threads(k);
                int[,] graph = dijkstra.RandomGraph(n);


                Stopwatch seqtime = new Stopwatch();
                seqtime.Start();
                dijkstra.Dijkstra(graph, startVertex);
                seqtime.Stop();

               
                Stopwatch paraltime = new Stopwatch();
                paraltime.Start();
                dijkstra.DijkstraParallel(graph, startVertex);
                paraltime.Stop();

              
                double speedup = (double)seqtime.ElapsedMilliseconds / paraltime.ElapsedMilliseconds;
                double efficiency = (speedup / k) * 100;

                Console.WriteLine($"k: {k} || n: {n} || seq time: {seqtime.ElapsedMilliseconds} ms || paral time: {paraltime.ElapsedMilliseconds} ms || S: {speedup} || E: {efficiency}%");

            }
        }
        Console.ReadLine();
    }

}
class DijkstraAlgorithm
{
    private int V;

    ParallelOptions parallelOptions = new ParallelOptions();

    public DijkstraAlgorithm() { }
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

    public void Dijkstra(int[,] graph, int startVertex)
    {
        V = graph.GetLength(0);
        int[] distance = new int[V];
        bool[] shortestPath = new bool[V];

        for (int i = 0; i < V; i++)
        {
            distance[i] = int.MaxValue;
            shortestPath[i] = false;
        }

        distance[startVertex - 1] = 0;

        for (int count = 0; count < V - 1; count++)
        {
            int minIndex = MinDistance(distance, shortestPath);

            for (int v = 0; v < V; v++)
            {
                if (!shortestPath[v] && graph[minIndex, v] != 0 && distance[minIndex] != int.MaxValue &&
                    distance[minIndex] + graph[minIndex, v] < distance[v])
                {
                    distance[v] = distance[minIndex] + graph[minIndex, v];
                }
            }
        }
    }

    public void DijkstraParallel(int[,] graph, int startVertex)
    {
        V = graph.GetLength(0);
        int[] distance = new int[V];
        bool[] shortestPath = new bool[V];

        Parallel.For(0, V, parallelOptions, i =>
        {
            distance[i] = int.MaxValue;
            shortestPath[i] = false;
        });

        distance[startVertex - 1] = 0;

        Parallel.For(0, V - 1, parallelOptions, count =>
        {
            int minIndex = MinDistance(distance, shortestPath);

            for (int v = 0; v < V; v++)
            {
                if (!shortestPath[v] && graph[minIndex, v] != 0 &&
                    distance[minIndex] != int.MaxValue &&
                    distance[minIndex] + graph[minIndex, v] < distance[v])
                {
                    distance[v] = distance[minIndex] + graph[minIndex, v];
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

}