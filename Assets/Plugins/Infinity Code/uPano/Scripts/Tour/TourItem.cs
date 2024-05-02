/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Enums;
using InfinityCode.uPano.HotAreas;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.Tours
{
    [Serializable]
    public class TourItem: MonoBehaviour
    {
        public Vector2 position;

        [NonSerialized]
        private PanoRenderer _panoRenderer;

        [NonSerialized]
        private Tour _tour;

        private List<TourItem> _outLinks;

        private HotAreaManager _hotAreaManager;
        private HotSpotManager _hotSpotManager;
        private DirectionManager _directionManager;

        public DirectionManager directionManager
        {
            get { return _directionManager; }
        }

        public HotAreaManager hotAreaManager
        {
            get { return _hotAreaManager; }
        }

        public HotSpotManager hotSpotManager
        {
            get { return _hotSpotManager; }
        }

        public List<TourItem> outLinks
        {
            get
            {
                if (_outLinks == null) UpdateOutLinks();
                return _outLinks;
            }
        }

        public PanoRenderer panoRenderer
        {
            get { return _panoRenderer; }
        }

        public Tour tour
        {
            get { return _tour; }
            set { _tour = value; }
        }

        public Texture texture
        {
            get
            {
                ISingleTexturePanoRenderer singleTexturePanoRenderer = panoRenderer as ISingleTexturePanoRenderer;
                if (singleTexturePanoRenderer != null) return singleTexturePanoRenderer.texture;
                return null;
            }
            set
            {
                ISingleTexturePanoRenderer singleTexturePanoRenderer = panoRenderer as ISingleTexturePanoRenderer;
                if (singleTexturePanoRenderer != null) singleTexturePanoRenderer.texture = value;
            }
        }

        public static TourItem Create(Tour tour, Texture texture = null)
        {
            int index = tour.items.Count;
            string name;

            do
            {
                name = "Item " + index;
                index++;
            } while (tour.transform.Find(name) != null);

            GameObject target = new GameObject(name);
            target.transform.parent = tour.transform;
            TourItem item = target.AddComponent<TourItem>();
            item._tour = tour;

            Pano pano = target.AddComponent<Pano>();
            if (tour.preset == TourPreset.googleVR)
            {
                pano.cameraType = Pano.CameraType.existing;
                pano.addPhysicsRaycaster = false;
                pano.existingCamera = Camera.main;
                pano.rotationMode = Pano.RotationMode.rotateGameObject;
            }

            if (texture != null)
            {
                if (texture is Cubemap)
                {
                    CubemapPanoRenderer r = target.AddComponent<CubemapPanoRenderer>();
                    r.cubemap = texture as Cubemap;
                    item._panoRenderer = r;
                }
                else
                {
                    SphericalPanoRenderer r = target.AddComponent<SphericalPanoRenderer>();
                    r.texture = texture;
                    item._panoRenderer = r;
                }
            }
            else item._panoRenderer = target.AddComponent<SphericalPanoRenderer>();

            item._directionManager = target.AddComponent<DirectionManager>();
            item._hotSpotManager = target.AddComponent<HotSpotManager>();
            item._hotAreaManager = target.AddComponent<HotAreaManager>();

            target.SetActive(false);

            return item;
        }

        public void Init()
        {
            _panoRenderer = GetComponent<PanoRenderer>();
            _hotAreaManager = GetComponent<HotAreaManager>();
            _hotSpotManager = GetComponent<HotSpotManager>();
            _directionManager = GetComponent<DirectionManager>();
            UpdateOutLinks();
        }

        public void SetOutLink(InteractiveElement element, GameObject target)
        {
            element.switchToPanorama = target;
            UpdateOutLinks();
        }

        public void UpdateOutLinks()
        {
            if (_outLinks == null) _outLinks = new List<TourItem>();
            else _outLinks.Clear();

            IInteractiveElementList[] managers = 
            {
                hotSpotManager,
                directionManager,
                hotAreaManager
            };

            for (int i = 0; i < managers.Length; i++)
            {
                IInteractiveElementList manager = managers[i];
                if (manager == null) continue;

                for (int j = 0; j < manager.Count; j++)
                {
                    GameObject target = manager.GetItemAt(j).switchToPanorama;
                    if (target != null) _outLinks.Add(target.GetComponent<TourItem>());
                }
            }
        }
    }
}