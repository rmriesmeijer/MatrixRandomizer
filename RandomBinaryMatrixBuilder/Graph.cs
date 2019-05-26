using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBinaryMatrixBuilder
{
    class Graph
    {
        public Vertex[] Vertices;
        public Edge[] Edges;
        private Vertex Source;
        private Vertex Sink;
        private int nz;
        private int m;
        private int n;

        /// <summary>
        /// INPUT: A set of row and column sums to make a binary matrix for.
        /// OUTPUT: A flow network that models the content of a binary matrix which would have the wanted row and column sums.
        /// </summary>
        public Graph(int[] rowsums, int[] colsums)
        {
            // Get dimensions to work with from input matrix.
            m = rowsums.Length;
            n = colsums.Length;
            for (int j = 0; j < colsums.Length; j++)
                nz += colsums[j];

            // Build vertices.
            Vertices = new Vertex[m + n + 2];
            for (int i = 0; i < m + n + 2; i++)
            {
                Vertices[i] = new Vertex(i);
            }
            Source = Vertices[m + n];
            Sink = Vertices[m + n + 1];

            // Build edges.
            Edges = new Edge[m * n + m + n];
            int id = 0;
            for (int i = 0; i < m; i++)
            {
                Edges[id] = new Edge(m + n, i, rowsums[i], -1);
                Vertices[m + n].AdjacentVertices.Add(Edges[id]);
                Vertices[i].AdjacentVertices.Add(Edges[id++]);
            }
            for (int j = m; j < m + n; j++)
            {
                Edges[id] = new Edge(j, m + n + 1, colsums[j - m], -1);
                Vertices[j].AdjacentVertices.Add(Edges[id]);
                Vertices[m + n + 1].AdjacentVertices.Add(Edges[id++]);
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = m; j < m + n; j++)
                {
                    Edges[id] = new Edge(i, j, 1, 0);
                    Vertices[i].AdjacentVertices.Add(Edges[id]);
                    Vertices[j].AdjacentVertices.Add(Edges[id++]);
                }
            }
        }

        /// <summary>
        /// Modeled after the python code on https://en.wikipedia.org/wiki/Ford%E2%80%93Fulkerson_algorithm.
        /// </summary>
        /// <param name="Parent"></param>
        /// <returns></returns>
        public bool BFS(Vertex[] Parent, int MaxID)
        {
            // Mark all vertices as not visited.
            bool[] visited = new bool[Vertices.Length];

            // Create a queue for BFS.
            Queue<Vertex> queue = new Queue<Vertex>();

            // Mark the source node as visited and enqueue it.
            queue.Enqueue(Source);
            visited[Source.ID] = true;

            // Standard BFS loop.
            while (queue.Count > 0 && visited[Sink.ID] == false)
            {
                Vertex u = queue.Dequeue();

                // Get all adjacent vertices of the dequeued vertex u
                // If an adjacent vertex has not been visited, then mark it
                // visited and enqueue it.
                foreach (Edge e in u.AdjacentVertices)
                {
                    if (e.ID <= MaxID) {
                        if (e.From == u.ID) // Forward edge.
                        {
                            if (visited[e.To] == false && e.Capacity != e.Flow)
                            {
                                queue.Enqueue(Vertices[e.To]);
                                visited[e.To] = true;
                                Parent[e.To] = u;
                            }
                        }
                        else // Back edge.
                        {
                            if (visited[e.From] == false && e.Flow > 0)
                            {
                                queue.Enqueue(Vertices[e.From]);
                                visited[e.From] = true;
                                Parent[e.From] = u;
                            }
                        }
                    }
                }
            }

            // If we reached the sink in BFS starting from source, then return true, else false.
            return visited[Sink.ID];
        }

        /// <summary>
        /// Modeled after the python code on https://en.wikipedia.org/wiki/Ford%E2%80%93Fulkerson_algorithm.
        /// </summary>
        /// <returns></returns>
        public int EdmondsKarp(int MaxID = 0)
        {
            foreach(Edge e in Edges)
            {
                e.Flow = 0;
            }
            // This array is filled by BFS to store flow augmenting paths.
            Vertex[] Parent = new Vertex[Vertices.Length];

            // Set initial flow.
            int max_flow = 0;

            // Augment the flow while there is a path from source to sink, becaues of the network any augmentation will add at most 1.
            while(this.BFS(Parent, MaxID))
            {
                // Update flow network.
                Vertex s = Sink;
                while(s.ID != Source.ID)
                {
                    foreach(Edge e in Edges)
                    {
                        if(e.From == Parent[s.ID].ID && e.To == s.ID)
                        {
                            e.Flow++;
                        }
                        else if(e.To == Parent[s.ID].ID && e.From == s.ID)
                        {
                            e.Flow--;
                        }
                    }
                    s = Parent[s.ID];
                }

                // Add path flow to overall flow.
                max_flow++;
            }

            return max_flow;
        }

        public List<int> FindFirstMaxFlowEdge(List<int> FoundEdgeIDs)
        {
            for(int k = 0; k < Edges.Length; k++)
            {
                int a = EdmondsKarp(k);
                if (a == nz - FoundEdgeIDs.Count)
                {
                    FoundEdgeIDs.Add(k);
                    return FoundEdgeIDs;
                }
            }
            return FoundEdgeIDs;
        }

        public int[,] BuildBinaryMatrix()
        {
            int id = 0;
            for(int e = 0; e < Edges.Length; e++)
            {
                if (Edges[e].ID != -1)
                {
                    Edges[e].ID = id++;
                }
            }

            List<int> edgelist = new List<int>();
            for(int i = 0; i < nz; i++)
            {
                edgelist = FindFirstMaxFlowEdge(edgelist);
            }

            int[,] matrix = new int[m, n];

            foreach(int c in edgelist)
            {
                foreach(Edge e in Edges)
                {
                    if (e.ID == c)
                    {
                        matrix[e.From, e.To - m] = 1;
                    }
                }
            }

            return matrix;
        }


        public int[,] GetRandomMatrix()
        {
            KnuthShuffle(Edges);
            return BuildBinaryMatrix();
        }

        /// <summary>
        /// Code taken from: https://www.rosettacode.org/wiki/Knuth_shuffle#C.23
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void KnuthShuffle<T>(T[] array)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < array.Length; i++)
            {
                int j = random.Next(i, array.Length); // Don't select from the entire array on subsequent loops
                T temp = array[i]; array[i] = array[j]; array[j] = temp;
            }
        }
    }

    public class Vertex
    {
        public List<Edge> AdjacentVertices = new List<Edge>();
        public int ID;

        public Vertex(int id)
        {
            this.ID = id;
        }
    }

    public class Edge
    {
        public int From;
        public int To;
        public int Capacity;
        public int Flow;
        public int ID;

        public Edge(int from, int to, int capacity, int id)
        {
            this.From = from;
            this.To = to;
            this.Capacity = capacity;
            this.Flow = 0;
            this.ID = id;
        }
    }
}
