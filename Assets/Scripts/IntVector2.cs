using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class IntVector2 : IEquatable<IntVector2>, IComparer<IntVector2>
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(IntVector2 other)
        {
            if (other == null)
            {
                return false;
            }

            return x == other.x && y == other.y;
        }

        public int Compare(IntVector2 x, IntVector2 y)
        {
            if (x == null)
            {
                return -1;
            }

            if (x.Equals(y))
            {
                return 0;
            }

            return -1;
        }
    }
}