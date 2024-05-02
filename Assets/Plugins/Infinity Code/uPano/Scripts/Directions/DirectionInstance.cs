/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Directions
{
    /// <summary>
    /// Direction instance to event handling
    /// </summary>
    [AddComponentMenu("")]
    public class DirectionInstance : InteractiveElementInstance<Direction>
    {
        public override InteractiveElementGlobalActions globalActions
        {
            get { return element.manager.globalActions ?? DirectionGlobalActions.instance; }
        }
    }
}