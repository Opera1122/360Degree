/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Component for moving the panorama by keyboard
    /// </summary>
    [WizardEnabled(true)]
    [AddComponentMenu("uPano/Controls/KeyboardControl")]
    public class KeyboardControl : SensitivityControl
    {
        private EventSystem eventSystem;

        private void Update()
        {
            if (pano == null || pano.locked) return;
            if (exclusiveControl != null && exclusiveControl != this) return;

            if (eventSystem == null) eventSystem = CanvasUtils.GetEventSystem();
            if (eventSystem != null)
            {
                GameObject obj = eventSystem.currentSelectedGameObject;
                if (obj != null && (obj.GetComponent<Selectable>() != null || obj.GetComponentInParent<Selectable>() != null)) return;
            }

            float prevPan = _pano.pan;
            float prevTilt = _pano.tilt;
            float prevFov = _pano.fov;

            if (axes != Axes.Tilt) _pano.pan += InputManager.GetAxis("Horizontal") * GetSensitivity(sensitivityPan) * Time.deltaTime;
            if (axes != Axes.Pan) _pano.tilt += InputManager.GetAxis("Vertical") * GetSensitivity(sensitivityTilt) * Time.deltaTime;

            float zoomSpeed = 0;

            if (InputManager.GetKey(KeyCode.Plus) || InputManager.GetKey(KeyCode.KeypadPlus)) zoomSpeed = 1;
            else if (InputManager.GetKey(KeyCode.Minus) || InputManager.GetKey(KeyCode.KeypadMinus)) zoomSpeed = -1;
            _pano.fov -= zoomSpeed * sensitivityFov * Time.deltaTime;

            if (OnInput != null)
            {
                if (Math.Abs(_pano.pan - prevPan) > float.Epsilon || 
                    Math.Abs(_pano.tilt - prevTilt) > float.Epsilon || 
                    Math.Abs(_pano.fov - prevFov) > float.Epsilon)
                {
                    OnInput(this);
                }
            }
        }
    }
}