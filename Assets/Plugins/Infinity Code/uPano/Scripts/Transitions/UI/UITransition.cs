/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Transitions.UI
{
    /// <summary>
    /// Base class for transitions using UI Canvas
    /// </summary>
    public abstract class UITransition : TimeBasedTransition
    {
        /// <summary>
        /// The transition must be under other UI elements?
        /// </summary>
        public bool underUI = true;

        /// <summary>
        /// UI element size multiplier
        /// </summary>
        public float sizeMultiplier = 1;

        /// <summary>
        /// Reference to GameObject where the transition is displayed
        /// </summary>
        protected GameObject displayGameObject;

        private Canvas canvas;
        private Graphic graphic;

        /// <summary>
        /// The name of the GameObject that will display the transition
        /// </summary>
        protected abstract string containerName { get; }

        protected override void Dispose()
        {
            base.Dispose();

            graphic = null;
            canvas = null;
            Destroy(displayGameObject);
        }

        /// <summary>
        /// Fits UI element to screen
        /// </summary>
        /// <param name="g">UI element</param>
        protected void FitToScreen(Graphic g)
        {
            canvas = g.canvas;
            graphic = g;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                RectTransform rt = g.rectTransform;

                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.localPosition = Vector3.zero;
                rt.sizeDelta = Vector2.zero;
                rt.localScale = new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier);
            }
            else if (canvas.renderMode == RenderMode.WorldSpace)
            {
                FitWorldSpace();
            }
        }

        private void FitWorldSpace()
        {
            Transform t = graphic.transform;
            float camHeight;
            Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            float distance = 1;
            if (cam.orthographic) camHeight = cam.orthographicSize * 2;
            else camHeight = 2 * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float m_ScaleFactor = 10;
            float scale = camHeight / Screen.height * m_ScaleFactor * sizeMultiplier;

            t.position = cam.transform.position + cam.transform.forward * distance;
            t.LookAt(cam.transform);
            t.localScale = new Vector3(scale / Screen.height * Screen.width, scale, scale);
        }

        public override void Init()
        {
            base.Init();

            Canvas canvas = CanvasUtils.GetCanvas();
            displayGameObject = new GameObject(containerName);
            displayGameObject.transform.SetParent(canvas.transform, false);
            if (underUI) displayGameObject.transform.SetAsFirstSibling();
            displayGameObject.AddComponent<CanvasRenderer>();
        }

        public override void Process()
        {
            base.Process();

            if (canvas.renderMode == RenderMode.WorldSpace) FitWorldSpace();
        }
    }
}