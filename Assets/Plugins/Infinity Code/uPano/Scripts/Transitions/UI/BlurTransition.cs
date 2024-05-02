/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Transitions.UI
{
    /// <summary>
    /// Blur transition
    /// </summary>
    [AddComponentMenu("uPano/Transitions/UI/Blur")]
    public class BlurTransition : UITransition
    {
        /// <summary>
        /// Blur material
        /// </summary>
        public Material blurMaterial;

        /// <summary>
        /// The name of the material field that is responsible for the blur radius
        /// </summary>
        public string radiusFieldName = "_Radius";

        /// <summary>
        /// Initial blur raduis
        /// </summary>
        public float fromRadius = 0;

        /// <summary>
        /// Target blur radius
        /// </summary>
        public float toRadius = 30;

        private Material material;

        protected override string containerName
        {
            get { return "__uPano Blur__"; }
        }

        protected override void Dispose()
        {
            base.Dispose();

            Destroy(material);
        }

        protected override void Finish()
        {
            started = false;
        }

        public override void Init()
        {
            base.Init();

            if (blurMaterial == null) material = new Material(Shader.Find("uPano/Transitions/ImageBlur"));
            else material = Instantiate(blurMaterial);

            Image image = displayGameObject.AddComponent<Image>();
            image.material = material;

            FitToScreen(image);

            rootTransition.OnFinish += t => Dispose();
        }

        public override void Process()
        {
            base.Process();

            material.SetFloat(radiusFieldName, Mathf.Lerp(fromRadius, toRadius, curvedProgress));
        }
    }
}