/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uPano.Directions
{
    /// <summary>
    /// Arrow pointing direction to the next panorama
    /// </summary>
    [Serializable]
    public class Direction: PrefabElement, IScalableElement
    {
        [SerializeField]
        private float _pan;

        [SerializeField]
        private Vector3 _scale = Vector3.one;

        private DirectionManager _manager;
        private DirectionInstance _instance;

        /// <summary>
        /// Instance of the direction
        /// </summary>
        public DirectionInstance instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Reference to the Direction manager
        /// </summary>
        public DirectionManager manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Pan
        /// </summary>
        public float pan
        {
            get { return _pan; }
            set
            {
                _pan = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Prefab
        /// </summary>
        public override GameObject prefab
        {
            get { return _prefab; }
            set
            {
                _prefab = value; 
                Reinit();
            }
        }

        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                if (_instance != null) _instance.transform.localScale = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="prefab">Prefab</param>
        /// <param name="manager">Reference to the manager</param>
        public Direction(float pan, GameObject prefab, DirectionManager manager)
        {
            _pan = pan;
            _prefab = prefab;
            _manager = manager;
            _title = "Direction " + (manager.Count + 1);
        }

        internal void CreateInstance(DirectionManager manager, Transform parent)
        {
            if (!Application.isPlaying) return;

            _manager = manager;
            pano = manager.pano;
            InstantiatePrefab(parent);
            if (_instance != null) InitQuickActions(_instance.gameObject);
        }

        internal void DestroyInstance()
        {
            if (_instance != null)
            {
                Object.DestroyImmediate(_instance.gameObject);
                _instance = null;
            }
        }

        public override void GetPanTilt(out float pan, out float tilt)
        {
            pan = this.pan;
            tilt = 0;
        }

        private void InstantiatePrefab(Transform parent)
        {
            GameObject currentPrefab = prefab != null? prefab : manager.defaultPrefab;

            if (currentPrefab == null) return;

            GameObject go = Object.Instantiate(currentPrefab);
            go.name = string.IsNullOrEmpty(title) ? "Direction" : title;
            go.layer = manager.gameObject.layer;
            _instance = go.AddComponent<DirectionInstance>();
            _instance.element = this;
            _instance.transform.SetParent(parent, false);
            _instance.transform.localScale = _scale;
        }

        public override void Reinit()
        {
            if (_instance == null) return;

            Transform parent = _instance.transform.parent;

            Object.Destroy(_instance.gameObject);
            InstantiatePrefab(parent);
            UpdatePosition();
            if (_instance != null) InitQuickActions(_instance.gameObject);
        }

        public override void SetPanTilt(float pan, float tilt)
        {
            this.pan = pan;
        }

        public void UpdatePosition()
        {
            if (_instance == null) return;
            Vector3 worldPosition = pano.panoRenderer.GetWorldPosition(pan, 0);
            _instance.transform.localPosition = (worldPosition - _instance.transform.parent.position).normalized * manager.internalRadius;
            float angle = 90 - MathHelper.Angle2D(_instance.transform.position, worldPosition);
            _instance.transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }
}