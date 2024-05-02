/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Component for synchronous change of fov when using multiple cameras
    /// </summary>
    [AddComponentMenu("uPano/Plugins/MultiCamera")]
    public class MultiCamera : Plugin
    {
        public enum ItemsType
        {
            manual,
            childsOfGameObject
        }

        public enum InitType
        {
            Start,
            LateUpdate
        }

        /// <summary>
        /// Array of cameras
        /// </summary>
        public Camera[] cameras;

        public ItemsType itemsType = ItemsType.manual;
        public InitType initType = InitType.Start;
        public bool excludeContainer = true;

        public GameObject camerasContainer;

        /// <summary>
        /// Whether to change the distance between the cameras?
        /// </summary>
        public bool updateDistance = false;

        /// <summary>
        /// Distance curve, where time is fov, value is distance
        /// </summary>
        public AnimationCurve distanceCurve = AnimationCurve.Linear(5, 0.05f, 60, 0.5f);

        private bool inited;

        private void InitCameras()
        {
            if (itemsType == ItemsType.childsOfGameObject)
            {
                cameras = camerasContainer.GetComponentsInChildren<Camera>();
                if (excludeContainer) cameras = cameras.Where(c => c.gameObject != camerasContainer).ToArray();
            }

            if (cameras != null)
            {
                foreach (Camera cam in cameras)
                {
                    if (cam == null) continue;
                    if (pano.addPhysicsRaycaster && cam.GetComponent<PhysicsRaycaster>() == null) cam.gameObject.AddComponent<PhysicsRaycaster>();
                }
            }

            inited = true;
        }

        private void LateUpdate()
        {
            if (!inited && initType == InitType.LateUpdate) InitCameras();
        }

        protected override void OnEnableLate()
        {
            base.OnEnableLate();

            if (_pano == null) return;

            _pano.OnFOVChanged += OnFovChanged;
            OnFovChanged(_pano.fov);
        }

        protected virtual void OnFovChanged(float value)
        {
            if (cameras == null) return;

            float distanceValue = distanceCurve.Evaluate(value);

            foreach (Camera cam in cameras)
            {
                if (cam == null) continue;

                cam.fieldOfView = value;
                UpdateDistance(cam, distanceValue);
            }
        }

        protected override void Start()
        {
            base.Start();

            if (initType == InitType.LateUpdate) InitCameras();

            OnFovChanged(_pano.fov);
        }

        protected void UpdateDistance(Camera cam, float distanceValue)
        {
            if (!updateDistance) return;

            cam.transform.localPosition = cam.transform.localPosition.normalized * distanceValue;
        }
    }
}