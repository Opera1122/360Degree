/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;

namespace InfinityCode.uPano.Attributes
{
    /// <summary>
    /// An attribute that specifies that a specific PanoRenderer is required for the class
    /// </summary>
    public class RequirePanoRendererAttribute : Attribute
    {
        /// <summary>
        /// Type of PanoRenderer.
        /// </summary>
        public Type type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of PanoRenderer</param>
        public RequirePanoRendererAttribute(Type type)
        {
            this.type = type;
        }
    }
}