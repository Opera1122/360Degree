/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Attributes;
using InfinityCode.uPano.InteractiveElements;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;

namespace InfinityCode.uPano.HotSpots
{
    /// <summary>
    /// List of HotSpots
    /// </summary>
    [WizardEnabled(true)]
    [AddComponentMenu("uPano/Hot Spots/Hot Spot Manager")]
    public class HotSpotManager : PrefabElementList<HotSpot>
    {
        [NonSerialized]
        public bool isPreview;

        internal HotSpotGlobalActions globalActions;

        /// <summary>
        /// Container containing instances of HotSpots
        /// </summary>
        public Transform container
        {
            get { return _container; }
        }

        public override void Add(HotSpot item)
        {
            if (item == null) return;

            items.Add(item);
            item.CreateInstance(this, _container);
        }

        public override void Clear()
        {
            foreach (HotSpot item in items) item.DestroyInstance();
            items.Clear();
        }

        /// <summary>
        /// Creates a new HotSpot and adds it to the list.
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        /// <param name="prefab">Prefab</param>
        /// <returns>HotSpot</returns>
        public HotSpot Create(float pan, float tilt, GameObject prefab)
        {
            HotSpot spot = new HotSpot(pan, tilt, prefab, this);
            Add(spot);
            return spot;
        }
        
        public override void Insert(int index, HotSpot item)
        {
            if (item == null) return;
            items.Insert(index, item);
            item.CreateInstance(this, _container);
        }

        protected override void OnEnableLate()
        {
            base.OnEnableLate();
            InitContainer("HotSpots");
        }

        public override bool Remove(HotSpot item)
        {
            if (item == null) return false;
            item.DestroyInstance();
            return items.Remove(item);
        }

        public override void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) return;
            HotSpot item = items[index];
            if (item != null) item.DestroyInstance();
            items.RemoveAt(index);
        }

        public override int RemoveAll(Predicate<HotSpot> match)
        {
            List<HotSpot> hotSpots = items.FindAll(match);
            foreach (HotSpot spot in hotSpots) spot.DestroyInstance();
            items.RemoveAll(match);
            return hotSpots.Count;
        }

        protected override void Start()
        {
            base.Start();

            CanvasUtils.GetEventSystem();

            foreach (HotSpot item in items)
            {
                if (item.instance == null) item.CreateInstance(this, _container);
            }
        }

        public void StartPreview(PanoRenderer panoRenderer)
        {
            GameObject previewContainer = new GameObject("__Preview HotSpots__");
            previewContainer.transform.SetParent(transform, false);
            isPreview = true;

            foreach (HotSpot item in _items)
            {
                if (item.instance == null) item.CreatePreview(panoRenderer, previewContainer.transform);
            }
        }

        public void StopPreview()
        {
            isPreview = false;
            Transform previewContainer = transform.Find("__Preview HotSpots__");
            if (previewContainer != null) DestroyImmediate(previewContainer.gameObject);
        }
    }
}