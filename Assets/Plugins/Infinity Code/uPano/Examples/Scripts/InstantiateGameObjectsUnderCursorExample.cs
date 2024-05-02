/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Renderers.Base;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Examples
{
    /// <summary>
    /// Example of how to get pan and tilt under the cursor, convert it to Unity World Position, and position GameObjects
    /// </summary>
    [AddComponentMenu("uPano/Examples/InstantiateGameObjectsUnderCursorExample")]
    public class InstantiateGameObjectsUnderCursorExample : MonoBehaviour
    {
        /// <summary>
        /// Prefab
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// Text which will be display pan and tilt under the cursor
        /// </summary>
        public Text text;

        /// <summary>
        /// Reference to PanoRenderer
        /// </summary>
        private PanoRenderer panoRenderer;

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// </summary>
        private void Start()
        {
            // Store an instance of PanoRenderer
            panoRenderer = GetComponent<PanoRenderer>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled
        /// </summary>
        private void Update()
        {
            // Get pan and tilt under the cursor
            float pan, tilt;
            panoRenderer.GetPanTiltUnderCursor(out pan, out tilt);

            // Show pan and tilt on UI
            if (text != null) text.text = "Pan: " + pan + ", tilt: " + tilt;

            // If the left mouse button is pressed and left control is hold ...
            if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))
            {
                if (prefab != null)
                {
                    // ... instantiate prefab ...
                    GameObject go = Instantiate(prefab);

                    // ... convert pan and tilt to Unity World Position and set the position of GameObject
                    go.transform.position = panoRenderer.GetWorldPosition(pan, tilt);
                }
            }
        }
    }
}