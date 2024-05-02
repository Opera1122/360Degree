/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Component for moving the panorama by UI joystick
    /// </summary>
    [WizardCreateMethod("CreateUIJoystick")]
    [AddComponentMenu("uPano/Controls/JoystickControl")]
    public class JoystickControl : UIControl
    {
        /// <summary>
        /// Reference to instance of the Pano
        /// </summary>
        public Pano panoInstance;

        /// <summary>
        /// Reference to the floating central image
        /// </summary>
        public Image center;

        /// <summary>
        /// Speed of change pan
        /// </summary>
        public float panSpeed = 100;

        /// <summary>
        /// Speed of change tilt
        /// </summary>
        public float tiltSpeed = 100;

        private bool isDrag;

        protected override void GetPanoInstance()
        {
            _pano = panoInstance;
        }

        /// <summary>
        /// The method that should be called when the central image is pressed
        /// </summary>
        public void OnCenterPress()
        {
            isDrag = true;
        }

        /// <summary>
        /// The method that should be called when the central image is released.
        /// </summary>
        public void OnCenterRelease()
        {
            isDrag = false;
            center.transform.localPosition = Vector3.zero;
        }

        private void Update()
        {
            if (pano == null || pano.locked) return;
            if (!isDrag) return;

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(center.canvas.transform as RectTransform, pano.GetInputPosition(), center.canvas.worldCamera, out pos);
            center.transform.position = center.canvas.transform.TransformPoint(pos);
            Vector3 localPosition = center.transform.localPosition;
            float magnitude = localPosition.magnitude;
            if (magnitude > 60) localPosition = center.transform.localPosition = localPosition.normalized * 60;

            pano.pan += localPosition.x / 60 * panSpeed * Time.deltaTime;
            pano.tilt += localPosition.y / 60 * tiltSpeed * Time.deltaTime;

            if (OnInput != null && localPosition.magnitude > 0) OnInput(this);
        }
    }
}