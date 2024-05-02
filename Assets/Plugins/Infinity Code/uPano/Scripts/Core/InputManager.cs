/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// Wrapper for fields and methods of Input used by uPano. Can be used to intercept input and adapt an asset to a new input system.
    /// </summary>
    public static class InputManager
    {
        /// <summary>
        /// Intercept getting compass
        /// </summary>
        public static Func<Compass> OnCompass;

        /// <summary>
        /// Intercept getting axis
        /// </summary>
        public static Func<string, float> OnGetAxis;

        /// <summary>
        /// Intercept getting key
        /// </summary>
        public static Func<KeyCode, bool> OnGetKey;

        /// <summary>
        /// Intercept getting mouse button
        /// </summary>
        public static Func<int, bool> OnGetMouseButton;

        /// <summary>
        /// Intercept getting mouse button down
        /// </summary>
        public static Func<int, bool> OnGetMouseButtonDown;

        /// <summary>
        /// Intercept getting mouse button up
        /// </summary>
        public static Func<int, bool> OnGetMouseButtonUp;

        /// <summary>
        /// Intercept getting touche
        /// </summary>
        public static Func<int, Touch> OnGetTouch;

        /// <summary>
        /// Intercept getting gyro
        /// </summary>
        public static Func<Gyroscope> OnGyro;

        /// <summary>
        /// Intercept getting location
        /// </summary>
        public static Func<LocationService> OnLocation;

        /// <summary>
        /// Intercept getting mouse position
        /// </summary>
        public static Func<Vector2> OnMousePosition;

        /// <summary>
        /// Intercept getting touch count
        /// </summary>
        public static Func<int> OnTouchCount;

        /// <summary>
        /// Intercept getting touch supported
        /// </summary>
        public static Func<bool> OnTouchSupported;

        /// <summary>
        /// Intercept getting touches
        /// </summary>
        public static Func<Touch[]> OnTouches;

        /// <summary>
        /// Property for accessing compass (handheld devices only). 
        /// </summary>
        public static Compass compass
        {
            get
            {
                if (OnCompass != null) return OnCompass();
                return Input.compass;
            }
        }

        /// <summary>
        /// Returns default gyroscope.
        /// </summary>
        public static Gyroscope gyro
        {
            get
            {
                if (OnGyro != null) return OnGyro();
                return Input.gyro;
            }
        }

        /// <summary>
        /// Property for accessing device location (handheld devices only).
        /// </summary>
        public static LocationService location
        {
            get
            {
                if (OnLocation != null) return OnLocation();
                return Input.location;
            }
        }

        /// <summary>
        /// The current mouse position in pixel coordinates.
        /// </summary>
        public static Vector2 mousePosition
        {
            get
            {
                if (OnMousePosition != null) return OnMousePosition();
                return Input.mousePosition;
            }
        }

        /// <summary>
        /// Number of touches. Guaranteed not to change throughout the frame. 
        /// </summary>
        public static int touchCount
        {
            get
            {
                if (OnTouchCount != null) return OnTouchCount();
                return Input.touchCount;
            }
        }

        /// <summary>
        /// Returns whether the device on which application is currently running supports touch input.
        /// </summary>
        public static bool touchSupported
        {
            get
            {
                if (OnTouchSupported != null) return OnTouchSupported();
                return Input.touchSupported;
            }
        }

        /// <summary>
        /// Returns list of objects representing status of all touches during last frame.
        /// </summary>
        public static Touch[] touches
        {
            get
            {
                if (OnTouches != null) return OnTouches();
                return Input.touches;
            }
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName.
        /// </summary>
        /// <param name="axisName">Axis name</param>
        /// <returns>Value</returns>
        public static float GetAxis(string axisName)
        {
            if (OnGetAxis != null) return OnGetAxis(axisName);
            return Input.GetAxis(axisName);
        }

        /// <summary>
        /// Returns true while the user holds down the key identified by the key KeyCode enum parameter.
        /// </summary>
        /// <param name="key">KeyCode</param>
        /// <returns>Button state</returns>
        public static bool GetKey(KeyCode key)
        {
            if (OnGetKey != null) return OnGetKey(key);
            return Input.GetKey(key);
        }

        /// <summary>
        /// Returns whether the given mouse button is held down.
        /// </summary>
        /// <param name="button">Mouse button</param>
        /// <returns>Mouse button state</returns>
        public static bool GetMouseButton(int button)
        {
            if (OnGetMouseButton != null) return OnGetMouseButton(button);
            return Input.GetMouseButton(button);
        }

        /// <summary>
        /// Returns true during the frame the user pressed the given mouse button.
        /// </summary>
        /// <param name="button">Mouse button</param>
        /// <returns>Mouse button state</returns>
        public static bool GetMouseButtonDown(int button)
        {
            if (OnGetMouseButtonDown != null) return OnGetMouseButtonDown(button);
            return Input.GetMouseButtonDown(button);
        }

        /// <summary>
        /// Returns true during the frame the user releases the given mouse button.
        /// </summary>
        /// <param name="button">Mouse button</param>
        /// <returns>Mouse button state</returns>
        public static bool GetMouseButtonUp(int button)
        {
            if (OnGetMouseButtonUp != null) return OnGetMouseButtonUp(button);
            return Input.GetMouseButtonUp(button);
        }

        /// <summary>
        /// Get Touch struct by index.
        /// </summary>
        /// <param name="index">Touch index</param>
        /// <returns>Touch struct</returns>
        public static Touch GetTouch(int index)
        {
            if (OnGetTouch != null) return OnGetTouch(index);
            return Input.GetTouch(index);
        }
    }
}