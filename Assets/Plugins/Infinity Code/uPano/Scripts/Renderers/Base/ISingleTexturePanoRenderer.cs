/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;

namespace InfinityCode.uPano.Renderers.Base
{
    /// <summary>
    /// Interface for panoramas consisting of single texture
    /// </summary>
    public interface ISingleTexturePanoRenderer
    {
        /// <summary>
        /// Gets and sets the texture of the panorama
        /// </summary>
        Texture texture { get; set; }

        /// <summary>
        /// Download panorama by URL
        /// </summary>
        /// <param name="url">URL of panorama</param>
        void Download(string url);

        /// <summary>
        /// First, download a low-resolution panorama, then a full panorama.
        /// </summary>
        /// <param name="lowresURL">URL of low-resolution panorama</param>
        /// <param name="url">URL of full-resolution panorama</param>
        void Download(string lowresURL, string url);

        /// <summary>
        /// Converts UV of the texture to pan and tilt
        /// </summary>
        /// <param name="uv">UV</param>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns>True - success, false - otherwise</returns>
        bool GetPanTiltByUV(Vector2 uv, out float pan, out float tilt);

        /// <summary>
        /// Converts pan and tilt to UV of the texture
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <returns>True - success, false - otherwise</returns>
        Vector2 GetUV(float pan, float tilt);
    }
}