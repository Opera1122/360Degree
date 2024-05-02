/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using InfinityCode.uPano.Actions;
using UnityEngine;
using UnityEngine.Serialization;

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// The base class for interactive elements like HotSpot, Direction, etc.
    /// </summary>
    [Serializable]
    public abstract class InteractiveElement
    {
        /// <summary>
        /// Event that occurs when click Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnClick = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when press Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerDown = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when release Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerUp = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when cursor enter on Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerEnter = new InteractiveElementEvent();

        /// <summary>
        /// Event that occurs when cursor exit from Interactive Element
        /// </summary>
        [SerializeField]
        public InteractiveElementEvent OnPointerExit = new InteractiveElementEvent();

        /// <summary>
        /// Should the element ignore global actions?
        /// </summary>
        public bool ignoreGlobalActions = false;

        /// <summary>
        /// Prefab of panorama that should be instantiated and opened when you click on an element
        /// </summary>
        [FormerlySerializedAs("targetPanorama")]
        public GameObject loadPanoramaPrefab;

        /// <summary>
        /// Panorama that should be opened when you click on an element
        /// </summary>
        public GameObject switchToPanorama;

        /// <summary>
        /// Copy pan and tilt from source to target panorama
        /// </summary>
        public bool copyPanTilt = true;

        /// <summary>
        /// Prefab which contains the transition that is played before the panorama is closed
        /// </summary>
        public GameObject beforeTransitionPrefab;

        /// <summary>
        /// Prefab which contains the transition that is played after the panorama is closed
        /// </summary>
        public GameObject afterTransitionPrefab;

        [SerializeField]
        protected bool _expanded = true;

        [SerializeField]
        protected bool _expandedEvents = false;

        [SerializeField]
        protected bool _expandedQuickActions = false;

        [SerializeField]
        protected string _title;

        [SerializeField]
        protected bool _visible = true;

        private Dictionary<string, object> _runtimeFields;
        private Pano _pano;

        /// <summary>
        /// Reference to instance of a panorama
        /// </summary>
        public Pano pano
        {
            get { return _pano; }
            set { _pano = value; }
        }

        /// <summary>
        /// Gets and sets the user value to be stored in an element
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public object this[string key]
        {
            get
            {
                if (_runtimeFields == null) return null;
                object f;
                _runtimeFields.TryGetValue(key, out f);
                return f;
            }
            set
            {
                if (_runtimeFields == null) _runtimeFields = new Dictionary<string, object>();
                _runtimeFields[key] = value;
            }
        }

        /// <summary>
        /// Title
        /// </summary>
        public virtual string title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Visible
        /// </summary>
        public virtual bool visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
            }
        }

        /// <summary>
        /// Destroy an interactive element
        /// </summary>
        public virtual void Destroy()
        {
            _runtimeFields = null;
        }

        /// <summary>
        /// Initiates quick actions
        /// </summary>
        /// <param name="target">The container that will contain the Quick Actions components</param>
        public virtual void InitQuickActions(GameObject target)
        {
            bool useTransition = true;

            if (loadPanoramaPrefab != null)
            {
                LoadAnotherPanorama loadAnotherPanorama = target.AddComponent<LoadAnotherPanorama>();
                loadAnotherPanorama.prefab = loadPanoramaPrefab;
                loadAnotherPanorama.beforeTransitionPrefab = beforeTransitionPrefab;
                loadAnotherPanorama.afterTransitionPrefab = afterTransitionPrefab;

                OnClick.AddListener(loadAnotherPanorama.Invoke);

                useTransition = false;
            }

            if (switchToPanorama != null)
            {
                SetGameObjectActive sgo = target.AddComponent<SetGameObjectActive>();
                sgo.beforeTransitionPrefab = beforeTransitionPrefab;
                sgo.afterTransitionPrefab = afterTransitionPrefab;
                sgo.useTransition = useTransition;
                
                sgo.items = new []
                {
                    new SetGameObjectActive.Item
                    {
                        target = target.GetComponentInParent<Pano>().gameObject,
                        value = false
                    },
                    new SetGameObjectActive.Item
                    {
                        target = switchToPanorama,
                        value = true
                    }
                };

                OnClick.AddListener(sgo.Invoke);

                if (copyPanTilt)
                {
                    CopyPanTilt cpt = target.AddComponent<CopyPanTilt>();
                    cpt.source = target.GetComponentInParent<Pano>();
                    cpt.target = switchToPanorama.GetComponent<Pano>();
                    cpt.useTransition = false;
                    OnClick.AddListener(cpt.Invoke);
                }
            }
        }

        /// <summary>
        /// Gets the pan and tilt of an interactive element
        /// </summary>
        /// <param name="pan">Pan</param>
        /// <param name="tilt">Tilt</param>
        public abstract void GetPanTilt(out float pan, out float tilt);

        /// <summary>
        /// Gets user value for the key and cast to the specified type
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public T GetRuntimeField<T>(string key)
        {
            if (_runtimeFields == null) return default(T);
            object f;
            _runtimeFields.TryGetValue(key, out f);
            return (T)f;
        }

        /// <summary>
        /// Reinitializes an interactive element
        /// </summary>
        public abstract void Reinit();

        /// <summary>
        /// Sets pan and tilt for an interactive element
        /// </summary>
        /// <param name="pan">New pan</param>
        /// <param name="tilt">New tilt</param>
        public abstract void SetPanTilt(float pan, float tilt);
    }
}