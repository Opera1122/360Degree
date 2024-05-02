/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Examples
{
    /// <summary>
    /// Example of how to position UI elements
    /// </summary>
    [AddComponentMenu("uPano/Examples/PositioningUIElementExample")]
    public class PositioningUIElementExample : MonoBehaviour
    {
        /// <summary>
        /// UI tooltip GameObject
        /// </summary>
        public GameObject tooltipGameObject;

        /// <summary>
        /// RectTransform of the tooltip background
        /// </summary>
        public RectTransform tooltipBackground;

        /// <summary>
        /// The text where the tooltip will be shown
        /// </summary>
        public Text textfield;

        /// <summary>
        /// HotSpot prefab
        /// </summary>
        public GameObject hotSpotPrefab;

        /// <summary>
        /// Reference to Pano
        /// </summary>
        private Pano pano;

        /// <summary>
        /// HotSpot for which the tooltip is currently displayed
        /// </summary>
        private HotSpot activeHotSpot;

        /// <summary>
        /// Camera used in Canvas
        /// </summary>
        private Camera worldCamera
        {
            get
            {
                if (textfield.canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
                return textfield.canvas.worldCamera;
            }
        }

        /// <summary>
        /// This method is called when pointer enter to HotSpot
        /// </summary>
        /// <param name="hotSpot">HotSpot which invoked method</param>
        private void OnPointerEnter(InteractiveElement hotSpot)
        {
            // Store active hot spot
            activeHotSpot = hotSpot as HotSpot;

            // Enable tooltip GameObject
            tooltipGameObject.SetActive(true);

            // Get tooltip string from hot spot
            string tooltip = hotSpot["tooltip"] as string;

            // Set tooltip text
            textfield.text = tooltip;

            // Calculate tooltip width
            int width = CanvasUtils.CalculateWidthOfMessage(tooltip, textfield.font, textfield.fontSize) + 20;

            // Set width of textfield and background
            Vector2 sizeDelta = new Vector2(width, tooltipBackground.sizeDelta.y);
            tooltipBackground.sizeDelta = textfield.rectTransform.sizeDelta = sizeDelta;

            // Update tooltip position
            UpdateTooltipPosition(0);
        }

        /// <summary>
        /// This method is called when pointer exit from HotSpot
        /// </summary>
        /// <param name="hotSpot">HotSpot which invoked method</param>
        private void OnPointerExit(InteractiveElement hotSpot)
        {
            // Remove reference
            activeHotSpot = null;

            // Hide tooltip
            tooltipGameObject.SetActive(false);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// </summary>
        private void Start()
        {
            // Initial hide the tooltip
            tooltipGameObject.SetActive(false);

            // Get Pano instance
            pano = GetComponent<Pano>();

            // Subscribe to panorama actions
            pano.OnFOVChanged += UpdateTooltipPosition;
            pano.OnPanChanged += UpdateTooltipPosition;
            pano.OnTiltChanged += UpdateTooltipPosition;

            // Get hot spot manager
            HotSpotManager hsManager = GetComponent<HotSpotManager>();

            // Create hot spot 1
            HotSpot hotSpot = hsManager.Create(0, 0, hotSpotPrefab);
            
            // Subscribe to hot spot actions
            hotSpot.OnPointerEnter.AddListener(OnPointerEnter);
            hotSpot.OnPointerExit.AddListener(OnPointerExit);

            // Store tooltip string in hot spot
            // You can store any data in the hot spot
            hotSpot["tooltip"] = "HotSpot 1";

            // Create hot spot 2
            hotSpot = hsManager.Create(10, 10, hotSpotPrefab);
            hotSpot.OnPointerEnter.AddListener(OnPointerEnter);
            hotSpot.OnPointerExit.AddListener(OnPointerExit);
            hotSpot["tooltip"] = "HotSpot 2";
        }

        /// <summary>
        /// Updates hot spot position
        /// </summary>
        /// <param name="value">If this method caused a panorama event, a new value is passed</param>
        private void UpdateTooltipPosition(float value)
        {
            // If there is no active hot spot, return
            if (activeHotSpot == null) return;

            // Convert the screen position of HotSpot to the local position of tooltip
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipGameObject.transform.parent as RectTransform, activeHotSpot.screenPosition, worldCamera, out point);

            // Move the tooltip a little bit up
            point.y += 50;

            // Set the tooltip local position
            tooltipGameObject.transform.localPosition = point;
        }
    }
}