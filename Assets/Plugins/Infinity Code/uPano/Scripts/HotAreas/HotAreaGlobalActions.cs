/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.HotAreas
{
    /// <summary>
    /// Global Hot area actions
    /// </summary>
    [AddComponentMenu("uPano/Hot Areas/Direction Global Actions")]
    public class HotAreaGlobalActions : InteractiveElementGlobalActions
    {
        private static HotAreaGlobalActions _instance;

        /// <summary>
        /// Reference to instance of global actions
        /// </summary>
        public static HotAreaGlobalActions instance
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

            HotAreaManager manager = GetComponent<HotAreaManager>();
            if (manager != null)
            {
                manager.globalActions = this;
                hasOwner = true;
            }
            else _instance = this;
        }
    }
}