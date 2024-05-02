/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Activates / Deactivates the GameObject
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set GameObject Active")]
    public class SetGameObjectActive : TransitionAction
    {
        /// <summary>
        /// Array of items to be activated or deactivated
        /// </summary>
        public Item[] items;

        protected override void InvokeAction(InteractiveElement element)
        {
            if (items == null) return;

            foreach (Item item in items)
            {
                if (item.target != null)
                {
                    item.target.SetActive(item.value);
                }
            }
        }

        /// <summary>
        /// Item to be activated or deactivated and state
        /// </summary>
        [Serializable]
        public class Item
        {
            /// <summary>
            /// GameObject to be activated or deactivated 
            /// </summary>
            public GameObject target;

            /// <summary>
            /// State. True - activate, false - deactivate
            /// </summary>
            public bool value;
        }
    }
}