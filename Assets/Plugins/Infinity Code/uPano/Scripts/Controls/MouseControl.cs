/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Attributes;
using InfinityCode.uPano.HotAreas;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// Component for moving the panorama by mouse and touch
    /// </summary>
    [WizardEnabled(true)]
    [AddComponentMenu("uPano/Controls/MouseControl")]
    public class MouseControl : SensitivityControl
    {
        /// <summary>
        /// Enum of modes of moving the panorama
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Panorama will constantly move towards the cursor
            /// </summary>
            Free,

            /// <summary>
            /// Panorama will move toward the cursor, when you hold down the left mouse button
            /// </summary>
            LeftMouseButtonDown,

            /// <summary>
            /// Panorama will be moved by dragging. The sensitivity of pan and tilt will be ignored
            /// </summary>
            Drag
        }

        /// <summary>
        /// Mode of moving the panorama
        /// </summary>
        public Mode mode = Mode.Drag;

        /// <summary>
        /// The panorama will ignore the pressing on the UI element
        /// </summary>
        public bool notInteractUnderUI = true;

        /// <summary>
        /// Change fov using mouse wheel
        /// </summary>
        public bool wheelZoom = true;

        /// <summary>
        /// Change fov using pinch to zoom
        /// </summary>
        public bool pinchToZoom = true;

        /// <summary>
        /// Inertia pan and tilt when dragging a panorama
        /// </summary>
        public bool inertia = false;

        /// <summary>
        /// Speed of inertia slowdown
        /// </summary>
        public float inertiaLerpSpeed = 1f;

        private float lastPan, lastTilt;
        private bool isPressed;
        private bool pinchToZoomStarted;
        private float lastTouchDistance;
        private int lastTouchCount;
        private Vector2 inertiaSpeed;
        private Vector2 avgInertiaSpeed;
        private Vector2 lastInputPosition;
        private Vector2 inputPosition;

        private void ApplyInertia(ref float pan, ref float tilt)
        {
            float i = Time.deltaTime * inertiaLerpSpeed;
            inertiaSpeed = Vector3.Lerp(inertiaSpeed, Vector3.zero, i);

            if (inertiaSpeed.magnitude > 2)
            {
                Vector2 p = lastInputPosition + inertiaSpeed;
                float iPan, iTilt;
                if (_panoRenderer.GetPanTiltByScreenPosition(p, out iPan, out iTilt))
                {
                    _pano.tilt += lastTilt - iTilt;
                    _pano.pan += lastPan - iPan;

                    _panoRenderer.GetPanTiltUnderCursor(out pan, out tilt);
                }
            }
        }

        private void CalculateInertiaSpeed()
        {
            Vector2 delta = inputPosition - lastInputPosition;
            avgInertiaSpeed = Vector2.Lerp(avgInertiaSpeed, delta, Time.deltaTime * 5);
        }

        private void Drag(ref bool changed)
        {
            float pan, tilt;
            bool hit = _panoRenderer.GetPanTiltUnderCursor(out pan, out tilt);

            if (InputManager.GetMouseButtonDown(0))
            {
                if (!TryStartDrag(pan, tilt))
                {
                    lastPan = pan;
                    lastTilt = tilt;
                    return;
                }
            }
            else if (InputManager.GetMouseButtonUp(0))
            {
                if (exclusiveControl == this) exclusiveControl = null;
                isPressed = false;
                inertiaSpeed = avgInertiaSpeed;
            }

            if (InputManager.touchSupported && lastTouchCount != InputManager.touchCount)
            {
                lastPan = pan;
                lastTilt = tilt;
                lastTouchCount = InputManager.touchCount;
            }

            if (isPressed)
            {
                if (hit)
                {
                    _pano.pan += lastPan - pan;
                    _pano.tilt += lastTilt - tilt;

                    if (Math.Abs(_pano.pan - lastPan) > float.Epsilon || Math.Abs(_pano.tilt - lastTilt) > float.Epsilon) changed = true;

                    _panoRenderer.GetPanTiltUnderCursor(out pan, out tilt);

                    if (inertia) CalculateInertiaSpeed();
                }
            }
            else
            {
                if (inertia) ApplyInertia(ref pan, ref tilt);
            }

            lastPan = pan;
            lastTilt = tilt;
        }

        private bool IsCursorOnUI()
        {
            if (EventSystem.current == null) return false;

            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = inputPosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0 && hits[0].gameObject != pano.panoRenderer.meshGameObject;
        }

        private void Move(ref bool changed)
        {
            if (!isPressed)
            {
                if (InputManager.GetMouseButtonDown(0))
                {
                    if (notInteractUnderUI && IsCursorOnUI()) return;
                    isPressed = true;
                }
                else return;
            }
            else if (InputManager.GetMouseButtonUp(0))
            {
                isPressed = false;
                return;
            }

            ProcessMove(ref changed);
        }

        private void PinchToZoom()
        {
            if (!InputManager.touchSupported) return;

            if (InputManager.touchCount != 2)
            {
                pinchToZoomStarted = false;
                return;
            }

            Vector2 touch1 = InputManager.GetTouch(0).position;
            Vector2 touch2 = InputManager.GetTouch(1).position;
            float touchDistance = (touch1 - touch2).magnitude;

            if (!pinchToZoomStarted)
            {
                lastTouchDistance = touchDistance;
                pinchToZoomStarted = true;
                return;
            }

            if (Mathf.Abs(touchDistance - lastTouchDistance) > 2)
            {
                _pano.fov /= touchDistance / lastTouchDistance;
                lastTouchDistance = touchDistance;
            }
        }

        private void ProcessMove(ref bool changed)
        {
            if (axes != Axes.Tilt)
            {
                float lastPan = _pano.pan;
                _pano.pan -= GetSensitivity(sensitivityPan) * InputManager.GetAxis("Mouse X") / 20;
                if (Math.Abs(_pano.pan - lastPan) > float.Epsilon) changed = true;
            }

            if (axes != Axes.Pan)
            {
                float prevTilt = _pano.tilt;
                _pano.tilt -= GetSensitivity(sensitivityTilt) * InputManager.GetAxis("Mouse Y") / 20;
                if (Math.Abs(_pano.tilt - prevTilt) > float.Epsilon) changed = true;
            }
        }

        private bool TryStartDrag(float pan, float tilt)
        {
            if (exclusiveControl != null && exclusiveControl != this) return false;

            lastPan = pan;
            lastTilt = tilt;
            inertiaSpeed = avgInertiaSpeed = Vector2.zero;

            if (notInteractUnderUI && IsCursorOnUI()) return false;

            if (HotAreaManager.GetActiveArea() != null) return false;

            exclusiveControl = this;
            isPressed = true;
            return true;
        }

        private void Update()
        {
            if (pano == null || pano.locked) return;

            bool changed = false;

            inputPosition = pano.GetInputPosition();

            UpdateFov(ref changed);

            if (mode == Mode.Drag)
            {
                Drag(ref changed);
            }
            else if (mode == Mode.Free)
            {
                ProcessMove(ref changed);
            }
            else if (mode == Mode.LeftMouseButtonDown)
            {
                Move(ref changed);
            }

            if (changed && OnInput != null) OnInput(this);

            lastInputPosition = inputPosition;
        }

        private void UpdateFov(ref bool changed)
        {
            float prevFov = _pano.fov;

            if (wheelZoom) WheelZoom();
            if (pinchToZoom) PinchToZoom();

            if (Math.Abs(prevFov - _pano.fov) > float.Epsilon) changed = true;
        }

        private void WheelZoom()
        {
            _pano.fov -= InputManager.GetAxis("Mouse ScrollWheel") * sensitivityFov;
        }
    }
}
