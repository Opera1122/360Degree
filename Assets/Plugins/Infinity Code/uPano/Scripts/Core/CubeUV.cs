/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// UV of texture for cube sides
    /// </summary>
    [Serializable]
    public class CubeUV: IEnumerable
    {
        /// <summary>
        /// Side labels
        /// </summary>
        public static string[] sides = { "Top", "Front", "Left", "Back", "Right", "Bottom" };

        /// <summary>
        /// UV of top side
        /// </summary>
        public RotatableRectUV top;

        /// <summary>
        /// UV of front side
        /// </summary>
        public RotatableRectUV front;

        /// <summary>
        /// UV of left side
        /// </summary>
        public RotatableRectUV left;

        /// <summary>
        /// UV of back side
        /// </summary>
        public RotatableRectUV back;

        /// <summary>
        /// UV of right side
        /// </summary>
        public RotatableRectUV right;

        /// <summary>
        /// UV of bottom side
        /// </summary>
        public RotatableRectUV bottom;

        /// <summary>
        /// Get and set UV of side by index
        /// </summary>
        /// <param name="index">Index of side</param>
        /// <returns>UV of side</returns>
        public RotatableRectUV this[int index]
        {
            get
            {
                if (index == 0) return top;
                if (index == 1) return front;
                if (index == 2) return left;
                if (index == 3) return back;
                if (index == 4) return right;
                if (index == 5) return bottom;
                throw new Exception("Index out of range (0-5)");
            }
            set
            {
                if (index == 0)
                {
                    top = value;
                    return;
                }

                if (index == 1)
                {
                    front = value;
                    return;
                }

                if (index == 2)
                {
                    left = value;
                    return;
                }
                if (index == 3)
                {
                    back = value;
                    return;
                }
                if (index == 4)
                {
                    right = value;
                    return;
                }
                if (index == 5)
                {
                    bottom = value;
                    return;
                }
                throw new Exception("Index out of range (0-5)");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="top">UV of top side</param>
        /// <param name="front">UV of front side</param>
        /// <param name="left">UV of left side</param>
        /// <param name="back">UV of back side</param>
        /// <param name="right">UV of right side</param>
        /// <param name="bottom">UV of bottom side</param>
        public CubeUV(RotatableRectUV top, RotatableRectUV front, RotatableRectUV left, RotatableRectUV back, RotatableRectUV right, RotatableRectUV bottom)
        {
            this.top = top;
            this.front = front;
            this.left = left;
            this.back = back;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>
        /// Returns the side index by UV
        /// </summary>
        /// <param name="uv">UV</param>
        /// <returns>Side index</returns>
        public int GetSideIndex(Vector2 uv)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this[i].Contain(uv)) return i;
            }

            return -1;
        }

        public IEnumerator GetEnumerator()
        {
            yield return top;
            yield return front;
            yield return left;
            yield return back;
            yield return right;
            yield return bottom;
        }
    }
}