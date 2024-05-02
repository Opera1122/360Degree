/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Destroys the current panorama
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Destroy Current Panorama")]
    public class DestroyCurrentPanorama: TransitionAction
    {
        protected override void InvokeAction(InteractiveElement element)
        {
            Destroy(element.pano.gameObject);
        }
    }
}