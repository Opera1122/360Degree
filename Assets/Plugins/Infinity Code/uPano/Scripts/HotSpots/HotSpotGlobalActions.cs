/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.HotSpots
{
    /// <summary>
    /// Global hot spot actions
    /// </summary>
    [AddComponentMenu("uPano/Hot Spots/Hot Spot Global Actions")]
    public class HotSpotGlobalActions : InteractiveElementGlobalActions
    {
        private static HotSpotGlobalActions _instance;

        /// <summary>
        /// Reference to instance of global actions
        /// </summary>
        public static HotSpotGlobalActions instance
        {
            get { return _instance; }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!hasOwner) _instance = null;
        }

        protected override void OnEnableLate()
        {
            base.OnEnableLate();

            HotSpotManager manager = GetComponent<HotSpotManager>();
            if (manager != null)
            {
                manager.globalActions = this;
                hasOwner = true;
            }
            else _instance = this;
        }
    }
}