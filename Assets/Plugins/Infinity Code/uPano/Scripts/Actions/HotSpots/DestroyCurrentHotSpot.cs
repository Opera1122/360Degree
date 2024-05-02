/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.HotSpots;
using UnityEngine;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Destroys the current HotSpot
    /// </summary>
    [AddComponentMenu("uPano/Actions/For HotSpots/Destroy Current Hot Spot")]
    public class DestroyCurrentHotSpot : HotSpotAction
    {
        public override void Invoke(HotSpot hotSpot)
        {
            hotSpot.Destroy();
        }
    }
}