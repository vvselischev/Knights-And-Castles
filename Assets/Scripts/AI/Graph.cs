using System.Collections.Generic;

namespace Assets.Scripts
{
    /// <summary>
    /// Stores graph of distances between all board cells
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Enumerate board cells
        /// </summary>
        private Dictionary<Cell, int> nodeIndexByCell = new Dictionary<Cell, int>();
        
        /// <summary>
        /// Stores graph edges
        /// </summary>
        private List<List<Edge>> edges = new List<List<Edge>>();
        
        /// <summary>
        /// Stores board for which graph was built
        /// </summary>
        private BlockBoardStorage boardStorage;
        
        /// <summary>
        /// Graph size
        /// </summary>
        private int size;
        
        /// <summary>
        /// Stores distances between all cells 
        /// </summary>
        private int[,] distance;

        public Graph(BlockBoardStorage boardStorage)
        {
            this.boardStorage = boardStorage;
            size = boardStorage.GetNumberOfCells();
            InitializeEdges();

            var index = 0;
            var cells = boardStorage.GetListOfCells();
            
            foreach (var cell in cells)
            {
                nodeIndexByCell.Add(cell, index);
                index++;
            }

            FillEdges(cells);
            CalculateDistances();
        }

        /// <summary>
        /// Returns distance between two cells
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int GetDistance(Cell from, Cell to)
        {
            var indexFrom = nodeIndexByCell[from];
            var indexTo = nodeIndexByCell[to];

            return distance[indexFrom, indexTo];
        }

        /// <summary>
        /// Calculates distances between all pairs of cells
        /// </summary>
        private void CalculateDistances()
        {
            distance = new int[size, size];
            for (var i = 0; i < size; i++)
            {
                CalculateDistancesFromVertex(i);
            }
        }

        /// <summary>
        /// Calculates distance from given vertex to all others using 0-1 BFS
        /// </summary>
        /// <param name="startIndex"></param>
        private void CalculateDistancesFromVertex(int startIndex) // 0-1 BFS
        {
            PrepareDistances(startIndex);            

            var bfsDequeue = new LinkedList<int>();
            bfsDequeue.AddLast(startIndex);

            while (bfsDequeue.Count > 0)
            {
                var vertex = bfsDequeue.First.Value;
                bfsDequeue.RemoveFirst();

                foreach (var neighbour in edges[vertex])
                {
                    if (distance[startIndex, neighbour.To] > distance[startIndex, vertex] + neighbour.Weight)
                    {
                        distance[startIndex, neighbour.To] = distance[startIndex, vertex] + neighbour.Weight;
                        if (neighbour.Weight == 0)
                        {
                            bfsDequeue.AddFirst(neighbour.To);
                        }
                        else
                        {
                            bfsDequeue.AddLast(neighbour.To);
                        }
                    }
                }
            }
        }

        private void PrepareDistances(int vertex)
        {
            for (var i = 0; i < size; i++)
            {
                distance[vertex, i] = size + 1; // Distance is always less than size + 1
            }

            distance[vertex, vertex] = 0;
        }

        /// <summary>
        /// Add edges to graph
        /// </summary>
        /// <param name="cells"></param>
        private void FillEdges(IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                var adjacent = boardStorage.GetAdjacent(cell);
                AddEdges(cell, adjacent);
            }

            var passes = boardStorage.GetPassesAsFromToCells();

            foreach (var pass in passes)
            {
                var from = pass.From;
                edges[nodeIndexByCell[from]].Clear();
                AddEdge(from, pass.To, 0);
            }
        }

        /// <summary>
        /// Adds edges to graph which are incidental to given vertex
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="adjacent"></param>
        private void AddEdges(Cell cell, IEnumerable<Cell> adjacent)
        {
            foreach (var adjacentCell in adjacent)
            {
                AddEdge(cell, adjacentCell, 1);
            }
        }

        /// <summary>
        /// Adds one edge
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="weight"></param>
        private void AddEdge(Cell from, Cell to, int weight)
        {
            var indexFrom = nodeIndexByCell[from];
            var indexTo = nodeIndexByCell[to];
            
            edges[indexFrom].Add(new Edge(indexFrom, indexTo, weight));
        }

        /// <summary>
        /// For each vertex creates empty list of edges
        /// </summary>
        private void InitializeEdges()
        {
            for (var i = 0; i < size; i++)
            {
                edges.Add(new List<Edge>());
            }
        }
    }
}