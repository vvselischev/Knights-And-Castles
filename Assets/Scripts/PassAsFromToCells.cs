namespace Assets.Scripts
{
    public class PassAsFromToCells
    {
        public Cell From { get; }
        public Cell To { get; }

        public PassAsFromToCells(Cell from, Cell to)
        {
            From = from;
            To = to;
        }
    }
}