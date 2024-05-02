/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Controls;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Examples
{
    /// <summary>
    /// Example of dynamic panorama creation
    /// </summary>
    [AddComponentMenu("uPano/Examples/DynamicCreationExample")]
    public class DynamicCreationExample : MonoBehaviour
    {
        /// <summary>
        /// Texture of spherical panorama
        /// </summary>
        public Texture sphericalTexture;

        /// <summary>
        /// Texture of cylindrical panorama
        /// </summary>
        public Texture2D cylindricalTexture;

        /// <summary>
        /// Texture of horizontal cross panorama
        /// </summary>
        public Texture2D horizontalCrossTexture;

        /// <summary>
        /// Textures of cube faces panorama
        /// </summary>
        public Texture2D[] cubeFacesTextures = new Texture2D[6];

        /// <summary>
        /// Cubemap of cubemap panorama
        /// </summary>
        public Cubemap cubemap;

        /// <summary>
        /// Shader for all types of panoramas except cubemap
        /// </summary>
        public Shader shader;

        /// <summary>
        /// 23.09.20 Shin // Material
        /// </summary>
        public Material material;
        /// <summary>
        /// UV for horizontal cross panorama
        /// </summary>
        public CubeUV horizontalCrossUV = CubeUVPresets.horizontalCrossPreset;

        /// <summary>
        /// Radius of spherical and cylindrical panorama
        /// </summary>
        public float radius = 10;

        /// <summary>
        /// Number of segments spherical panorama
        /// </summary>
        public int sphericalSegments = 16;

        /// <summary>
        /// Quality of icosahedron
        /// </summary>
        public int icosahedronQuality = 2;

        /// <summary>
        /// Height of cylindrical panorama
        /// </summary>
        public float cylindricalHeight = 20;

        /// <summary>
        /// Number of sides cylindrical panorama
        /// </summary>
        public int cylindricalSides = 16;

        /// <summary>
        /// The size of the cube-based panoramas
        /// </summary>
        public Vector3 cubeSize = new Vector3(10, 10, 10);

        /// <summary>
        /// Reference to PanoRenderer
        /// </summary>
        private PanoRenderer panoRenderer;

        /// <summary>
        /// Indicates that there is pan, tilt and fov from the previous panorama
        /// </summary>
        private bool hasPrevValues = false;

        /// <summary>
        /// Pan of the previous panorama
        /// </summary>
        private float pan;

        /// <summary>
        /// Tilt of the previous panorama
        /// </summary>
        private float tilt;

        /// <summary>
        /// Fov of the previous panorama
        /// </summary>
        private float fov;

        /// <summary>
        /// Creates spherical panorama
        /// </summary>
        public void CreateSphericalPano()
        {
            DestroyPrevPano();
            panoRenderer = SphericalPanoRenderer.CreateSphere(sphericalTexture, radius, sphericalSegments);
            InitPano();
        }

        /// <summary>
        /// Creates iconsahedron panorama
        /// </summary>
        public void CreateIconsahedron()
        {
            DestroyPrevPano();
            panoRenderer = SphericalPanoRenderer.CreateIcosahedron(sphericalTexture, radius, icosahedronQuality);
            InitPano();
        }

        /// <summary>
        /// Creates cylindrical panorama
        /// </summary>
        public void CreateCylindrical()
        {
            DestroyPrevPano();
            panoRenderer = CylindricalPanoRenderer.Create(cylindricalTexture, radius, cylindricalHeight, cylindricalSides);
            InitPano();
        }

        /// <summary>
        /// Creates horizontal cross panorama
        /// </summary>
        public void CreateHorizontalCross()
        {
            DestroyPrevPano();
            panoRenderer = SingleTextureCubeFacesPanoRenderer.Create(horizontalCrossTexture, cubeSize, horizontalCrossUV);
            InitPano();
        }

        /// <summary>
        /// Creates cube faces panorama
        /// </summary>
        public void CreateCubeFaces()
        {
            DestroyPrevPano();
            panoRenderer = CubeFacesPanoRenderer.Create(cubeFacesTextures, cubeSize);
            InitPano();
        }

        /// <summary>
        /// Creates cubemap panorama
        /// </summary>
        public void CreateCubemap()
        {
            DestroyPrevPano();
            panoRenderer = CubemapPanoRenderer.Create(cubemap, cubeSize);
            InitPano();
        }

        /// <summary>
        /// Destroys the previous instance of the panorama
        /// </summary>
        private void DestroyPrevPano()
        {
            if (panoRenderer == null) return;

            hasPrevValues = true;
            pan = panoRenderer.pano.pan;
            tilt = panoRenderer.pano.tilt;
            fov = panoRenderer.pano.fov;

            Destroy(panoRenderer.gameObject);
            panoRenderer = null;
        }

        /// <summary>
        /// Initializes the panorama
        /// </summary>
        private void InitPano()
        {
            // Adds control via keyboard and mouse
            panoRenderer.gameObject.AddComponent<MouseControl>();
            panoRenderer.gameObject.AddComponent<KeyboardControl>();

            // Adds limit values (pan, tilt, fov) of the panorama
            panoRenderer.gameObject.AddComponent<Limits>();

            // Sets shader for all types of panoramas except cubemap
            if (shader != null && !(panoRenderer is CubemapPanoRenderer)) panoRenderer.shader = shader;

            // Set Material
            //if (material != null && panoRenderer.defaultMaterial == null) panoRenderer.defaultMaterial = material;
            panoRenderer.defaultMaterial = material;

            // Restores pan, tilt, fov from the previous instance of the panorama
            if (hasPrevValues)
            {
                panoRenderer.pano.pan = pan;
                panoRenderer.pano.tilt = tilt;
                panoRenderer.pano.fov = fov;
            }
        }
    }
}