/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Attributes;
using InfinityCode.uPano.Plugins;
using UnityEngine;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Component for moving the panorama by UI buttons
    /// </summary>
    [WizardCreateMethod("CreateUIButtons")]
    [AddComponentMenu("uPano/Controls/UIButtonsControl")]
    public class UIButtonsControl : UIControl
    {
        /// <summary>
        /// Reference to instance of the Pano
        /// </summary>
        public Pano panoInstance;

        /// <summary>
        /// Reference to auto rotate button
        /// </summary>
        public GameObject autoRotateButton;

        /// <summary>
        /// Speed of change pan
        /// </summary>
        public float panSpeed = 100;

        /// <summary>
        /// Speed of change tilt
        /// </summary>
        public float tiltSpeed = 100;

        /// <summary>
        /// Speed of change fov
        /// </summary>
        public float fovSpeed = 60;

        /// <summary>
        /// The width of the content with AutoRotate button
        /// </summary>
        public float widthWithAutoRotate = 220;

        /// <summary>
        /// The width of the content without AutoRotate button
        /// </summary>
        public float widthWithoutAutoRotate = 185;

        /// <summary>
        /// Adapt sensitivity based on FOV
        /// </summary>
        public bool adaptiveSensitivity = true;

        /// <summary>
        /// Sensitivity multiplier curve
        /// </summary>
        public AnimationCurve fovSensitivityMulCurve = AnimationCurve.Linear(5, 0.05f, 60, 1f);

        private bool isLeft;
        private bool isRight;
        private bool isUp;
        private bool isDown;
        private bool isZoomIn;
        private bool isZoomOut;
        private AutoRotate autoRotate;
        private bool restoreByTimer;

        protected override void GetPanoInstance()
        {
            _pano = panoInstance;
        }

        /// <summary>
        /// Gets adapted sensitivity
        /// </summary>
        /// <param name="value">Sensitivity</param>
        /// <returns>Adapted sensitivity if enabled, or input sensitivity</returns>
        protected float GetSensitivity(float value)
        {
            if (!adaptiveSensitivity) return value;
            return value * fovSensitivityMulCurve.Evaluate(pano.fov);
        }

        private void InitAutoRotate()
        {
            autoRotate = _pano.GetComponent<AutoRotate>();
            RectTransform rt = transform as RectTransform;
            if (autoRotate == null)
            {
                if (autoRotateButton != null) autoRotateButton.SetActive(false);
                rt.sizeDelta = new Vector2(widthWithoutAutoRotate, rt.sizeDelta.y);
            }
            else
            {
                if (autoRotateButton != null)
                {
                    autoRotateButton.SetActive(true);
                    rt.sizeDelta = new Vector2(widthWithAutoRotate, rt.sizeDelta.y);
                }
                restoreByTimer = autoRotate.restoreByTimer;
            }
        }

        /// <summary>
        /// The method that should be called when the auto rotate button is clicked
        /// </summary>
        public void OnAutoRotateClick()
        {
            if (autoRotate == null) return;

            autoRotate.paused = !autoRotate.paused;
            if (!autoRotate.paused) autoRotate.restoreByTimer = restoreByTimer;
        }

        /// <summary>
        /// The method that should be called when the down button is pressed
        /// </summary>
        public void OnDownPressed()
        {
            isDown = true;
        }

        /// <summary>
        /// The method that should be called when the down button is released
        /// </summary>
        public void OnDownReleased()
        {
            isDown = false;
        }

        /// <summary>
        /// The method that should be called when the left button is pressed
        /// </summary>
        public void OnLeftPressed()
        {
            isLeft = true;
        }

        /// <summary>
        /// The method that should be called when the left button is released
        /// </summary>
        public void OnLeftReleased()
        {
            isLeft = false;
        }

        /// <summary>
        /// The method that should be called when the right button is pressed
        /// </summary>
        public void OnRightPressed()
        {
            isRight = true;
        }

        /// <summary>
        /// The method that should be called when the right button is released
        /// </summary>
        public void OnRightReleased()
        {
            isRight = false;
        }

        /// <summary>
        /// The method that should be called when the up button is pressed
        /// </summary>
        public void OnUpPressed()
        {
            isUp = true;
        }

        /// <summary>
        /// The method that should be called when the up button is released
        /// </summary>
        public void OnUpReleased()
        {
            isUp = false;
        }

        protected override void OnPanoEnabled(Pano pano)
        {
            base.OnPanoEnabled(pano);

            InitAutoRotate();
        }

        /// <summary>
        /// The method that should be called when the zoom in button is pressed
        /// </summary>
        public void OnZoomInPressed()
        {
            isZoomIn = true;
        }

        /// <summary>
        /// The method that should be called when the zoom in button is released
        /// </summary>
        public void OnZoomInReleased()
        {
            isZoomIn = false;
        }

        /// <summary>
        /// The method that should be called when the zoom out button is pressed
        /// </summary>
        public void OnZoomOutPressed()
        {
            isZoomOut = true;
        }

        /// <summary>
        ///  The method that should be called when the zoom out button is released
        /// </summary>
        public void OnZoomOutReleased()
        {
            isZoomOut = false;
        }

        protected override void Start()
        {
            base.Start();

            InitAutoRotate();
        }

        private void Update()
        {
            if (pano == null || pano.locked) return;

            bool changed = false;
            if (isLeft)
            {
                _pano.pan -= GetSensitivity(panSpeed) * Time.deltaTime;
                changed = true;
            }

            if (isRight)
            {
                _pano.pan += GetSensitivity(panSpeed) * Time.deltaTime;
                changed = true;
            }

            if (isUp)
            {
                _pano.tilt += GetSensitivity(tiltSpeed) * Time.deltaTime;
                changed = true;
            }

            if (isDown)
            {
                _pano.tilt -= GetSensitivity(tiltSpeed) * Time.deltaTime;
                changed = true;
            }

            if (isZoomIn)
            {
                _pano.fov -= fovSpeed * Time.deltaTime;
                changed = true;
            }

            if (isZoomOut)
            {
                _pano.fov += fovSpeed * Time.deltaTime;
                changed = true;
            }

            if (changed && OnInput != null) OnInput(this);
        }
    }
}