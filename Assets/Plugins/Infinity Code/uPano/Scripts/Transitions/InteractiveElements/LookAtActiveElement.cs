/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano.Transitions.InteractiveElements
{
    /// <summary>
    /// Sets the pan and tilt of the panorama as the pan and tilt of the element that caused the transition. Note: This can not be used in After Transitions
    /// </summary>
    [AddComponentMenu("uPano/Transitions/Interactive Elements/Look At Active Element")]
    public class LookAtActiveElement : TimeBasedTransition
    {
        /// <summary>
        /// Use Fov?
        /// </summary>
        public bool useFov;

        /// <summary>
        /// Fov of the panorama that should be set
        /// </summary>
        public float targetFov = 60;

        private float targetPan, targetTilt;
        private float originalPan, originalTilt, originalFov;

        public override void Init()
        {
            if (element == null) throw new Exception("Element cannot be null.");

            base.Init();

            element.GetPanTilt(out targetPan, out targetTilt);
            if (!useFov) targetFov = element.pano.fov;
            originalPan = element.pano.pan;
            originalTilt = element.pano.tilt;
            originalFov = element.pano.fov;
        }

        public override void Process()
        {
            element.pano.pan = Mathf.LerpAngle(originalPan, targetPan, curvedProgress);
            element.pano.tilt = Mathf.LerpAngle(originalTilt, targetTilt, curvedProgress);
            element.pano.fov = Mathf.LerpAngle(originalFov, targetFov, curvedProgress);
        }
    }
}