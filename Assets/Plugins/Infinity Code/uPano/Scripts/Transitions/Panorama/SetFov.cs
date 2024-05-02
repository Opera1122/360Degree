/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Transitions
{
    /// <summary>
    /// Sets fov of the panorama
    /// </summary>
    [AddComponentMenu("uPano/Transitions/Panorama/Set Fov")]
    public class SetFov : TimeBasedTransition
    {
        /// <summary>
        /// Initial fov
        /// </summary>
        public float fromFov = 60;

        /// <summary>
        /// The initial fov is the fov of the panorama?
        /// </summary>
        public bool fromIsOriginal = false;

        /// <summary>
        /// Target fov
        /// </summary>
        public float toFov = 60;

        /// <summary>
        /// The target fov is the fov of the panorama?
        /// </summary>
        public bool toIsOriginal = false;

        private Pano pano;

        public override void Init()
        {
            base.Init();

            pano = FindObjectOfType<Pano>();

            if (pano != null)
            {
                if (fromIsOriginal) fromFov = pano.fov;
                if (toIsOriginal) toFov = pano.fov;
            }
            else finished = true;
        }

        public override void Process()
        {
            pano.fov = Mathf.Lerp(fromFov, toFov, curvedProgress);
        }
    }
}