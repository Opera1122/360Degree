/*           INFINITY CODE           */
/*     https://infinity-code.com     */

namespace InfinityCode.uPano
{
    /// <summary>
    /// Presets for SingleTextureCubeFacesPanoRenderer.cubeUV
    /// </summary>
    public static class CubeUVPresets
    {
        /// <summary>
        /// Preset of UV for horizontal cross panorama
        /// </summary>
        public static CubeUV horizontalCrossPreset
        {
            get
            {
                return new CubeUV(
                    new RotatableRectUV(.251f, .999f, .499f, .667f),
                    new RotatableRectUV(.251f, .665f, .499f, .334f),
                    new RotatableRectUV(.001f, .665f, .249f, .334f),
                    new RotatableRectUV(.751f, .665f, .999f, .334f),
                    new RotatableRectUV(.501f, .665f, .749f, .334f),
                    new RotatableRectUV(.251f, .332f, .499f, .001f)
                );
            }
        }

        /// <summary>
        /// Preset of UV for vertical cross panorama
        /// </summary>
        public static CubeUV verticalCrossPreset
        {
            get
            {
                return new CubeUV(
                    new RotatableRectUV(.334f, .999f, .665f, .751f),
                    new RotatableRectUV(.334f, .749f, .665f, .501f),
                    new RotatableRectUV(.001f, .749f, .332f, .501f),
                    new RotatableRectUV(.665f, .001f, .334f, .249f),
                    new RotatableRectUV(.667f, .749f, .999f, .501f),
                    new RotatableRectUV(.334f, .499f, .665f, .251f)
                );
            }
        }

        /// <summary>
        /// Preset of UV for YouTube video panorama
        /// </summary>
        public static CubeUV youtubePreset
        {
            get
            {
                return new CubeUV(
                    new RotatableRectUV(.667f, .499f, .999f, .001f, RotatableRectUV.Rotation.rotation90),
                    new RotatableRectUV(.334f, .999f, .666f, .501f),
                    new RotatableRectUV(.001f, .999f, .333f, .501f),
                    new RotatableRectUV(.334f, .499f, .666f, .001f, RotatableRectUV.Rotation.rotation270),
                    new RotatableRectUV(.667f, .999f, .999f, .501f),
                    new RotatableRectUV(.001f, .499f, .333f, .001f, RotatableRectUV.Rotation.rotation90)
                );
            }
        }
    }
}