/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Directions
{
    /// <summary>
    /// List of Directions
    /// </summary>
    [Serializable]
    [AddComponentMenu("uPano/Directions/Direction Manager")]
    public class DirectionManager: PrefabElementList<Direction>
    {
        [SerializeField]
        private float _externalRadius = 3;

        [SerializeField]
        private float _internalRadius = 1;

        [SerializeField]
        private float _verticalOffset = -1;

        internal DirectionGlobalActions globalActions;

        /// <summary>
        /// Radius from the camera to the center point around which the directions rotate
        /// </summary>
        public float externalRadius
        {
            get { return _externalRadius; }
            set
            {
                _externalRadius = value; 
                UpdatePosition();
            }
        }

        /// <summary>
        /// Radius from center point of rotation to Direction instance
        /// </summary>
        public float internalRadius
        {
            get { return _internalRadius; }
            set
            {
                _internalRadius = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Vertical offset of the center point
        /// </summary>
        public float verticalOffset
        {
            get { return _verticalOffset; }
            set
            {
                _verticalOffset = value;
                UpdatePosition();
            }
        }

        public override void Add(Direction item)
        {
            if (item == null) return;

            items.Add(item);
            item.CreateInstance(this, _container);
            item.UpdatePosition();
        }

        /// <summary>
        /// Creates a new Direction and adds it to the list.
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="prefab">Prefab</param>
        /// <returns>Direction</returns>
        public Direction Create(float pan, GameObject prefab)
        {
            Direction direction = new Direction(pan, prefab, this);
            Add(direction);
            return direction;
        }

        public override void Clear()
        {
            foreach (Direction item in items) item.DestroyInstance();
            items.Clear();
        }

        protected override void OnEnableLate()
        {
            base.OnEnableLate();

            InitContainer("Directions");

            if (_pano != null)
            {
                pano.OnPanChanged += OnPanoChanged;
                pano.OnTiltChanged += OnPanoChanged;
            }
        }

        private void OnPanoChanged(float f)
        {
            UpdatePosition();
        }

        protected override void Start()
        {
            base.Start();

            foreach (Direction item in items)
            {
                if (item.instance == null) item.CreateInstance(this, _container);
            }

            UpdatePosition();
        }

        public override bool Remove(Direction item)
        {
            if (item == null) return false;
            item.DestroyInstance();
            return items.Remove(item);
        }

        public override void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) return;
            Direction item = items[index];
            if (item != null) item.DestroyInstance();
            items.RemoveAt(index);
        }

        public override int RemoveAll(Predicate<Direction> match)
        {
            List<Direction> hotSpots = items.FindAll(match);
            foreach (Direction spot in hotSpots) spot.DestroyInstance();
            items.RemoveAll(match);
            return hotSpots.Count;
        }

        /// <summary>
        /// Updates the position of the center point and all items
        /// </summary>
        public void UpdatePosition()
        {
            if (!Application.isPlaying || _container == null || _items == null) return;

            _container.localPosition = (panoRenderer.GetWorldPosition(pano.pan, pano.tilt) - transform.position).normalized * _externalRadius + new Vector3(0, _verticalOffset, 0);

            foreach (Direction item in _items)
            {
                item.UpdatePosition();
            }
        }
    }
}
