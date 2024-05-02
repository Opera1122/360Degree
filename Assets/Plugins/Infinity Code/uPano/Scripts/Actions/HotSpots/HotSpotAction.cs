/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Base class for HotSpot actions
    /// </summary>
    [Serializable]
    public abstract class HotSpotAction: InteractiveElementAction
    {
        /// <summary>
        /// This method must be called for the action happened
        /// </summary>
        /// <param name="hotSpot">HotSpot which called the action</param>
        public abstract void Invoke(HotSpot hotSpot);

        public override void Invoke(InteractiveElement element)
        {
            Invoke(element as HotSpot);
        }
    }
}
