namespace Assets.Scripts
{
    public class Edge
    {
        public int Weight { get; }
        public int To { get; }
        public int From { get; }

        public Edge(int from, int to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }
}