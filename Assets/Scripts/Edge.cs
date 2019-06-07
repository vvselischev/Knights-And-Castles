namespace Assets.Scripts
{
    /// <summary>
    /// Stores graph edge
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Edge weight
        /// </summary>
        public int Weight { get; }
        
        /// <summary>
        /// Edge to vertex's index
        /// </summary>
        public int To { get; }
        
        /// <summary>
        /// Edge from vertex's index
        /// </summary>
        public int From { get; }

        public Edge(int from, int to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }
}