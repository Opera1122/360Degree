/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Shows and hides tooltips on canvas
    /// </summary>
    [AddComponentMenu("uPano/Actions/For HotSpots/Show Tooltip")]
    public class ShowTooltip : HotSpotTooltipAction
    {
        /// <summary>
        /// Vertical offset relative to HotSpot center
        /// </summary>
        public float yOffset = 50;

        /// <summary>
        /// Adjusts the size of the textfield and background to the size of the text
        /// </summary>
        [Tooltip("Adjusts the size of the textfield and background to the size of the text")]
        public bool adjustSize = true;

        public bool getTextFromElement = false;

        private static Canvas canvas;

        private Camera worldCamera
        {
            get
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
                return canvas.worldCamera;
            }
        }

        /// <summary>
        /// Hides the tooltip for the current HotSpot
        /// </summary>
        /// <param name="element">HotSpot</param>
        public override void Hide(InteractiveElement element)
        {
            if (owner != element || instance == null) return;

            Destroy(instance);
            instance = null;
            owner = null;
        }

        public override void Invoke(HotSpot hotSpot)
        {
            Show(hotSpot);
        }

        /// <summary>
        /// Shows the tooltip for the current HotSpot
        /// </summary>
        /// <param name="element">HotSpot</param>
        public override void Show(InteractiveElement element)
        {
            Show(element as HotSpot);
        }

        /// <summary>
        /// Shows the tooltip for the current HotSpot
        /// </summary>
        /// <param name="hotSpot">HotSpot</param>
        public void Show(HotSpot hotSpot)
        {
            if (tooltipPrefab == null) throw new Exception("Tooltip prefab can not be null");

            if (canvas == null) canvas = CanvasUtils.GetCanvas();

            if (instance != null) Destroy(instance);
            string currentText = getTextFromElement ? hotSpot.tooltip : text;

            if (string.IsNullOrEmpty(currentText)) return;

            instance = Instantiate(tooltipPrefab);
            instance.transform.SetParent(canvas.transform);

            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, hotSpot.screenPosition, worldCamera, out point);
            point.y += yOffset;
            instance.transform.localPosition = point;

            Text textfield = instance.GetComponentInChildren<Text>();
            if (textfield != null)
            {
                textfield.text = currentText;

                if (adjustSize)
                {
                    int width = CanvasUtils.CalculateWidthOfMessage(text, textfield.font, textfield.fontSize) + 20;
                    textfield.rectTransform.sizeDelta = new Vector2(width, textfield.rectTransform.sizeDelta.y);

                    Transform background = instance.transform.Find("background");
                    if (background != null) (background as RectTransform).sizeDelta = textfield.rectTransform.sizeDelta;
                }
            }

            owner = hotSpot;
        }

        private void Update()
        {
            if (instance == null) return;
            if (owner == null || canvas == null)
            {
                Destroy(instance);
                owner = null;
                instance = null;
                return;
            }

            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, owner.screenPosition, worldCamera, out point);
            point.y += yOffset;
            instance.transform.localPosition = point;
        }
    }
}