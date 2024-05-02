/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// Range of float values
    /// </summary>
    [Serializable]
    public class FloatRange
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public float min;

        /// <summary>
        /// Maximum value
        /// </summary>
        public float max;

        /// <summary>
        /// Constructor
        /// </summary>
        public FloatRange()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Clamps a value between a minimum float and maximum float value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Value between a minimum float and maximum</returns>
        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Loops the value t, so that it is never larger than maximum and never smaller than minimum
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Value between a minimum float and maximum</returns>
        public float Repeat(float value)
        {
            return Mathf.Repeat(value - min, max - min) + min;
        }
    }
}