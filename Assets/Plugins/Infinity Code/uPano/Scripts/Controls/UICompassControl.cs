/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Compass showing the current orientation of the panorama.
    /// </summary>
    [WizardCreateMethod("CreateUICompass")]
    [AddComponentMenu("uPano/Controls/UICompassControl")]
    public class UICompassControl : UIControl
    {
        /// <summary>
        /// Reference to instance of the Pano
        /// </summary>
        public Pano panoInstance;

        /// <summary>
        /// Reference to central arrow button
        /// </summary>
        public Image arrow;

        /// <summary>
        /// Changing of the pan should be animated?
        /// </summary>
        public bool animated = true;

        /// <summary>
        /// Animation duration
        /// </summary>
        public float duration = 0.3f;

        private float startPan;
        private float targetPan;
        private bool isAnim;
        private float progress;

        protected override void GetPanoInstance()
        {
            _pano = panoInstance;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _pano.OnPanChanged -= OnPanChanged;
        }

        private void OnPanChanged(float f)
        {
            arrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 360 - f);
        }

        protected override void OnPanoDestroy(Pano pano)
        {
            if (pano == _pano)
            {
                _pano.OnPanChanged -= OnPanChanged;
            }

            base.OnPanoDestroy(pano);
        }

        protected override void OnPanoEnabled(Pano pano)
        {
            if (_pano != null) _pano.OnPanChanged -= OnPanChanged;

            base.OnPanoEnabled(pano);

            pano.OnPanChanged += OnPanChanged;
            OnPanChanged(pano.pan);
        }

        /// <summary>
        /// This method should be called to rotate the panorama 90 degrees counterclockwise
        /// </summary>
        public void RotateLeft()
        {
            SetPan((Mathf.CeilToInt(pano.pan / 90) - 1) * 90);
        }

        /// <summary>
        /// This method should be called to rotate the panorama 90 degrees clockwise
        /// </summary>
        public void RotateRight()
        {
            SetPan(((int)(pano.pan / 90) + 1) * 90);
        }

        /// <summary>
        /// This method should be called to rotate the panorama to the north (pan-0)
        /// </summary>
        public void SetNorth()
        {
            SetPan(pano.pan > 180? 360: 0);
        }

        private void SetPan(float pan)
        {
            if (pano.locked) return;

            if (!animated)
            {
                pano.pan = pan;
                return;
            }

            targetPan = pan;
            startPan = pano.pan;
            progress = 0;
            isAnim = true;

            if (OnInput != null) OnInput(this);
        }

        protected override void Start()
        {
            base.Start();

            pano.OnPanChanged += OnPanChanged;
            OnPanChanged(pano.pan);
        }

        private void Update()
        {
            if (pano.locked) return;
            if (!isAnim) return;

            progress += Time.deltaTime / duration;
            if (progress >= 1)
            {
                progress = 1;
                isAnim = false;
            }

            pano.pan = Mathf.Lerp(startPan, targetPan, progress);
        }
    }
}