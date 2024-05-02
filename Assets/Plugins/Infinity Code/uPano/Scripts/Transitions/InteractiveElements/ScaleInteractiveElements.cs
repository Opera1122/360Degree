/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using System.Linq;
using InfinityCode.uPano.Directions;
using InfinityCode.uPano.HotSpots;
using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Transitions.InteractiveElements
{
    /// <summary>
    /// Set the scale of interactive elements
    /// </summary>
    [AddComponentMenu("uPano/Transitions/Interactive Elements/Scale Interactive Elements")]
    public class ScaleInteractiveElements : TimeBasedTransition
    {
        /// <summary>
        /// Enumeration of the types of interactive elements for which the transition will occur
        /// </summary>
        public enum ElementType
        {
            /// <summary>
            /// Only active element. Note: This can not be used in After Transitions
            /// </summary>
            activeElement,

            /// <summary>
            /// All the interactive elements of all types that support scaling
            /// </summary>
            allElements,

            /// <summary>
            /// All interactive elements of the same type as an integrative element that caused the transition. Note: This can not be used in After Transitions
            /// </summary>
            allSameTypeElements
        }

        /// <summary>
        /// Type of interactive elements for which the transition will occur
        /// </summary>
        public ElementType elementType = ElementType.allElements;

        /// <summary>
        /// Initial scale
        /// </summary>
        public Vector3 fromScale = Vector3.one;

        /// <summary>
        /// The initial scale is the scale of the interactive element?
        /// </summary>
        public bool fromScaleIsOriginal = true;

        /// <summary>
        /// Target scale
        /// </summary>
        public Vector3 toScale;

        /// <summary>
        /// The target scale is the scale of the interactive element?
        /// </summary>
        public bool toScaleIsOriginal = false;

        private List<InteractiveElement> elements;

        protected override void Dispose()
        {
            base.Dispose();

            elements = null;
        }

        public override void Init()
        {
            elements = new List<InteractiveElement>();

            Pano pano = FindObjectOfType<Pano>();
            if (pano == null)
            {
                return;
            }

            if (elementType == ElementType.activeElement) elements.Add(element);
            else if (elementType == ElementType.allSameTypeElements)
            {
                if (element is HotSpot) elements.AddRange((element as HotSpot).manager.items.Cast<InteractiveElement>());
                else if (element is Direction) elements.AddRange((element as Direction).manager.items.Cast<InteractiveElement>());
            }
            else if (elementType == ElementType.allElements)
            {
                HotSpotManager hsManager = pano.GetComponent<HotSpotManager>();
                if (hsManager != null) elements.AddRange(hsManager.items.Cast<InteractiveElement>());

                DirectionManager dManager = pano.GetComponent<DirectionManager>();
                if (dManager != null) elements.AddRange(dManager.items.Cast<InteractiveElement>());
            }

            foreach (InteractiveElement e in elements)
            {
                Vector3 s = (e as IScalableElement).scale;
                e["__fromScale__"] = fromScaleIsOriginal? s: fromScale;
                e["__toScale__"] = toScaleIsOriginal? s: toScale;
            }
        }

        public override void Process()
        {
            foreach (InteractiveElement e in elements)
            {
                Vector3 s1 = (Vector3)e["__fromScale__"];
                Vector3 s2 = (Vector3)e["__toScale__"];
                (e as IScalableElement).scale = Vector3.Lerp(s1, s2, curvedProgress);
            }
        }
    }
}