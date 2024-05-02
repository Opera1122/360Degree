/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Sets the rotation of the current HotSpot
    /// </summary>
    [AddComponentMenu("uPano/Actions/For HotSpots/Set Hot Spot Rotation")]
    public class SetHotSpotRotation : HotSpotAnimatedAction<SetHotSpotRotation, Quaternion>
    {
        /// <summary>
        /// The rotation that must be set
        /// </summary>
        public Vector3 rotation = Vector3.one;

        protected override void SetAnimatedValue(float f)
        {
            hotSpot.rotation = Quaternion.Lerp(initialValue, Quaternion.Euler(rotation), f);
        }

        protected override void SetFixedValue()
        {
            hotSpot.rotation = Quaternion.Euler(rotation);
        }

        protected override void StoreInitialValue()
        {
            initialValue = hotSpot.rotation;
        }
    }
}