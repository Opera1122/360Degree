/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Directions
{
    /// <summary>
    /// Global direction actions
    /// </summary>
    [AddComponentMenu("uPano/Directions/Direction Global Actions")]
    public class DirectionGlobalActions : InteractiveElementGlobalActions
    {
        private static DirectionGlobalActions _instance;

        /// <summary>
        /// Reference to instance of global actions
        /// </summary>
        public static DirectionGlobalActions instance
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

            DirectionManager manager = GetComponent<DirectionManager>();
            if (manager != null)
            {
                manager.globalActions = this;
                hasOwner = true;
            }
            else _instance = this;
        }
    }
}