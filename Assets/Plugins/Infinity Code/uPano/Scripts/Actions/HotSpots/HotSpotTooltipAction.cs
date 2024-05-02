/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Base class for tooltip actions
    /// </summary>
    [Serializable]
    public abstract class HotSpotTooltipAction : HotSpotAction
    {
        /// <summary>
        /// Tooltip prefab. Must contain Text component
        /// </summary>
        public GameObject tooltipPrefab;

        /// <summary>
        /// The text that should be shown
        /// </summary>
        public string text;

        protected static HotSpot owner;
        protected static GameObject instance;

        /// <summary>
        /// Hides the tooltip for the current HotSpot
        /// </summary>
        /// <param name="element">HotSpot</param>
        public abstract void Hide(InteractiveElement element);

        /// <summary>
        /// Shows the tooltip for the current HotSpot
        /// </summary>
        /// <param name="element">HotSpot</param>
        public abstract void Show(InteractiveElement element);
    }
}