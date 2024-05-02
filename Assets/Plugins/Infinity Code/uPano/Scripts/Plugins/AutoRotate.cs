/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Controls;
using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Component for automatic rotation of the panorama
    /// </summary>
    [Serializable]
    [AddComponentMenu("uPano/Plugins/AutoRotate")]
    public class AutoRotate : Plugin
    {
        /// <summary>
        /// Stop the rotation when the user interacts with the panorama
        /// </summary>
        public bool stopOnInput = true;

        /// <summary>
        /// Restore rotation after inactivity
        /// </summary>
        public bool restoreByTimer = true;

        /// <summary>
        /// Delay before starting rotation
        /// </summary>
        public float startDelay = 3;

        /// <summary>
        /// Number of seconds of inactivity, after which the rotation will be restored
        /// </summary>
        public float restoreAfter = 10;

        /// <summary>
        /// Acceleration curve of rotation
        /// </summary>
        public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 3, 1);

        /// <summary>
        /// Speed of change pan
        /// </summary>
        public float panSpeed = 9f;

        /// <summary>
        /// Speed of change tilt
        /// </summary>
        public float tiltSpeed = 0.2f;

        private bool _paused;
        private float pauseTimer;
        private float startTime;

        /// <summary>
        /// Gets and sets the pause rotation
        /// </summary>
        public bool paused
        {
            get { return _paused; }
            set
            {
                _paused = value;
                restoreByTimer = false;
            }
        }

        private void OnControlStarted(PanoControl control)
        {
            if (pano.gameObject == gameObject)
            {
                if (control.pano != pano) return;
            }

            control.OnInput -= OnControlInput;
            control.OnInput += OnControlInput;

            if (startDelay > 0)    
            {
                pauseTimer = startDelay;
                _paused = true;
            }
            else startTime = Time.realtimeSinceStartup;
        }

        private void OnControlInput(PanoControl control)
        {
            if (stopOnInput)
            {
                _paused = true;
                pauseTimer = restoreAfter;
            }
        }

        protected override void OnEnableLate()
        {
            Pano.OnPanoEnabled -= OnPanoEnabled;
            Pano.OnPanoEnabled += OnPanoEnabled;

            PanoControl.OnControlStarted -= OnControlStarted;
            PanoControl.OnControlStarted += OnControlStarted;

            PanoControl[] controls = GetComponentsInParent<PanoControl>();
            foreach (PanoControl control in controls)
            {
                control.OnInput -= OnControlInput;
                control.OnInput += OnControlInput;
            }
        }

        private void OnPanoEnabled(Pano newPano)
        {
            if (pano != null && pano.gameObject != gameObject) _pano = newPano;
        }

        private void Update()
        {
            if (pano.locked) return;

            if (_paused)
            {
                if (restoreByTimer)
                {
                    pauseTimer -= Time.deltaTime;
                    if (pauseTimer <= 0)
                    {
                        _paused = false;
                        startTime = Time.realtimeSinceStartup;
                    }
                }
                if (_paused) return;
            }

            float f = accelerationCurve.Evaluate(Time.realtimeSinceStartup - startTime);

            _pano.tilt = Mathf.Lerp(_pano.tilt, 0,  tiltSpeed * f * Time.deltaTime);
            _pano.pan = _pano.pan + panSpeed * f * Time.deltaTime;
        }
    }
}