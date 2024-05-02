/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using InfinityCode.uPano.Renderers.Base;
using InfinityCode.uPano.Requests;
using UnityEngine;

namespace InfinityCode.uPano.Renderers
{
    /// <summary>
    /// Pano renderer for displaying a panorama, each side of which is a separate texture
    /// </summary>
    [AddComponentMenu("uPano/Renderers/CubeFacesPanoRenderer")]
    public class CubeFacesPanoRenderer : CubePanoRenderer<CubeFacesPanoRenderer>
    {
        [SerializeField]
        private  Texture2D[] _textures;

        private Material[] materials;

        /// <summary>
        /// Gets and sets array of panorama textures. Order: 0-Top, 1-Front, 2-Left, 3-Back, 4-Right, 5-Bottom
        /// </summary>
        public Texture2D[] textures
        {
            get
            {
                if (_textures == null || _textures.Length != 6) _textures = new Texture2D[6];
                return _textures;
            }
            set
            {
                if (_textures == value) return;
                if (value == null) throw new Exception("Array of textures can not be null");
                if (value.Length != 6) throw new Exception("Array of textures must have a size of 6");
                _textures = value;
                if (hasMesh && materials != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        SetMainTexture(materials[i], textures[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new panorama and pano renderer
        /// </summary>
        /// <param name="textures">Array of panorama textures. Order: 0-Top, 1-Front, 2-Left, 3-Back, 4-Right, 5-Bottom</param>
        /// <param name="size">Size of cube mesh</param>
        /// <returns>Instance of pano renderer</returns>
        public static CubeFacesPanoRenderer Create(Texture2D[] textures, Vector3 size)
        {
            if (textures == null) throw new Exception("Textures can not be null.");
            if (textures.Length != 6) throw new Exception("Textures.Length must be 6.");
            
            CubeFacesPanoRenderer panoRenderer = CreatePanoInstance();
            panoRenderer._textures = textures;
            panoRenderer._size = size;
            return panoRenderer;
        }

        protected override void CreateMaterial()
        {
            base.CreateMaterial();

            if (materials == null) materials = new Material[6];

            for (int i = 0; i < 6; i++)
            {
                if (materials[i] == null)
                {
                    Material m;
                    if (defaultMaterial != null) m = Instantiate(defaultMaterial);
                    else m = new Material(shader);
                    materials[i] = m;
                    m.name = CubeUV.sides[i];
                }
            }
            meshRenderer.sharedMaterials = materials;
        }

        public override bool CreateMesh()
        {
            if (!base.CreateMesh()) return false;

            for (int i = 0; i < 6; i++) SetMainTexture(materials[i], _textures[i]);
            return true;
        }

        protected override int[][] CreateTriangles()
        {
            int[][] triangles = new int[6][];
            for (int i = 0; i < 6; i++)
            {
                int i4 = i * 4;

                int[] t = new int[6];

                t[0] = i4;
                t[1] = i4 + 1;
                t[2] = i4 + 2;
                t[3] = i4;
                t[4] = i4 + 2;
                t[5] = i4 + 3;
                triangles[i] = t;
            }
            return triangles;
        }

        protected override Vector2[] CreateUV()
        {
            Vector2[] uv = new Vector2[24];

            int j = 0;
            for (int i = 0; i < 6; i++)
            {
                uv[j++] = new Vector2(0, 1);
                uv[j++] = new Vector2(1, 1);
                uv[j++] = new Vector2(1, 0);
                uv[j++] = new Vector2(0, 0);
            }

            return uv;
        }

        /// <summary>
        /// Download panorama sides by URLs
        /// </summary>
        /// <param name="urls">URLs of panorama sides</param>
        public void Download(string[] urls)
        {
            if (urls == null) throw new Exception("URLs can not be null");
            if (urls.Length != 6) throw new Exception("URLs of panorama sides must have a size of 6");

            for (int i = 0; i < 6; i++)
            {
                WWWRequest request = new WWWRequest(urls[i]);
                request.OnComplete += r =>
                {
                    if (r.hasErrors)
                    {
                        Debug.LogWarning(r.error);
                        return;
                    }

                    try
                    {
                        SetSide(i, r.texture);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                };
            }
        }

        /// <summary>
        /// First, download a low-resolution sides of the panorama, then a full sides of the panorama.
        /// </summary>
        /// <param name="lowresURLs">URLs of low-resolution sides of the panorama</param>
        /// <param name="urls">URLs of full-resolution sides of the panorama</param>
        public void Download(string[] lowresURLs, string[] urls)
        {
            if (urls == null) throw new Exception("URLs can not be null");
            if (urls.Length != 6) throw new Exception("URLs of panorama sides must have a size of 6");

            for (int i = 0; i < 6; i++)
            {
                int j = i;
                new WWWRequest(lowresURLs[j]).OnComplete += r =>
                {
                    if (r.hasErrors)
                    {
                        Debug.LogWarning(r.error);
                        return;
                    }

                    try
                    {
                        if (_textures[j] != null) DestroyImmediate(_textures[j]);

                        SetSide(j, r.texture);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    new WWWRequest(urls[j]).OnComplete += r2 =>
                    {
                        if (r2.hasErrors)
                        {
                            Debug.LogWarning(r2.error);
                            return;
                        }

                        try
                        {
                            if (_textures[j] != null) DestroyImmediate(_textures[j]);

                            SetSide(j, r2.texture);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    };
                };
            }
        }

        /// <summary>
        /// Sets the texture for the side of the cube.
        /// </summary>
        /// <param name="index">Index of the side (0-Top, 1-Front, 2-Left, 3-Back, 4-Right, 5-Bottom)</param>
        /// <param name="texture">Texture of the side</param>
        public void SetSide(int index, Texture2D texture)
        {
            if (!hasMesh || materials == null || _textures == null) return;
            if (index < 0 || index > 5) throw new Exception("The index should be in the range of 0 to 5");
            _textures[index] = texture;
            SetMainTexture(materials[index], texture);
        }

        protected override bool ValidateFields()
        {
            return true;
        }

        protected override void UpdateShader()
        {
            if (!hasMesh || materials == null) return;

            foreach (Material material in materials)
            {
                if (material != null) material.shader = shader;
            }
        }
    }
}