/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.uPano.Plugins
{
    [AddComponentMenu("uPano/Plugins/TimedGaze")]
    [RequireComponent(typeof(HotSpotGlobalActions), typeof(DirectionGlobalActions))]
    public class TimedGaze : Plugin
    {
        public GameObject gazeCircle;
        public Image frontCircleImage;
        public Component disableComponent;
        public GameObject disableGameObject;
        public float delayBefore = 0.1f;
        public float delayAfter = 0.1f;
        public float time = 2f;

        private bool started = false;
        private float t = 0;
        private HotSpotGlobalActions hsga;
        private DirectionGlobalActions dga;
        private InteractiveElement targetElement;

        private void HideCircle(InteractiveElement element)
        {
            gazeCircle.SetActive(false);
            started = false;
            targetElement = null;
            if (disableComponent != null)
            {
                if (disableComponent is Renderer) (disableComponent as Renderer).enabled = true;
                else if (disableComponent is MonoBehaviour) (disableComponent as MonoBehaviour).enabled = true;
            }
            if (disableGameObject != null) disableGameObject.SetActive(true);
        }

        private void ShowCircle(InteractiveElement element)
        {
            t = 0;
            gazeCircle.SetActive(true);
            started = true;
            targetElement = element;
            if (disableComponent != null)
            {
                if (disableComponent is Renderer) (disableComponent as Renderer).enabled = false;
                else if (disableComponent is MonoBehaviour) (disableComponent as MonoBehaviour).enabled = false;
            }
            if (disableGameObject != null) disableGameObject.SetActive(false);
            UpdateProgress();
        }

        protected override void Start()
        {
            gazeCircle.SetActive(false);
            hsga = GetComponent<HotSpotGlobalActions>();
            hsga.OnPointerEnter.AddListener(ShowCircle);
            hsga.OnPointerExit.AddListener(HideCircle);

            dga = GetComponent<DirectionGlobalActions>();
            hsga.OnPointerEnter.AddListener(ShowCircle);
            hsga.OnPointerExit.AddListener(HideCircle);
        }

        private void Update()
        {
            if (!started) return;

            t += Time.deltaTime;
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            Ray ray = new Ray(targetElement.pano.activeCamera.transform.position, targetElement.pano.activeCamera.transform.rotation * Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                gazeCircle.transform.position = hit.point;
                gazeCircle.transform.LookAt(targetElement.pano.transform);
            }

            float ct = t - delayBefore;
            if (ct < 0) ct = 0;
            float progress = ct / time;
            if (progress > 1) progress = 1;
            
            frontCircleImage.fillAmount = 1 - progress;

            ct -= time;
            if (ct > delayAfter)
            {
                if (targetElement is HotSpot) hsga.OnClick.Invoke(targetElement);
                else if (targetElement is Direction) dga.OnClick.Invoke(targetElement);
                targetElement.OnClick.Invoke(targetElement);
                HideCircle(null);
            }
        }
    }
}