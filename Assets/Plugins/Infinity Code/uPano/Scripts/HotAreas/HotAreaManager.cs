/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinityCode.uPano.HotAreas
{
    /// <summary>
    /// List of HotAreas
    /// </summary>
    [Serializable]
    public class HotAreaManager : InteractiveElementList<HotArea>, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
#if UNITY_2021_1_OR_NEWER
        , IPointerMoveHandler
#endif
    {
        internal HotAreaGlobalActions globalActions;

        private static HotArea lastArea;

#if !UNITY_2021_1_OR_NEWER
        private Vector2 lastPosition;
#endif

        /// <summary>
        /// Creates a new HotArea and adds it to the list
        /// </summary>
        /// <returns>HotArea</returns>
        public HotArea Create()
        {
            HotArea area = new HotArea();
            items.Add(area);
            return area;
        }

        /// <summary>
        /// Get the last area where the cursor was
        /// </summary>
        /// <returns>Last area where the cursor was</returns>
        public static HotArea GetActiveArea()
        {
            return lastArea;
        }

        /// <summary>
        /// Returns the area at the screen position
        /// </summary>
        /// <param name="position">Screen position</param>
        /// <returns>Hot Area</returns>
        public HotArea GetArea(Vector2 position)
        {
            float pan, tilt;
            panoRenderer.GetPanTiltByScreenPosition(position, out pan, out tilt);

            for (int i = 0; i < items.Count; i++)
            {
                HotArea area = items[i];
                if (area.Contain(pan, tilt))
                {
                    return area;
                }
            }

            return null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (pano.locked) return;

            HotArea area = GetArea(eventData.position);
            if (area == null) return;

            if (!area.ignoreGlobalActions && globalActions != null) globalActions.OnClick.Invoke(area);
            area.OnClick.Invoke(area);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pano.locked) return;

            HotArea area = GetArea(eventData.position);
            if (area == null) return;

            if (!area.ignoreGlobalActions && globalActions != null) globalActions.OnPointerDown.Invoke(area);
            area.OnPointerDown.Invoke(area);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pano.locked) return;

            HotArea area = GetArea(eventData.position);
            if (area == null) return;

            if (!area.ignoreGlobalActions && globalActions != null) globalActions.OnPointerUp.Invoke(area);
            area.OnPointerUp.Invoke(area);
        }

#if UNITY_2021_1_OR_NEWER
        public void OnPointerMove(PointerEventData eventData)
        {
            if (pano.locked) return;

            HotArea area = GetArea(eventData.position);
            
            if (lastArea == area) return;

            if (lastArea != null)
            {
                if (!lastArea.ignoreGlobalActions && globalActions != null) globalActions.OnPointerExit.Invoke(lastArea);
                lastArea.OnPointerEnter.Invoke(lastArea);
            }

            if (area != null)
            {
                if (!area.ignoreGlobalActions && globalActions != null) globalActions.OnPointerExit.Invoke(area);
                area.OnPointerEnter.Invoke(area);
            }

            lastArea = area;
        }
#endif

        public override int RemoveAll(Predicate<HotArea> match)
        {
            return items.RemoveAll(match);
        }

        protected override void Start()
        {
            base.Start();

            InitContainer("HotAreas");

            foreach (HotArea item in items)
            {
                item.manager = this;
                item.pano = pano;
                item.Reinit();
                item.InitQuickActions(_container.gameObject);
            }
        }

#if !UNITY_2021_1_OR_NEWER
        private void Update()
        {
            if (pano.locked) return;

            Vector2 inputPosition = pano.GetInputPosition();
            if (inputPosition == lastPosition) return;
            lastPosition = inputPosition;

            HotArea area = GetArea(inputPosition);

            if (lastArea == area) return;

            if (lastArea != null)
            {
                if (!lastArea.ignoreGlobalActions && globalActions != null) globalActions.OnPointerExit.Invoke(lastArea);
                lastArea.OnPointerExit.Invoke(lastArea);
            }

            if (area != null)
            {
                if (!area.ignoreGlobalActions && globalActions != null) globalActions.OnPointerEnter.Invoke(area);
                area.OnPointerEnter.Invoke(area);
            }

            lastArea = area;
        }
#endif
    }
}