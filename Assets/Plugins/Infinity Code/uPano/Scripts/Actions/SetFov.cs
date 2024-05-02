/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Sets fov of the panorama
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Set Fov")]
    public class SetFov : AnimatedAction<SetFov, float>
    {
        /// <summary>
        /// Target panorama. If null, will be used panorama of the element that causes the action
        /// </summary>
        public Pano pano;

        /// <summary>
        /// Target fov
        /// </summary>
        public float fov;

        private Pano _pano;
        private float initialFov;

        protected override void SetAnimatedValue(float f)
        {
            if (_pano == null) return;

            _pano.fov = Mathf.LerpAngle(initialFov, fov, f);
        }

        protected override void SetFixedValue()
        {
            if (_pano == null) return;

            _pano.fov = fov;
        }

        protected override void StoreInitialValue()
        {
            if (pano != null) _pano = pano;
            else _pano = element.pano;

            initialFov = _pano.fov;
        }
    }
}