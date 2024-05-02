/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Instantiates prefab
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Instantiate Prefab")]
    public class InstantiatePrefab : InteractiveElementAction
    {
        /// <summary>
        /// Prefab which must be instantiated
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// Parent that will be assigned to the new object
        /// </summary>
        public Transform parent;

        public override void Invoke(InteractiveElement element)
        {
            if (prefab != null) Instantiate(prefab, parent);
        }
    }
}