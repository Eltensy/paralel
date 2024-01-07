using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        var startVertex = 2;
        var k = 8;
        var n = 10000;

        var dijkstra = new DijkstraAlgorithm();
        dijkstra.Threads(k);
        int[,] graph = dijkstra.RandomGraph(n);


        Stopwatch seqtime = new Stopwatch();
        seqtime.Start();
        dijkstra.Dijkstra(graph, startVertex);
        seqtime.Stop();

        Console.WriteLine($"dijkstra seq run time: {seqtime.ElapsedMilliseconds} ms");

        Stopwatch paraltime = new Stopwatch();
        paraltime.Start();
        dijkstra.DijkstraParallel(graph, startVertex);
        paraltime.Stop();

        Console.WriteLine($"dijkstra paral run time with {k} threads: {paraltime.ElapsedMilliseconds} ms");

        double acceleration = dijkstra.Acceleration(seqtime, paraltime);
        Console.WriteLine($"speedup: {acceleration.ToString("f2")}");
        double dijkstra_efficiency = dijkstra.Efficiency(acceleration, k);
        Console.WriteLine($"efficiency: {dijkstra_efficiency.ToString("f2")}%");
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