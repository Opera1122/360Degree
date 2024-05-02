/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// Interface of the list of interactive elements
    /// </summary>
    public interface IInteractiveElementList
    {
        /// <summary>
        /// Number of elements
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get items
        /// </summary>
        /// <returns>List of items</returns>
        IList GetItems();

        /// <summary>
        /// Get item by index
        /// </summary>
        /// <param name="index">Item index</param>
        /// <returns>Interactive element</returns>
        InteractiveElement GetItemAt(int index);
    }
}