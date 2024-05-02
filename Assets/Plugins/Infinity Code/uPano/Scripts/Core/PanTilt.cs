/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// The point on the panorama set by pan and tilt.
    /// </summary>
    [Serializable]
    public struct PanTilt
    {
        /// <summary>
        /// Pan
        /// </summary>
        public float pan;

        /// <summary>
        /// Tilt
        /// </summary>
        public float tilt;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        public PanTilt(float pan, float tilt)
        {
            this.pan = pan;
            this.tilt = tilt;
        }

        /// <summary>
        /// Sets new pan and tilt
        /// </summary>
        /// <param name="pan">New pan</param>
        /// <param name="tilt">New tilt</param>
        public void Set(float pan, float tilt)
        {
            this.pan = pan;
            this.tilt = tilt;
        }

        public static implicit operator Vector2(PanTilt pt)
        {
            return new Vector2(pt.pan, pt.tilt);
        }

        public static implicit operator PanTilt(Vector2 v)
        {
            return new PanTilt(v.x, v.y);
        }
    }
}