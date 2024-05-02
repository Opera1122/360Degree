/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Plugins;
using UnityEngine;

namespace InfinityCode.uPano.InteractiveElements
{
    public abstract class InteractiveElementGlobalActions : Plugin
    {
        /// <summary>
        /// Event that occurs when click Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnClick = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when press Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerDown = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when release Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerUp = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when cursor enter on Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerEnter = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when cursor exit from Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerExit = new InteractiveElementEvent();

        protected bool hasOwner = false;
    }
}