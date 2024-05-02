/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Actions.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uPano.HotSpots
{
    /// <summary>
    /// Interactive GameObject, which is automatically positioned on the panorama with pan and tilt
    /// </summary>
    [Serializable]
    public class HotSpot: PrefabElement, IScalableElement
    {
        /// <summary>
        /// Tooltip when hovering over a hot spot
        /// </summary>
        public string tooltip;

        /// <summary>
        /// Enumeration of actions to show the tooltip
        /// </summary>
        public enum TooltipAction
        {
            UICanvas,
            textMesh,
            multiCamera
        }

        /// <summary>
        /// The action used to display the tooltip
        /// </summary>
        public TooltipAction tooltipAction;

        /// <summary>
        /// Tooltip prefab
        /// </summary>
        public GameObject tooltipPrefab;

        [SerializeField]
        private bool _lookToCenter = true;

        [SerializeField]
        private float _pan;

        [SerializeField]
        private float _tilt;

        [SerializeField]
        private Vector3 _scale = Vector3.one;

        [SerializeField]
        private Quaternion _rotation = Quaternion.identity;

        [SerializeField]
        private float _distanceMultiplier = 0.95f;

        private HotSpotInstance _instance;
        private HotSpotManager _manager;
        private PanoRenderer previewRenderer;

        /// <summary>
        /// The multiplier of distance from the center of the panorama to the world position of HotSpot.
        /// </summary>
        public float distanceMultiplier
        {
            get { return _distanceMultiplier; }
            set
            {
                _distanceMultiplier = value;
                UpdatePosition();
            }
        }

        public bool lookToCenter
        {
            get { return _lookToCenter; }
            set
            {
                _lookToCenter = value;
                UpdatePosition();
            }
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
        /// Tilt
        /// </summary>
        public float tilt
        {
            get { return _tilt; }
            set
            {
                _tilt = value;
                UpdatePosition();
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
                if (_instance == null) return;
                _instance.transform.localScale = _scale = value;
            }
        }

        /// <summary>
        /// Rotation
        /// </summary>
        public Quaternion rotation
        {
            get { return _rotation; }
            set
            {
                if (_instance == null) return;
                _rotation = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Prefab
        /// </summary>
        public override GameObject prefab
        {
            get
            {
                if (_prefab == null)
                {
                    if (manager != null) return manager.defaultPrefab;
                }
                return _prefab;
            }
            set
            {
                _prefab = value;
                if (_instance != null)
                {
                    Object.DestroyImmediate(_instance.gameObject);
                    _instance = null;
                }
                if (_manager != null && value != null) CreateInstance(_manager, _manager.container);
            }
        }

        public override string title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (_instance != null) _instance.name = value;
            }
        }

        public override bool visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                if (_instance != null) _instance.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// Reference to instance of HotSpot
        /// </summary>
        public HotSpotInstance instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Reference to HotSpot manager
        /// </summary>
        public HotSpotManager manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Gets the screen position of HotSpot
        /// </summary>
        public Vector3 screenPosition
        {
            get { return GetScreenPosition(); }
        }

        /// <summary>
        /// Gets the world position of HotSpot
        /// </summary>
        public Vector3 worldPosition
        {
            get { return manager.panoRenderer.GetWorldPosition(pan, tilt); }
        }

        internal HotSpot(float pan, float tilt, GameObject prefab, HotSpotManager manager)
        {
            _pan = pan;
            _tilt = tilt;
            _prefab = prefab;
            _manager = manager;
            _title = "HotSpot " + (manager.Count + 1);
        }

        internal void CreateInstance(HotSpotManager manager, Transform parent)
        {
            if (!Application.isPlaying) return;

            _manager = manager;
            pano = manager.pano;

            if (prefab == null) return;

            GameObject go = Object.Instantiate(prefab);
            go.name = string.IsNullOrEmpty(title) ? "HotSpot" : title;
            go.layer = manager.gameObject.layer;
            _instance = go.AddComponent<HotSpotInstance>();
            _instance.element = this;
            _instance.transform.parent = parent;
            _instance.transform.localScale = _scale;
            UpdatePosition();

            InitQuickActions(go);
        }

        internal void CreatePreview(PanoRenderer panoRenderer, Transform parent)
        {
            previewRenderer = panoRenderer;

            if (prefab == null) return;

            GameObject go = Object.Instantiate(prefab);
            go.name = string.IsNullOrEmpty(title) ? "HotSpot" : title;
            _instance = go.AddComponent<HotSpotInstance>();
            _instance.transform.parent = parent;
            _instance.transform.localScale = _scale;
            UpdatePreviewPosition();
        }

        /// <summary>
        /// Destroys current HotSpot
        /// </summary>
        public override void Destroy()
        {
            manager.Remove(this);
            _prefab = null;
            
            base.Destroy();
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
            tilt = this.tilt;
        }

        /// <summary>
        /// Gets the screen position of HotSpot
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <returns>Screen position of HotSpot</returns>
        public Vector2 GetScreenPosition(Camera camera = null)
        {
            return manager.panoRenderer.GetScreenPosition(pan, tilt, camera);
        }

        public override void InitQuickActions(GameObject target)
        {
            base.InitQuickActions(target);

            if (!string.IsNullOrEmpty(tooltip) && tooltipPrefab != null)
            {
                HotSpotTooltipAction action = null;
                GameObject container = target.transform.parent.gameObject;
                if (tooltipAction == TooltipAction.UICanvas) action = container.AddComponent<ShowTooltip>();
                else if (tooltipAction == TooltipAction.textMesh) action = container.AddComponent<ShowTextMeshTooltip>();
                else if (tooltipAction == TooltipAction.multiCamera) action = container.AddComponent<ShowTooltipMultiCamera>();

                if (action != null)
                {
                    action.text = tooltip;
                    action.tooltipPrefab = tooltipPrefab;

                    OnPointerEnter.AddListener(action.Show);
                    OnPointerExit.AddListener(action.Hide);
                }
            }
        }

        private void InstantiatePrefab(Transform parent)
        {
            GameObject currentPrefab = prefab != null ? prefab : manager.defaultPrefab;

            if (currentPrefab == null) return;

            GameObject go = Object.Instantiate(currentPrefab);
            go.name = string.IsNullOrEmpty(title) ? "HotSpot" : title;
            go.layer = manager.gameObject.layer;
            _instance = go.AddComponent<HotSpotInstance>();
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
            this.tilt = tilt;
        }

        /// <summary>
        /// Updates position of HotSpot
        /// </summary>
        public void UpdatePosition()
        {
            if (_instance == null) return;
            _instance.transform.position = (worldPosition - _manager.transform.position) * _distanceMultiplier + _manager.transform.position;
            UpdateRotation();
        }

        public void UpdatePreviewPosition()
        {
            if (_instance == null) return;
            _instance.transform.position = (previewRenderer.GetWorldPosition(pan, tilt) - previewRenderer.transform.position) * _distanceMultiplier + previewRenderer.transform.position;
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            if (_lookToCenter)
            {
                _instance.transform.LookAt(manager.transform.position + _instance.transform.localPosition * 2);
                _instance.transform.Rotate(rotation.eulerAngles);
            }
            else _instance.transform.localRotation = rotation;
        }
    }
}