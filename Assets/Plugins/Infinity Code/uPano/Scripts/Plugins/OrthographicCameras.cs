/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Component for synchronous change of fov when using orthographic cameras
    /// </summary>
    [AddComponentMenu("uPano/Plugins/OrthographicCameras")]
    public class OrthographicCameras : MultiCamera
    {
        /// <summary>
        /// Orthographic size curve, where time is fov, value is orthographic size
        /// </summary>
        public AnimationCurve orthographicSizeCurve = AnimationCurve.Linear(5, 0.5f, 60, 5);

        protected override void OnFovChanged(float value)
        {
            if (cameras == null) return;

            float distanceValue = distanceCurve.Evaluate(value);
            value = orthographicSizeCurve.Evaluate(value);

            foreach (Camera cam in cameras)
            {
                if (cam == null) continue;

                cam.orthographicSize = value;
                UpdateDistance(cam, distanceValue);
            }
        }
    }
}