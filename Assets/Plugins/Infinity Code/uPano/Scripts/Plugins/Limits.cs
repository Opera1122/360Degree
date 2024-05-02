/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Attributes;
using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Component for limiting the values of pan, tilt and fov
    /// </summary>
    [Serializable]
    [WizardEnabled(true)]
    [AddComponentMenu("uPano/Plugins/Limits")]
    public class Limits : Plugin
    {
        [SerializeField]
        private FloatRange _fovLimits = new FloatRange(5, 60);

        [SerializeField]
        private FloatRange _panLimits = new FloatRange(0, 360);

        [SerializeField]
        private FloatRange _tiltLimits = new FloatRange(-90, 90);

        [SerializeField]
        private bool _limitFOV = true;

        [SerializeField]
        private bool _limitPan = false;

        [SerializeField]
        private bool _limitTilt = true;

        /// <summary>
        /// Gets and sets the limits for fov
        /// </summary>
        public FloatRange fovLimits
        {
            get
            {
                if (_fovLimits == null) _fovLimits = new FloatRange(5, 60);
                return _fovLimits;
            }
            set
            {
                if (_fovLimits == value) return;
                _fovLimits = value;
                _pano.VerifyFOV();
            }
        }

        /// <summary>
        /// Gets and sets the limits for pan
        /// </summary>
        public FloatRange panLimits
        {
            get
            {
                if (_panLimits == null) _panLimits = new FloatRange(0, 360);
                return _panLimits;
            }
            set
            {
                if (_panLimits == value) return;
                _panLimits = value;
                _pano.VerifyPan();
            }
        }

        /// <summary>
        /// Gets and sets the limits for tilt
        /// </summary>
        public FloatRange tiltLimits
        {
            get
            {
                if (_tiltLimits == null) _tiltLimits = new FloatRange(-90, 90);
                return _tiltLimits;
            }
            set
            {
                if (_tiltLimits == value) return;
                _tiltLimits = value;
                _pano.VerifyTilt();
            }
        }

        /// <summary>
        /// Gets and sets whether to use limits for fov
        /// </summary>
        public bool limitFOV
        {
            get { return _limitFOV; }
            set
            {
                if (_limitFOV == value) return;
                _limitFOV = value;
                if (value) _pano.VerifyFOV();
            }
        }

        /// <summary>
        /// Gets and sets whether to use limits for pan
        /// </summary>
        public bool limitPan
        {
            get { return _limitPan; }
            set
            {
                if (_limitPan == value) return;
                _limitPan = value;
                if (value) _pano.VerifyPan();
            }
        }

        /// <summary>
        /// Gets and sets whether to use limits for tilt
        /// </summary>
        public bool limitTilt
        {
            get { return _limitTilt; }
            set
            {
                if (_limitTilt == value) return;
                _limitTilt = value;
                if (value) _pano.VerifyTilt();
            }
        }

        protected override void OnEnableLate()
        {
            base.OnEnableLate();

            Pano.OnPanoEnabled -= OnPanoEnabled;
            Pano.OnPanoEnabled += OnPanoEnabled;

            SetPano(_pano);
        }

        private void OnPanoEnabled(Pano newPano)
        {
            if (pano == null || pano.gameObject == gameObject) return;

            SetPano(newPano);
        }

        private void SetPano(Pano newPano)
        {
            _pano = newPano;

            _pano.OnVerifyFOV -= VerifyFOV;
            _pano.OnVerifyPan -= VerifyPan;
            _pano.OnVerifyTilt -= VerifyTilt;

            _pano.OnVerifyFOV += VerifyFOV;
            _pano.OnVerifyPan += VerifyPan;
            _pano.OnVerifyTilt += VerifyTilt;
        }

        private float VerifyFOV(float value)
        {
            if (!_limitFOV) return value;
            return Mathf.Clamp(value, fovLimits.min, fovLimits.max);
        }

        private float VerifyPan(float value)
        {
            if (!_limitPan) return value;
            if (panLimits.max - panLimits.min == 360) return Mathf.Repeat(value, 360);
            return Mathf.Clamp(value, panLimits.min, panLimits.max);
        }

        private float VerifyTilt(float value)
        {
            if (!_limitTilt) return value;
            return Mathf.Clamp(value, tiltLimits.min, tiltLimits.max);
        }
    }
}