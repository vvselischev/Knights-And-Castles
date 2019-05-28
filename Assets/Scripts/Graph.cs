using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Graph
    {
        private Dictionary<Cell, int> nodeIndexByCell = new Dictionary<Cell, int>();
        private List<List<Edge>> edges = new List<List<Edge>>();
        private BlockBoardStorage boardStorage;
        private int size;
        private int[,] distance;

        public Graph(BlockBoardStorage boardStorage)
        {
            this.boardStorage = boardStorage;
            size = boardStorage.GetNumberOfCells();
            InitializeEdges();

            int index = 0;
            var cells = boardStorage.GetListOfCells();
            
            foreach (var cell in cells)
            {
                nodeIndexByCell.Add(cell, index);
                index++;
            }

            FillEdges(cells);
            CalculateDistances();
        }

        public int GetDistance(Cell from, Cell to)
        {
            int indexFrom = nodeIndexByCell[from];
            int indexTo = nodeIndexByCell[to];

            return distance[indexFrom, indexTo];
        }

        private void CalculateDistances()
        {
            distance = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                CalculateDistancesFromVertex(i);
            }
        }

        private void CalculateDistancesFromVertex(int startIndex) // 0-1 BFS
        {
            for (int i = 0; i < size; i++)
            {
                distance[startIndex, i] = size + 1; // Distance is always less than size + 1
            }

            distance[startIndex, startIndex] = 0;

            var bfsDequeue = new LinkedList<int>();
            bfsDequeue.AddLast(startIndex);

            while (bfsDequeue.Count > 0)
            {
                int vertex = bfsDequeue.First.Value;
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

        private void FillEdges(List<Cell> cells)
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

        private void AddEdges(Cell cell, List<Cell> adjacent)
        {
            foreach (var adjacentCell in adjacent)
            {
                AddEdge(cell, adjacentCell, 1);
            }
        }

        
        private void AddEdge(Cell from, Cell to, int weight)
        {
            
            int indexFrom = nodeIndexByCell[from];
            int indexTo = nodeIndexByCell[to];
            
            edges[indexFrom].Add(new Edge(indexFrom, indexTo, weight));
        }

        private void InitializeEdges()
        {
            for (int i = 0; i < size; i++)
            {
                edges.Add(new List<Edge>());
            }
        }
    }
}