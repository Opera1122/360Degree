/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Linq;
using InfinityCode.uPano.Enums;
using UnityEngine;

namespace InfinityCode.uPano.Tours
{
    [Serializable]
    public class Tour : MonoBehaviour
    {
        public Vector2 center;
        public float scale = 1;
        public TourPreset preset = TourPreset.standard;

        [NonSerialized]
        private List<TourItem> _items = null;

        [NonSerialized]
        private TourItem _startItem = null;

        public List<TourItem> items
        {
            get
            {
                if (_items == null)
                {
                    if (gameObject == null) return null;
                    _items = GetComponentsInChildren<TourItem>(true).ToList();
                }
                return _items;
            }
        }

        public TourItem startItem
        {
            get
            {
                if (_startItem == null && items.Count > 0)
                {
                    _startItem = _items.FirstOrDefault(i => i.gameObject.activeSelf);
                    if (_startItem == null)
                    {
                        _startItem = _items[0];
                        _startItem.gameObject.SetActive(true);
                    }
                }
                return _startItem;
            }
            set
            {
                if (_startItem != null) _startItem.gameObject.SetActive(false);
                _startItem = value;
                _startItem.gameObject.SetActive(true);
            }
        }

        public void ClearItems()
        {
            _items = null;
        }
    }
}
