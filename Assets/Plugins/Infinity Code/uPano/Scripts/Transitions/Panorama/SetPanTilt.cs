/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Transitions
{
    /// <summary>
    /// Sets pan and tilt of the panorama
    /// </summary>
    [AddComponentMenu("uPano/Transitions/Panorama/Set Pan and Tilt")]
    public class SetPanTilt : TimeBasedTransition
    {
        /// <summary>
        /// Initial pan
        /// </summary>
        public float fromPan = 0;

        /// <summary>
        /// The initial pan is the pan of the panorama?
        /// </summary>
        public bool fromPanIsOriginal = false;

        /// <summary>
        /// Target pan
        /// </summary>
        public float toPan = 0;

        /// <summary>
        /// The target pan is the pan of the panorama?
        /// </summary>
        public bool toPanIsOriginal = false;

        /// <summary>
        /// Initial tilt
        /// </summary>
        public float fromTilt = 0;

        /// <summary>
        /// The initial tilt is the tilt of the panorama?
        /// </summary>
        public bool fromTiltIsOriginal = false;

        /// <summary>
        /// Target tilt
        /// </summary>
        public float toTilt = 0;

        /// <summary>
        /// The target tilt is the tilt of the panorama?
        /// </summary>
        public bool toTiltIsOriginal = false;

        private Pano pano;

        public override void Init()
        {
            base.Init();

            pano = FindObjectOfType<Pano>();

            if (pano != null)
            {
                if (fromPanIsOriginal) fromPan = pano.pan;
                if (toPanIsOriginal) toPan = pano.pan;
                if (fromTiltIsOriginal) fromTilt = pano.tilt;
                if (toTiltIsOriginal) toTilt = pano.tilt;
            }
            else finished = true;
        }

        public override void Process()
        {
            pano.pan = Mathf.LerpAngle(fromPan, toPan, curvedProgress);
            pano.tilt = Mathf.Lerp(fromTilt, toTilt, curvedProgress);
        }
    }
}