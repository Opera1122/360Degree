/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Requests;
using UnityEngine;

namespace InfinityCode.uPano.Renderers.Base
{
    /// <summary>
    /// Base class of pano renderers that have a single texture
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleTexturePanoRenderer<T> : PanoRenderer<T>, ISingleTexturePanoRenderer
        where T: SingleTexturePanoRenderer<T>
    {
        [SerializeField]
        protected Texture _texture;

        protected Material _material;

        /// <summary>
        /// The texture of the panorama
        /// </summary>
        public Texture texture
        {
            get { return _texture; }
            set
            {
                if (_texture == value) return;

                if (Application.isPlaying)
                {
                    if (_texture != null)
                    {
                        if (!(_texture is RenderTexture) && _texture.GetInstanceID() < 0) Destroy(_texture);
                    }
                    if (value != null && compressTextures)
                    {
                        Texture2D texture2D = value as Texture2D;
                        if (texture2D != null && texture2D.isReadable) texture2D.Compress(true);
                    }
                }

                _texture = value;

                if (hasMesh) SetMainTexture(_material, value);
            }
        }

        /// <summary>
        /// The material used to display the panorama 
        /// </summary>
        public Material material
        {
            get { return _material; }
            set
            {
                if (_material == value) return;
                if (Application.isPlaying && _material != null) DestroyImmediate(_material);

                _material = value;
                if (meshRenderer != null) meshRenderer.sharedMaterial = value;
            }
        }

        protected override void CreateMaterial()
        {
            base.CreateMaterial();

            if (_material != null) meshRenderer.sharedMaterial = _material;
            else if (defaultMaterial != null) material = Instantiate(defaultMaterial);
            else material = new Material(shader);
        }

        public override bool CreateMesh()
        {
            if (!base.CreateMesh()) return false;

            SetMainTexture(_material, texture);
            return true;
        }

        protected override void DestroyMeshItems()
        {
            base.DestroyMeshItems();

            if (_material != null) DestroyImmediate(_material);
        }

        /// <summary>
        /// Download panorama by URL
        /// </summary>
        /// <param name="url">URL of panorama</param>
        public void Download(string url)
        {
            TextureRequest request = new TextureRequest(url);
            request.OnComplete += OnRequestComplete;
        }

        /// <summary>
        /// First, download a low-resolution panorama, then a full panorama.
        /// </summary>
        /// <param name="lowresURL">URL of low-resolution panorama</param>
        /// <param name="url">URL of full-resolution panorama</param>
        public void Download(string lowresURL, string url)
        {
            TextureRequest request = new TextureRequest(lowresURL);
            request.OnComplete += r =>
            {
                OnLowResRequestComplete(r);
                Download(url);
            };
        }

        public abstract bool GetPanTiltByUV(Vector2 uv, out float pan, out float tilt);

        public abstract Vector2 GetUV(float pan, float tilt);

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_texture != null)
            {
                if (!(_texture is RenderTexture) && _texture.GetInstanceID() < 0) Destroy(texture);
                _texture = null;
            }

            if (_material != null)
            {
                if (_material.GetInstanceID() < 0) Destroy(_material);
                _material = null;
            }
        }

        private void OnLowResRequestComplete(WWWRequest request)
        {
            if (request.hasErrors)
            {
                Debug.LogWarning(request.error);
                return;
            }

            try
            {
                texture = request.texture;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }

        private void OnRequestComplete(WWWRequest request)
        {
            if (request.hasErrors)
            {
                Debug.LogWarning(request.error);
                return;
            }

            try
            {
                texture = request.texture;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected override void UpdateShader()
        {
            if (!hasMesh || _material == null) return;

            _material.shader = shader;
        }
    }
}