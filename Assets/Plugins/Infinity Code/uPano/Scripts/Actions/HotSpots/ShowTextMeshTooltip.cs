/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Shows and hides tooltips on canvas
    /// </summary>
    [AddComponentMenu("uPano/Actions/For HotSpots/Show TextMesh Tooltip")]
    public class ShowTextMeshTooltip : HotSpotTooltipAction
    {
        /// <summary>
        /// Tilt offset relative to HotSpot center
        /// </summary>
        public float tiltOffset = 5;

        public bool getTextFromElement = false;

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

            if (instance != null) Destroy(instance);

            string currentText = getTextFromElement ? hotSpot.tooltip : text;

            if (string.IsNullOrEmpty(currentText)) return;

            instance = Instantiate(tooltipPrefab);
            instance.transform.parent = transform;

            owner = hotSpot;

            Update();

            TextMesh textMesh = instance.GetComponentInChildren<TextMesh>();
            if (textMesh != null) textMesh.text = currentText;
        }

        private void Update()
        {
            if (instance == null || owner == null) return;

            instance.transform.position = owner.manager.panoRenderer.GetWorldPosition(owner.pan, owner.tilt + tiltOffset);
            instance.transform.localPosition *= 0.95f;
            instance.transform.LookAt(instance.transform.localPosition * 10);
        }
    }
}