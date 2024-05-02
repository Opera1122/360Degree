/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Component for moving the panorama by a device gyroscope
    /// </summary>
    [AddComponentMenu("uPano/Controls/GyroControl")]
    public class GyroControl : PanoControl
    {
        private Gyroscope gyro;
        private Vector3 rotationRate;
        private Vector2 rotationDelta = Vector2.zero;

        protected override void Start()
        {
            base.Start();

            if (SystemInfo.supportsGyroscope)
            {
                gyro = InputManager.gyro;
                gyro.enabled = true;
            }
        }

        private void Update()
        {
            if (pano.locked) return;
            if (gyro == null || !gyro.enabled) return;

            Quaternion q = gyro.attitude;
            q *= Quaternion.Euler(0, 0, 180);
            q *= Quaternion.Inverse(q) * Quaternion.Euler(270, 180, 180) * q;

            float prevPan = _pano.pan;
            float prevTilt = _pano.tilt;

            rotationRate = q.eulerAngles;

            float pan = 360 - rotationRate.y + rotationDelta.y;
            float tilt = rotationRate.x + rotationDelta.x;
            if (tilt > 180) tilt -= 360;
            else if (tilt < -180) tilt += 360;

            if (exclusiveControl != null && exclusiveControl != this)
            {
                rotationDelta.y -= pan - prevPan;
                rotationDelta.x -= tilt - prevTilt;
                return;
            }

            _pano.pan = pan;
            _pano.tilt = tilt;

            if (OnInput != null)
            {
                if (Math.Abs(_pano.pan - prevPan) > float.Epsilon || Math.Abs(_pano.tilt - prevTilt) > float.Epsilon)
                {
                    OnInput(this);
                }
            }
        }
    }
}