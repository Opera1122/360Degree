/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// Base class for lists of prefab-based interactive elements
    /// </summary>
    /// <typeparam name="T">Type of interactive element</typeparam>
    public abstract class PrefabElementList<T> : InteractiveElementList<T>, IPrefabElementList
        where T : PrefabElement
    {
        [SerializeField]
        protected GameObject _defaultPrefab;

        /// <summary>
        /// Prefab by default, which will be used if the element's prefab is not specified
        /// </summary>
        public GameObject defaultPrefab
        {
            get
            {
                return _defaultPrefab != null ? _defaultPrefab : GlobalSettings.GetDefaultPrefab<T>();
            }
            set
            {
                _defaultPrefab = value;
                foreach (T item in items)
                {
                    if (item.prefab == null) item.Reinit();
                }
            }
        }

        /// <summary>
        /// Get item by index
        /// </summary>
        /// <param name="index">Item index</param>
        /// <returns>Interactive element</returns>
        public new PrefabElement GetItemAt(int index)
        {
            return items[index];
        }
    }
}