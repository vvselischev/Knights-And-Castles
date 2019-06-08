namespace Assets.Scripts
{
    /// <summary>
    /// Stores the move for AI.
    /// </summary>
    public class MoveInformation
    {
        public Cell From { get; }

        public Cell To { get; }

        public double benefit;

        public MoveInformation(Cell from, Cell to)
        {
            From = from;
            To = to;
        }
    }
}