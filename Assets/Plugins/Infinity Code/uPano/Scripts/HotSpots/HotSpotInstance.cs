/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.HotSpots
{
    /// <summary>
    /// HotSpot instance to event handling
    /// </summary>
    [AddComponentMenu("")]
    public class HotSpotInstance : InteractiveElementInstance<HotSpot>
    {
        public override InteractiveElementGlobalActions globalActions
        {
            get { return element.manager.globalActions ?? HotSpotGlobalActions.instance; }
        }
    }
}