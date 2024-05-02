/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Linq;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinityCode.uPano
{
    /// <summary>
    /// The core of the panorama
    /// </summary>
    [DisallowMultipleComponent, Serializable]
    [AddComponentMenu("uPano/Pano", 100)]
    public class Pano : MonoBehaviour
    {
        /// <summary>
        /// uPano version
        /// </summary>
        public const string version = "3.1.0.1";

        #region Enums

        /// <summary>
        /// Types of camera for panorama
        /// </summary>
        public enum CameraType
        {
            /// <summary>
            /// Create a new camera
            /// </summary>
            createNew,

            /// <summary>
            /// Uses an existing camera
            /// </summary>
            existing
        }

        /// <summary>
        /// Types of new camera depth
        /// </summary>
        public enum NewCameraDepthType
        {
            /// <summary>
            /// Automatically calculates the depth of the camera to show the panorama on the front
            /// </summary>
            autoDetect,

            /// <summary>
            /// Use fixed camera depth
            /// </summary>
            manual
        }

        /// <summary>
        /// Projection of new camera
        /// </summary>
        public enum Projection
        {
            /// <summary>
            /// Perspective projection
            /// </summary>
            perspective,

            /// <summary>
            /// Orthographic projection
            /// </summary>
            orthographic
        }

        /// <summary>
        /// Types of rotation modes
        /// </summary>
        public enum RotationMode
        {
            /// <summary>
            /// Change pan and tilt rotate camera
            /// </summary>
            rotateCamera,

            /// <summary>
            /// Change pan and tilt rotate the panorama GameObject
            /// </summary>
            rotatePanorama,

            /// <summary>
            /// Change pan and tilt rotate the custom GameObject
            /// </summary>
            rotateGameObject
        }

        #endregion

        #region Actions

        /// <summary>
        /// Action that occurs when fov is changed
        /// </summary>
        public Action<float> OnFOVChanged;

        /// <summary>
        /// Allows you to intercept getting the current input position
        /// </summary>
        public Func<Vector2> OnGetInputPosition;

        /// <summary>
        /// Action that occurs when pan is changed
        /// </summary>
        public Action<float> OnPanChanged;

        /// <summary>
        /// Action that occurs when pano is destroyed
        /// </summary>
        public static Action<Pano> OnPanoDestroy;

        /// <summary>
        /// Action that occurs when pano is enabled
        /// </summary>
        public static Action<Pano> OnPanoEnabled;

        /// <summary>
        /// Action that occurs at the end of the Start method
        /// </summary>
        public static Action<Pano> OnPanoStarted;

        /// <summary>
        /// Action that occurs when pano is unloaded
        /// </summary>
        public static Action<Pano> OnPanoUnloaded;

        /// <summary>
        /// Action that occurs when tilt is changed
        /// </summary>
        public Action<float> OnTiltChanged;

        /// <summary>
        /// Function to verify and fix fov
        /// </summary>
        public Func<float, float> OnVerifyFOV;

        /// <summary>
        /// Function to verify and fix pan
        /// </summary>
        public Func<float, float> OnVerifyPan;

        /// <summary>
        /// Function to verify and fix tilt
        /// </summary>
        public Func<float, float> OnVerifyTilt;

        #endregion

        #region Fields

        public static Pano lastActivePano;

        /// <summary>
        /// Is preview in edit mode
        /// </summary>
        public static bool isPreview;

        /// <summary>
        /// Interaction with the panorama is locked?
        /// </summary>
        [NonSerialized]
        public bool locked;

        [SerializeField]
        private float _fov = 60;

        [SerializeField]
        private float _pan;

        [SerializeField]
        private float _tilt;

        [SerializeField]
        private float _northPan;

        private Camera _camera;

        [SerializeField]
        private LayerMask _cameraCullingMask = -1;

        [SerializeField]
        private CameraType _cameraType = CameraType.createNew;

        [SerializeField]
        protected Camera _existingCamera;

        [SerializeField]
        private int _newCameraDepth = 1;

        [SerializeField]
        private NewCameraDepthType _newCameraDepthType = NewCameraDepthType.autoDetect;

        [SerializeField]
        private Projection _newCameraProjection = Projection.perspective;

        [SerializeField]
        private RotationMode _rotationMode = RotationMode.rotateCamera;

        [SerializeField]
        private GameObject _rotateGameObject;

        [SerializeField]
        private bool _addPhysicsRaycaster = true;

        private PanoRenderer _panoRenderer;

        private bool lockCamera;
        private bool started;

        #endregion

        #region Properties

        /// <summary>
        /// Reference to the camera displaying the panorama
        /// </summary>
        public Camera activeCamera
        {
            get { return _camera; }
        }

        public bool addPhysicsRaycaster
        {
            get { return _addPhysicsRaycaster; }
            set { _addPhysicsRaycaster = value; }
        }

        public CameraType cameraType
        {
            get { return _cameraType; }
            set
            {
                if (_cameraType == value) return;
                
                if (_camera != null)
                {
                    if (_cameraType == CameraType.createNew) Destroy(_camera.gameObject);
                    _camera = null;
                }

                _cameraType = value;
            }
        }

        public Camera existingCamera
        {
            get { return _existingCamera; }
            set { _existingCamera = value; }
        }

        /// <summary>
        /// Gets and sets the fov
        /// </summary>
        public float fov
        {
            get { return _fov; }
            set
            {
                if (_fov == value) return;

                if (OnVerifyFOV != null) _fov = OnVerifyFOV(value);
                else _fov = value;

                if (Application.isPlaying)
                {
                    if (activeCamera != null && !activeCamera.orthographic) activeCamera.fieldOfView = fov;
                }

                if (OnFOVChanged != null) OnFOVChanged(_fov);
            }
        }

        /// <summary>
        /// Gets and sets the north pan
        /// </summary>
        public float northPan
        {
            get { return _northPan; }
            set
            {
                if (_northPan == value) return;

                _northPan = Mathf.Repeat(value, 360);
                UpdateRotation();
            }
        }

        /// <summary>
        /// Gets and sets the local pan
        /// </summary>
        public float localPan
        {
            get { return _pan; }
            set
            {
                if (_pan == value) return;

                if (OnVerifyPan != null) _pan = OnVerifyPan(value);
                else _pan = value;

                _pan = Mathf.Repeat(_pan, 360);

                if (OnPanChanged != null) OnPanChanged(pan);

                UpdateRotation();
            }
        }

        /// <summary>
        /// Gets and sets the pan (localPan - northPan)
        /// </summary>
        public float pan
        {
            get { return localPan - northPan; }
            set { localPan = value + northPan; }
        }

        /// <summary>
        /// Gets the reference to a pano renderer
        /// </summary>
        public PanoRenderer panoRenderer
        {
            get
            {
                if (_panoRenderer == null) _panoRenderer = GetComponent<PanoRenderer>();
                return _panoRenderer;
            }
        }


        /// <summary>
        /// Gets and sets the tilt
        /// </summary>
        public float tilt
        {
            get { return _tilt; }
            set
            {
                if (_tilt == value) return;

                if (OnVerifyTilt != null) _tilt = OnVerifyTilt(value);
                else _tilt = value;

                if (OnTiltChanged != null) OnTiltChanged(_tilt);

                UpdateRotation();
            }
        }

        /// <summary>
        /// Gets and sets the rotation mode
        /// </summary>
        public RotationMode rotationMode
        {
            get { return _rotationMode; }
            set
            {
                _rotationMode = value;
                UpdateRotation();
            }
        }

        /// <summary>
        /// Gets and sets GameObject that will rotate
        /// </summary>
        public GameObject rotateGameObject
        {
            get { return _rotateGameObject; }
            set
            {
                _rotateGameObject = value;
                UpdateRotation();
            }
        }

        #endregion

        #region Methods

        private void CreateNewCamera()
        {
            GameObject cameraGO = new GameObject("uPano Camera");
            cameraGO.transform.parent = transform;
            cameraGO.transform.localPosition = Vector3.zero;
            _camera = cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<PhysicsRaycaster>();

            if (_newCameraDepthType == NewCameraDepthType.manual) _camera.depth = _newCameraDepth;
            else
            {
                if (Camera.main != null) _camera.depth = Camera.main.depth + 1;
                else _camera.depth = 1;
            }

            _camera.orthographic = _newCameraProjection == Projection.orthographic;
            if (_camera.orthographic)
            {
                OrthographicCameras orthoCams = gameObject.AddComponent<OrthographicCameras>();
                orthoCams.cameras = new[] {_camera};
            }
            _camera.cullingMask = _cameraCullingMask;
        }

        public Vector2 GetInputPosition()
        {
            if (OnGetInputPosition != null) return OnGetInputPosition();

            if (InputManager.touchSupported && InputManager.touchCount > 0)
            {
                return InputManager.touches.Aggregate(Vector2.zero, (c, t) => c + t.position) / InputManager.touchCount;
            }
            return InputManager.mousePosition;
        }

        /// <summary>
        /// Look at point by pan and tilt
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        public void LookAt(float pan, float tilt)
        {
            lockCamera = true;
            this.pan = pan;
            this.tilt = tilt;
            lockCamera = false;
            UpdateRotation();
        }

        private void OnDestroy()
        {
            if (lastActivePano == this) lastActivePano = null;
            if (OnPanoDestroy != null) OnPanoDestroy(this);
        }

        protected void OnEnable()
        {
            lastActivePano = this;

            if (_cameraType == CameraType.createNew && _rotationMode != RotationMode.rotateCamera)
            {
                Debug.LogWarning("When creating a new camera, you can only rotate the camera");
                _rotationMode = RotationMode.rotateCamera;
            }
            UpdateRotation();

            if (OnPanoEnabled != null) OnPanoEnabled(this);
        }

        private void Start()
        {
            started = true;
            UpdateRotation();

            if (OnPanoStarted != null) OnPanoStarted(this);
        }

        public void Unload()
        {
            panoRenderer.DestroyMesh();

            if (OnPanoUnloaded != null) OnPanoUnloaded(this);
        }

        /// <summary>
        /// Updates the rotation
        /// </summary>
        public void UpdateRotation()
        {
            if (!Application.isPlaying || lockCamera) return;

            if (activeCamera == null)
            {
                if (_cameraType == CameraType.createNew) CreateNewCamera();
                else if (_cameraType == CameraType.existing)
                {
                    if (_existingCamera == null) return;
                    _camera = _existingCamera;
                    if (_addPhysicsRaycaster && _camera.GetComponent<PhysicsRaycaster>() == null)
                    {
                        _camera.gameObject.AddComponent<PhysicsRaycaster>();
                    }
                }

                if (!_camera.orthographic) _camera.fieldOfView = _fov;
            }

            if (started && panoRenderer != null) panoRenderer.UpdateRotation(_camera, _pan, _tilt);
        }

        /// <summary>
        /// Verifies fov
        /// </summary>
        public void VerifyFOV()
        {
            if (OnVerifyFOV != null) fov = OnVerifyFOV(_fov);
        }

        /// <summary>
        /// Verifies pan
        /// </summary>
        public void VerifyPan()
        {
            if (OnVerifyPan != null) pan = OnVerifyPan(_pan);
        }

        /// <summary>
        /// Verifies tilt
        /// </summary>
        public void VerifyTilt()
        {
            if (OnVerifyTilt != null) tilt = OnVerifyTilt(_tilt);
        }

        #endregion
    }
}