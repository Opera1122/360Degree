/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Plugins;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Shows and hides tooltips on canvas for multiple cameras
    /// </summary>
    [AddComponentMenu("uPano/Actions/For HotSpots/Show Tooltip Multi Camera")]
    public class ShowTooltipMultiCamera : HotSpotTooltipAction
    {
        /// <summary>
        /// Reference to MultiCamera script
        /// </summary>
        public MultiCamera multiCamera;

        /// <summary>
        /// Vertical offset relative to HotSpot center
        /// </summary>
        public float yOffset = 50;

        /// <summary>
        /// Adjusts the size of the textfield and background to the size of the text
        /// </summary>
        [Tooltip("Adjusts the size of the textfield and background to the size of the text")]
        public bool adjustSize = true;

        private static bool hasInstances = false;
        private static GameObject[] instances;

        private Canvas canvas;

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
        /// <param name="hotSpot">HotSpot</param>
        public override void Hide(InteractiveElement hotSpot)
        {
            if (owner != hotSpot || !hasInstances) return;

            for (int i = 0; i < instances.Length; i++)
            {
                Destroy(instances[i]);
                instances[i] = null;
            }

            hasInstances = false;
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

            if (hasInstances)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    Destroy(instances[i]);
                    instances[i] = null;
                }

                hasInstances = false;
            }

            if (multiCamera == null || multiCamera.cameras == null) return;
            if (instances == null || instances.Length != multiCamera.cameras.Length) instances = new GameObject[multiCamera.cameras.Length];

            for (int i = 0; i < multiCamera.cameras.Length; i++)
            {
                GameObject instance = Instantiate(tooltipPrefab);
                instance.transform.SetParent(canvas.transform);
                instances[i] = instance;
                hasInstances = true;

                Vector2 point;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, hotSpot.GetScreenPosition(multiCamera.cameras[i]), worldCamera, out point);
                point.y += yOffset;
                instance.transform.localPosition = point;

                Text textfield = instance.GetComponentInChildren<Text>();
                if (textfield != null)
                {
                    textfield.text = text;

                    if (adjustSize)
                    {
                        int width = CanvasUtils.CalculateWidthOfMessage(text, textfield.font, textfield.fontSize) + 20;
                        textfield.rectTransform.sizeDelta = new Vector2(width, textfield.rectTransform.sizeDelta.y);

                        Transform background = instance.transform.Find("background");
                        if (background != null) (background as RectTransform).sizeDelta = textfield.rectTransform.sizeDelta;
                    }
                }

            }
            

            owner = hotSpot;
        }

        private void Update()
        {
            if (instances == null || owner == null) return;

            for (int i = 0; i < instances.Length; i++)
            {
                GameObject instance = instances[i];
                if (instance == null) continue;

                Vector2 point;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, owner.GetScreenPosition(multiCamera.cameras[i]), worldCamera, out point);
                point.y += yOffset;
                instance.transform.localPosition = point;
            }
        }
    }
}