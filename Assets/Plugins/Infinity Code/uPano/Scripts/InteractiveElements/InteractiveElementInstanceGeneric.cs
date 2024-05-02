/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// The base class of instances for interactive elements like HotSpot, Direction, etc.
    /// </summary>
    /// <typeparam name="T">Type of interactive element</typeparam>
    public abstract class InteractiveElementInstance<T>: InteractiveElementInstance
        where T: InteractiveElement
    {
        /// <summary>
        /// Reference to InteractiveElement
        /// </summary>
        public T element;

        public virtual InteractiveElementGlobalActions globalActions
        {
            get { return null; }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (element.pano.locked) return;

            OnPointerExit(eventData);

            if (!element.ignoreGlobalActions && globalActions != null) globalActions.OnClick.Invoke(element);
            element.OnClick.Invoke(element);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (element.pano.locked) return;
            if (!element.ignoreGlobalActions && globalActions != null) globalActions.OnPointerDown.Invoke(element);
            element.OnPointerDown.Invoke(element);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (element.pano.locked) return;
            if (!element.ignoreGlobalActions && globalActions != null) globalActions.OnPointerUp.Invoke(element);
            element.OnPointerUp.Invoke(element);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (element.pano.locked) return;
            if (!element.ignoreGlobalActions && globalActions != null) globalActions.OnPointerEnter.Invoke(element);
            element.OnPointerEnter.Invoke(element);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (element.pano.locked) return;
            if (!element.ignoreGlobalActions && globalActions != null) globalActions.OnPointerExit.Invoke(element);
            element.OnPointerExit.Invoke(element);
        }
    }
}