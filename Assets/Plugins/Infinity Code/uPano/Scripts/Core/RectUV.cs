/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;

namespace InfinityCode.uPano
{
    /// <summary>
    /// UV of texture side
    /// </summary>
    [Serializable]
    public struct RectUV
    {
        /// <summary>
        /// Side labels
        /// </summary>
        public static string[] sides = {"Left", "Top", "Right", "Bottom"};

        /// <summary>
        /// Left texture U
        /// </summary>
        public float left;

        /// <summary>
        /// Top texture V
        /// </summary>
        public float top;

        /// <summary>
        /// Right textrue U
        /// </summary>
        public float right;

        /// <summary>
        /// Bottom texture V
        /// </summary>
        public float bottom;

        /// <summary>
        /// UV whole texture
        /// </summary>
        public static RectUV full
        {
            get { return new RectUV(0, 1, 1, 0); }
        }

        /// <summary>
        /// Height of texture V
        /// </summary>
        public float height
        {
            get { return top - bottom; }
        }

        /// <summary>
        /// Width of texture U
        /// </summary>
        public float width
        {
            get { return right - left; }
        }

        /// <summary>
        /// Get and set side by index
        /// </summary>
        /// <param name="index">Index of side</param>
        /// <returns>Value of the side</returns>
        public float this[int index]
        {
            get
            {
                if (index == 0) return left;
                if (index == 1) return top;
                if (index == 2) return right;
                if (index == 3) return bottom;
                throw new Exception("Index out of range (0-3)");
            }
            set
            {
                if (index == 0)
                {
                    left = value;
                    return;
                }

                if (index == 1)
                {
                    top = value;
                    return;
                }

                if (index == 2)
                {
                    right = value;
                    return;
                }
                if (index == 3)
                {
                    bottom = value;
                    return;
                }
                throw new Exception("Index out of range (0-3)");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="left">Left texture U</param>
        /// <param name="top">Top texture V</param>
        /// <param name="right">Right texture U</param>
        /// <param name="bottom">Bottom texture V</param>
        public RectUV(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }
}