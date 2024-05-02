/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// Interface for interactive elements that can be scaled
    /// </summary>
    interface IScalableElement
    {
        /// <summary>
        /// Gets and sets the scale of interactive element
        /// </summary>
        Vector3 scale { get; set; }
    }
}
