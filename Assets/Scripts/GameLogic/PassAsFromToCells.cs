namespace Assets.Scripts
{
    /// <summary>
    /// Stores information about pass
    /// </summary>
    public class PassAsFromToCells
    {
        /// <summary>
        /// From cell of pass
        /// </summary>
        public Cell From { get; }
        
        /// <summary>
        /// To cell of path
        /// </summary>
        public Cell To { get; }

        public PassAsFromToCells(Cell from, Cell to)
        {
            From = from;
            To = to;
        }
    }
}