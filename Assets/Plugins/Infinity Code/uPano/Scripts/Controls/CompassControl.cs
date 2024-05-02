/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Component for moving the panorama by a device compass
    /// </summary>
    [Serializable]
    [AddComponentMenu("uPano/Controls/CompassControl")]
    public class CompassControl : PanoControl
    {
        /// <summary>
        /// The action that is called when the compass value is changed
        /// </summary>
        public Action<float> OnChanged;

        private Compass compass;
        private double timestamp;
        private float lastHeading;

        protected override void Start()
        {
            base.Start();

            InputManager.location.Start();

            compass = InputManager.compass;
            compass.enabled = true;
            timestamp = compass.timestamp;
            lastHeading = compass.trueHeading;
        }

        private void Update()
        {
            if (pano.locked) return;

            if (Math.Abs(compass.timestamp - timestamp) > double.Epsilon)
            {
                timestamp = compass.timestamp;

                if (Math.Abs(lastHeading - compass.trueHeading) > float.Epsilon)
                {
                    lastHeading = compass.trueHeading;
                    if (OnChanged != null) OnChanged(lastHeading);
                }
            }

            if (timestamp > 0 && Math.Abs(_pano.pan - lastHeading) > float.Epsilon)
            {
                float prevPan = _pano.pan;
                _pano.pan = Mathf.LerpAngle(_pano.pan, lastHeading, Time.deltaTime * 2);
                if (OnInput != null && Math.Abs(prevPan - _pano.pan) > float.Epsilon) OnInput(this);
            }
        }
    }
}
