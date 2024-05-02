/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// Base class for prefab-based interactive elements
    /// </summary>
    public abstract class PrefabElement : InteractiveElement
    {
        [SerializeField]
        protected GameObject _prefab;

        /// <summary>
        /// Get / set a prefab
        /// </summary>
        public abstract GameObject prefab { get; set; }
    }
}