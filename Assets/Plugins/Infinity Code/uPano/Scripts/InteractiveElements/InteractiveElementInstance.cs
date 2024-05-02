/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// The base class of instances for interactive elements like HotSpot, Direction, etc.
    /// </summary>
    public abstract class InteractiveElementInstance : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public abstract void OnPointerClick(PointerEventData eventData);
        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
    }
}