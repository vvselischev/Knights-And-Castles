using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Stores point in Z^2
    /// </summary>
    public class IntVector2 : IEquatable<IntVector2>, IComparer<IntVector2>
    {
        /// <summary>
        /// Stores x coordinate
        /// </summary>
        public int x;
        
        /// <summary>
        /// Stores y coordinate
        /// </summary>
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Checks that two points have the same coordinates
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IntVector2 other)
        {
            if (other == null)
            {
                return false;
            }

            return x == other.x && y == other.y;
        }

        /// <summary>
        /// Converts vector to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        /// <summary>
        /// Creates new vector with same coordinates
        /// </summary>
        /// <returns></returns>
        public IntVector2 CloneVector()
        {
            return new IntVector2(x, y);
        }

        /// <summary>
        /// Compares two vectors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Increments x coordinate
        /// </summary>
        /// <returns></returns>
        public IntVector2 IncrementX()
        {
            return new IntVector2(x + 1, y);
        }

        /// <summary>
        /// Increments y coordinate
        /// </summary>
        /// <returns></returns>
        public IntVector2 IncrementY()
        {
            return new IntVector2(x, y + 1);
        }

        /// <summary>
        /// Decrements x coordinate
        /// </summary>
        /// <returns></returns>
        public IntVector2 DecrementX()
        {
            return new IntVector2(x - 1, y);
        }

        /// <summary>
        /// Decrements y coordinate
        /// </summary>
        /// <returns></returns>
        public IntVector2 DecrementY()
        {
            return new IntVector2(x, y - 1);
        }
    }
}