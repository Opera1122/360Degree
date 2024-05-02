/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets pan and tilt of the panorama
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Pan and Tilt")]
    public class SetPanTilt : AnimatedAction<SetPanTilt, float>
    {
        /// <summary>
        /// Target panorama. If null, will be used panorama of the element that causes the action
        /// </summary>
        public Pano pano;

        /// <summary>
        /// Target pan
        /// </summary>
        public float pan;

        /// <summary>
        /// Target tilt
        /// </summary>
        public float tilt;

        private float initialPan;
        private float initialTilt;

        protected override void SetAnimatedValue(float f)
        {
            if (pano == null) return;

            pano.pan = Mathf.LerpAngle(initialPan, pan, f);
            pano.tilt = Mathf.Lerp(initialTilt, tilt, f);
        }

        protected override void SetFixedValue()
        {
            if (pano == null) return;

            pano.pan = pan;
            pano.tilt = tilt;
        }

        protected override void StoreInitialValue()
        {
            if (pano == null) pano = element.pano;

            initialPan = pano.pan;
            initialTilt = pano.tilt;
        }
    }
}