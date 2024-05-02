/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Transitions
{
    public class DestroyUnusedPanoramaMesh: Transition
    {
        public override void Process()
        {
            Pano[] panoramas = Resources.FindObjectsOfTypeAll<Pano>();
            foreach (Pano pano in panoramas)
            {
                if (pano != Pano.lastActivePano) pano.Unload();
            }
            
            finished = true;
        }
    }
}