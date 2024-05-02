/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Sets the color of the first material of the current HotSpot
    /// </summary>
    [AddComponentMenu("uPano/Actions/For HotSpots/Set Hot Spot Color")]
    public class SetHotSpotColor : HotSpotAnimatedAction<SetHotSpotColor, Color>
    {
        /// <summary>
        /// The color that must be set
        /// </summary>
        public Color color = Color.white;

        private Renderer _renderer;

        protected override void SetAnimatedValue(float f)
        {
            if (_renderer == null) return;
            _renderer.materials[0].color = Color.Lerp(initialValue, color, f);
        }

        protected override void SetFixedValue()
        {
            if (_renderer == null) return;
            _renderer.materials[0].color = color;
        }

        protected override void StoreInitialValue()
        {
            _renderer = hotSpot.instance.GetComponent<Renderer>();
            if (_renderer == null) return;
            initialValue = _renderer.materials[0].color;
        }
    }
}