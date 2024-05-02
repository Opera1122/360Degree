/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Forwards input from Render Texture on Raw Image to panorama
    /// </summary>
    public class RawImageTouchForwarder : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        private static List<RawImageTouchForwarder> forwarders = new List<RawImageTouchForwarder>();
        private static RawImageTouchForwarder lastActiveForwarder;

        /// <summary>
        /// Raw Image where Render Texture is displayed
        /// </summary>
        public RawImage image;

        /// <summary>
        /// Reference to a panorama
        /// </summary>
        public Pano pano;

        /// <summary>
        /// Render Texture of a panorama
        /// </summary>
        public RenderTexture targetTexture;

#if !UNITY_EDITOR
        private static Vector2 pointerPos = Vector2.zero;
        private static Vector2[] touchPos = new Vector2[2];
#endif

        /// <summary>
        /// Gets the camera used to display the Render Texture
        /// </summary>
        public Camera worldCamera
        {
            get
            {
                if (image.canvas == null || image.canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
                return image.canvas.worldCamera;
            }
        }

        protected void OnDestroy()
        {
            forwarders.Remove(this);
            if (forwarders.Count == 0)
            {
                pano.OnGetInputPosition -= OnGetInputPosition;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
#if !UNITY_EDITOR
            if (InputManager.touchSupported && InputManager.touchCount > 1 && eventData.pointerId >= 0 && eventData.pointerId < 2)
            {
                touchPos[eventData.pointerId] = eventData.position;
                pointerPos = (touchPos[0] + touchPos[1]) / 2;
            }
            else
            {
                if (eventData.pointerId >= 0 && eventData.pointerId < 2) touchPos[eventData.pointerId] = eventData.position;
                pointerPos = eventData.position;
            }
#endif
        }

        private static Vector2 OnGetInputPosition()
        {
            Vector2 pos;
            for (int i = 0; i < forwarders.Count; i++)
            {
                var forwarder = forwarders[i];
#if UNITY_EDITOR
                if (forwarder.ProcessTouch(InputManager.mousePosition, out pos)) return pos;
#else
                if (InputManager.touchSupported) 
                {
                    if (forwarder.ProcessTouch(pointerPos, out pos)) return pos;
                }
                else 
                {
                    if (forwarder.ProcessTouch(InputManager.mousePosition, out pos)) return pos;
                }
#endif
            }

            return Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
#if !UNITY_EDITOR
            pointerPos = eventData.position;
            lastActiveForwarder = this;
#endif
        }

        /// <summary>
        /// Converts screen position from panorama space to forwarder space
        /// </summary>
        /// <param name="position">Screen position in panorama space</param>
        /// <returns>Screen position in forwarder space</returns>
        public Vector2 PanoToForwarderSpace(Vector2 position)
        {
            RectTransform t = image.rectTransform;
            Vector2 sizeDelta = t.rect.size;
            if ((int)sizeDelta.x == 0 || (int)sizeDelta.y == 0) return Vector2.zero;

            if (targetTexture == null)
            {
                position.x *= sizeDelta.x / Screen.width;
                position.y *= sizeDelta.y / Screen.height;
            }
            else
            {
                position.x *= sizeDelta.x / targetTexture.width;
                position.y *= sizeDelta.y / targetTexture.height;
            }

            position.x -= sizeDelta.x / 2;
            position.y -= sizeDelta.y / 2;

            Vector3 pos = (Vector3)position + image.transform.position;

            return RectTransformUtility.WorldToScreenPoint(worldCamera, pos);
        }

        private bool ProcessTouch(Vector2 inputTouch, out Vector2 localPosition, bool checkRect = true)
        {
            localPosition = Vector2.zero;

            RectTransform t = image.rectTransform;
            Vector2 sizeDelta = t.rect.size;
            if ((int)sizeDelta.x == 0 || (int)sizeDelta.y == 0) return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, inputTouch, worldCamera, out localPosition)) return false;
            if (checkRect && !t.rect.Contains(localPosition)) return false;

            localPosition += sizeDelta / 2.0f;

            if (targetTexture == null)
            {
                localPosition.x *= Screen.width / sizeDelta.x;
                localPosition.y *= Screen.height / sizeDelta.y;
            }
            else
            {
                localPosition.x *= targetTexture.width / sizeDelta.x;
                localPosition.y *= targetTexture.height / sizeDelta.y;
            }

            return true;
        }

        private void Start()
        {
            if (pano == null) pano = Pano.lastActivePano;

            pano.OnGetInputPosition += OnGetInputPosition;

            forwarders.Add(this);
        }
    }
}