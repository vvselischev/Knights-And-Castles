using UnityEngine;

namespace Assets.Scripts
{
    public class MoveInformation
    {
        public IntVector2 From { get; set; }

        public IntVector2 To { get; set; }

        public double benefit;

        public MoveInformation(IntVector2 from, IntVector2 to)
        {
            this.From = from;
            this.To = to;
        }
    }
}