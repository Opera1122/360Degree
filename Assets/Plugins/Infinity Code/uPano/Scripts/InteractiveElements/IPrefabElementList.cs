/*           INFINITY CODE           */
/*     https://infinity-code.com     */

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// Interface of the list of prefab-based interactive elements
    /// </summary>
    public interface IPrefabElementList
    {
        /// <summary>
        /// Number of elements
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get item by index
        /// </summary>
        /// <param name="index">Item index</param>
        /// <returns>Interactive element</returns>
        PrefabElement GetItemAt(int index);
    }
}