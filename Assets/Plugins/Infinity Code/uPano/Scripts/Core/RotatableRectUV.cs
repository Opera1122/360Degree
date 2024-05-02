/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Enums;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// Rotatable UV of texture side
    /// </summary>
    [Serializable]
    public struct RotatableRectUV
    {
        /// <summary>
        /// Enum of rotation angles
        /// </summary>
        public enum Rotation
        {
            /// <summary>
            /// 0 degrees
            /// </summary>
            rotation0,

            /// <summary>
            /// 90 degrees
            /// </summary>
            rotation90,

            /// <summary>
            /// 180 degrees
            /// </summary>
            rotation180,

            /// <summary>
            /// 270 degrees
            /// </summary>
            rotation270
        }

        /// <summary>
        /// Left texture U
        /// </summary>
        public float left;

        /// <summary>
        /// Top texture V
        /// </summary>
        public float top;

        /// <summary>
        /// Right texture U
        /// </summary>
        public float right;

        /// <summary>
        /// Bottom texture V
        /// </summary>
        public float bottom;

        /// <summary>
        /// Rotation of UV
        /// </summary>
        public Rotation rotation;

        /// <summary>
        /// UV whole texture
        /// </summary>
        public static RotatableRectUV full
        {
            get { return new RotatableRectUV(0, 1, 1, 0); }
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
        /// <param name="rotation">Rotation of UV</param>
        public RotatableRectUV(float left, float top, float right, float bottom, Rotation rotation = Rotation.rotation0)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.rotation = rotation;
        }

        /// <summary>
        /// Checks the current rect contains this UV
        /// </summary>
        /// <param name="uv">UV</param>
        /// <returns>True - current rect contains UV, false - otherwise</returns>
        public bool Contain(Vector2 uv)
        {
            float min, max;
            if (left < right)
            {
                min = left;
                max = right;
            }
            else
            {
                min = right;
                max = left;
            }

            if (uv.x < min || uv.x > max) return false;

            if (top < bottom)
            {
                min = top;
                max = bottom;
            }
            else
            {
                min = bottom;
                max = top;
            }

            return uv.y >= min && uv.y <= max;
        }

        /// <summary>
        /// Returns UV at normalized coordinates for this rect
        /// </summary>
        /// <param name="rx">Relative X (0-1)</param>
        /// <param name="ry">Relative Y (0-1)</param>
        /// <returns>UV</returns>
        public Vector2 GetUVBilinear(float rx, float ry)
        {
            float l, t, r, b;

            if (rotation == Rotation.rotation0)
            {
                l = left;
                t = top;
                r = right;
                b = bottom;
            }
            else if (rotation == Rotation.rotation90)
            {
                l = bottom;
                t = left;
                r = top;
                b = right;
            }
            else if (rotation == Rotation.rotation180)
            {
                l = right;
                t = bottom;
                r = left;
                b = top;
            }
            else
            {
                l = top;
                t = right;
                r = bottom;
                b = left;
            }

            return new Vector2((r - l) * rx + l, (t - b) * ry + b);
        }

        /// <summary>
        /// Converts UV to Unity World Position on the side of the cube with the size (2, 2, 2)
        /// </summary>
        /// <param name="uv">UV</param>
        /// <param name="cubeSide">Side of the cube</param>
        /// <returns>Unity World Position</returns>
        public Vector3 GetWorldPosition(Vector2 uv, CubeSide cubeSide)
        {
            float l, t, r, b;

            if (rotation == Rotation.rotation0)
            {
                l = left;
                t = top;
                r = right;
                b = bottom;
            }
            else if (rotation == Rotation.rotation90)
            {
                l = bottom;
                t = left;
                r = top;
                b = right;
            }
            else if (rotation == Rotation.rotation180)
            {
                l = right;
                t = bottom;
                r = left;
                b = top;
            }
            else
            {
                l = top;
                t = right;
                r = bottom;
                b = left;
            }

            float x = (uv.x - l) / (r - l) * 2 - 1;
            float y = (uv.y - b) / (t - b) * 2 - 1;

            if (cubeSide == CubeSide.up) return new Vector3(x, 1, -y);
            if (cubeSide == CubeSide.front) return new Vector3(x, y, 1);
            if (cubeSide == CubeSide.left) return new Vector3(-1, y, x);
            if (cubeSide == CubeSide.back) return new Vector3(-x, y, -1);
            if (cubeSide == CubeSide.right) return new Vector3(1, y, -x);
            return new Vector3(x, -1, y);
        }

        /// <summary>
        /// Sets UV values in the array
        /// </summary>
        /// <param name="uv">Array of UV values</param>
        /// <param name="index">Start index</param>
        public void SetUVToArray(Vector2[] uv, int index)
        {
            if (rotation == Rotation.rotation0)
            {
                uv[index++] = new Vector2(left, top);
                uv[index++] = new Vector2(right, top);
                uv[index++] = new Vector2(right, bottom);
                uv[index] = new Vector2(left, bottom);
            }
            else if (rotation == Rotation.rotation90)
            {
                uv[index++] = new Vector2(left, bottom);
                uv[index++] = new Vector2(left, top);
                uv[index++] = new Vector2(right, top);
                uv[index] = new Vector2(right, bottom);
            }
            else if (rotation == Rotation.rotation180)
            {
                uv[index++] = new Vector2(right, bottom);
                uv[index++] = new Vector2(left, bottom);
                uv[index++] = new Vector2(left, top);
                uv[index] = new Vector2(right, top);
            }
            else if (rotation == Rotation.rotation270)
            {
                uv[index++] = new Vector2(right, top);
                uv[index++] = new Vector2(right, bottom);
                uv[index++] = new Vector2(left, bottom);
                uv[index] = new Vector2(left, top);
            }
        }
    }
}