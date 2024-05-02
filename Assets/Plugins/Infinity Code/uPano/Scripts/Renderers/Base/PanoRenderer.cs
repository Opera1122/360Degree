/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano.Renderers.Base
{
    /// <summary>
    /// Base class of pano renderer
    /// </summary>
    [RequireComponent(typeof(Pano))]
    [DisallowMultipleComponent]
    public abstract class PanoRenderer : MonoBehaviour
    {
        /// <summary>
        /// Action that occurs when the pano renderer is started
        /// </summary>
        public Action OnStart;

        /// <summary>
        /// Action that occurs when the mesh is updated
        /// </summary>
        public Action OnUpdate;

        /// <summary>
        /// Compress textures to DXT format to reduce memory usage.
        /// </summary>
        public bool compressTextures = true;

        [SerializeField]
        protected Material _defaultMaterial;

        protected Pano _pano;

        [SerializeField]
        protected Shader _shader;

        [SerializeField]
        protected string _mainTex = "_MainTex";

        protected bool hasMesh;

        protected GameObject _meshGameObject;
        protected MeshRenderer meshRenderer;
        protected MeshFilter meshFilter;
        protected Mesh mesh;
        protected MeshCollider meshCollider;
        private bool isFirstCreate = true;

        /// <summary>
        /// The material that will be used to display the panorama
        /// </summary>
        public Material defaultMaterial
        {
            get { return _defaultMaterial; }
            set
            {
                if (value == _defaultMaterial) return;
                _defaultMaterial = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Default shader of a pano renderer
        /// </summary>
        public virtual Shader defaultShader
        {
            get { return Shader.Find("Unlit/Texture"); }
        }

        /// <summary>
        /// Reference to panorama
        /// </summary>
        public Pano pano
        {
            get
            {
                if (_pano == null) _pano = GetComponent<Pano>();
                return _pano;
            }
        }

        /// <summary>
        /// Reference to mesh GameObject
        /// </summary>
        public GameObject meshGameObject
        {
            get { return _meshGameObject; }
        }

        /// <summary>
        /// The shader that will be used to display the panorama. 
        /// If the material is specified, the shader will be ignored
        /// </summary>
        public Shader shader
        {
            get
            {
                if (_shader == null) _shader = defaultShader;
                return _shader;
            }
            set
            {
                if (value == null) value = defaultShader;
                if (_shader == value) return;
                _shader = value;
                UpdateShader();
            }
        }

        protected virtual void CreateMaterial()
        {
            
        }

        /// <summary>
        /// Creates panorama mesh
        /// </summary>
        /// <returns>True - success, false - otherwise</returns>
        public virtual bool CreateMesh()
        {
            if (!ValidateFields()) return false;
            InitMesh();
            mesh.Clear();

            hasMesh = true;

            if (isFirstCreate)
            {
                if (OnStart != null) OnStart();
                isFirstCreate = false;
            }
            else if (OnUpdate != null) OnUpdate();

            return true;
        }

        /// <summary>
        /// Destroys panorama mesh
        /// </summary>
        public void DestroyMesh()
        {
            DestroyMeshItems();
            hasMesh = false;
        }

        protected virtual void DestroyMeshItems()
        {
            if (meshRenderer != null) DestroyImmediate(meshRenderer);
            if (meshFilter != null) DestroyImmediate(meshFilter);
            if (meshCollider != null) DestroyImmediate(meshCollider);
            if (mesh != null) DestroyImmediate(mesh);
            if (_meshGameObject != null) DestroyImmediate(_meshGameObject);
        }

        /// <summary>
        /// Gets pan and tilt for the screen point
        /// </summary>
        /// <param name="screenPosX">Screen point X</param>
        /// <param name="screenPosY">Screen point Y</param>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns>True - success, false - otherwise</returns>
        public bool GetPanTiltByScreenPosition(float screenPosX, float screenPosY, out float pan, out float tilt)
        {
            return GetPanTiltByScreenPosition(new Vector2(screenPosX, screenPosY), out pan, out tilt);
        }

        /// <summary>
        /// Gets pan and tilt for the screen point
        /// </summary>
        /// <param name="screenPoint">Screen point</param>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns>True - success, false - otherwise</returns>
        public bool GetPanTiltByScreenPosition(Vector2 screenPoint, out float pan, out float tilt)
        {
            pan = tilt = 0;
            if (screenPoint == Vector2.zero) return false;

            RaycastHit hit;
            if (!Physics.Raycast(_pano.activeCamera.ScreenPointToRay(screenPoint), out hit)) return false;

            GetPanTiltByWorldPosition(hit.point, out pan, out tilt);
            return true;
        }

        /// <summary>
        /// Gets pan and tilt for the world position
        /// </summary>
        /// <param name="worldPosition">Position in Unity World Space</param>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        public virtual void GetPanTiltByWorldPosition(Vector3 worldPosition, out float pan, out float tilt)
        {
            Vector3 point = (Quaternion.Inverse(transform.rotation) * worldPosition - transform.position).normalized;
            pan = 180 - MathHelper.Angle2D(Vector3.zero, point);
            point = Quaternion.AngleAxis(-pan, Vector3.up) * point;
            tilt = MathHelper.Angle2D(0, 0, point.x, point.y);

            pan -= 90 + pano.northPan;

            tilt = -180 - tilt;
            if (tilt < -180) tilt += 360;
        }

        /// <summary>
        /// Gets pan and tilt under cursor
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns>True - success, false - otherwise</returns>
        public bool GetPanTiltUnderCursor(out float pan, out float tilt)
        {
            return GetPanTiltByScreenPosition(pano.GetInputPosition(), out pan, out tilt);
        }

        /// <summary>
        /// Gets the screen position for pan and tilt
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <param name="cam">Camera</param>
        /// <returns>Position in Screen Space</returns>
        public Vector3 GetScreenPosition(float pan, float tilt, Camera cam = null)
        {
            if (cam == null) cam = pano.activeCamera;
            return cam.WorldToScreenPoint(GetWorldPosition(pan, tilt));
        }

        /// <summary>
        /// Gets Unity World Position for pan and tilt
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns>Position in Unity World Space</returns>
        public abstract Vector3 GetWorldPosition(float pan, float tilt);

        protected void InitMesh()
        {
            Transform tr = gameObject.transform.Find("Pano Mesh");
            if (tr != null) _meshGameObject = tr.gameObject;
            else
            {
                _meshGameObject = new GameObject("Pano Mesh");
                _meshGameObject.transform.parent = transform;
                _meshGameObject.transform.localPosition = Vector3.zero;
                _meshGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _meshGameObject.transform.localScale = Vector3.one;
                _meshGameObject.layer = gameObject.layer;
            }

            meshFilter = _meshGameObject.GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = _meshGameObject.AddComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;

            meshCollider = _meshGameObject.GetComponent<MeshCollider>();
            if (meshCollider == null) meshCollider = _meshGameObject.AddComponent<MeshCollider>();

            meshRenderer = _meshGameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) meshRenderer = _meshGameObject.AddComponent<MeshRenderer>();

            CreateMaterial();
        }

        protected void OnEnable()
        {
            _pano = GetComponent<Pano>();
            if (!hasMesh) CreateMesh();
        }

        protected virtual void OnDestroy()
        {
            _pano = null;
            _shader = null;
            Destroy(mesh);
            mesh = null;
            hasMesh = false;
            if (meshCollider != null && meshCollider.sharedMesh != null) Destroy(meshCollider.sharedMesh);
            meshCollider = null;
            meshRenderer = null;
            meshFilter = null;
            _defaultMaterial = null;
            _meshGameObject = null;
        }

        protected virtual void Start()
        {
            //CreateMesh();
        }

        protected void SetMainTexture(Material m, Texture t)
        {
            if (m.HasProperty("_MainTex")) m.mainTexture = t;
            else if (m.HasProperty(_mainTex)) m.SetTexture(_mainTex, t);
        }

        /// <summary>
        /// Updates panorama mesh
        /// </summary>
        public virtual void UpdateMesh()
        {
            if (!hasMesh) return;

            DestroyMeshItems();
            CreateMesh();
        }

        /// <summary>
        /// Updates the rotation
        /// </summary>
        /// <param name="activeCamera">The camera used to display the panorama</param>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        public virtual void UpdateRotation(Camera activeCamera, float pan, float tilt)
        {
            if (_pano == null) return;
            if (_pano.rotationMode == Pano.RotationMode.rotateCamera)
            {
                Vector3 newEulerAngles = new Vector3(-tilt, pan, 0);
                Transform tf = activeCamera.transform;
                if (newEulerAngles != tf.localEulerAngles) tf.localEulerAngles = newEulerAngles;
            }
            else if (_pano.rotationMode == Pano.RotationMode.rotateGameObject)
            {
                if (_pano.rotateGameObject == null) return;
                Vector3 newEulerAngles = new Vector3(-tilt, pan, 0);
                Transform tf = _pano.rotateGameObject.transform;
                if (newEulerAngles != tf.localEulerAngles) tf.localEulerAngles = newEulerAngles;
            }
            else
            {
                Quaternion q = activeCamera.transform.rotation;
                q *= Quaternion.Euler(tilt, 0, 0);
                q *= Quaternion.Euler(0, -pan, 0);
                if (q != transform.localRotation) transform.localRotation = q;
            }
        }

        /// <summary>
        /// Updates material shader
        /// </summary>
        protected virtual void UpdateShader()
        {
            
        }

        protected abstract bool ValidateFields();
    }
}