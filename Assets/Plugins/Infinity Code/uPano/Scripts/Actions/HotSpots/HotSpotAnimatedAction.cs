/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.HotSpots;

namespace InfinityCode.uPano.Actions.HotSpots
{
    /// <summary>
    /// Base class for animated HotSpot actions
    /// </summary>
    /// <typeparam name="T">Type of HotSpotAnimatedAction</typeparam>
    /// <typeparam name="U">Type of animated value</typeparam>
    public abstract class HotSpotAnimatedAction<T, U> : AnimatedAction<T, U>
        where T : HotSpotAnimatedAction<T, U>
    {
        protected HotSpot hotSpot
        {
            get { return element as HotSpot; }
        }
    }
}