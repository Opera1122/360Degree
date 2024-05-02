/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Base class for Interactive Element actions
    /// </summary>
    [Serializable]
    public abstract class InteractiveElementAction : MonoBehaviour
    {
        public static Action<InteractiveElementAction> OnActionStarted;

        /// <summary>
        /// This method must be called for the action happened without interactive element
        /// </summary>
        public void Invoke()
        {
            Invoke(null);
        }

        /// <summary>
        /// This method must be called for the action happened
        /// </summary>
        /// <param name="element">Interactive Element which called the action</param>
        public abstract void Invoke(InteractiveElement element);

        protected virtual void Start()
        {
            if (OnActionStarted != null) OnActionStarted(this);
        }
    }
}