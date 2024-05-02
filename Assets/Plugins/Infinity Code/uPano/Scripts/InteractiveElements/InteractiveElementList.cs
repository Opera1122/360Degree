/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections;
using System.Collections.Generic;
using InfinityCode.uPano.Plugins;
using UnityEngine; 

namespace InfinityCode.uPano.InteractiveElements
{
    /// <summary>
    /// Base class for lists of interactive elements
    /// </summary>
    /// <typeparam name="T">Type of interactive element</typeparam>
    [Serializable]
    public abstract class InteractiveElementList<T> : Plugin, IList<T>, IInteractiveElementList
        where T: InteractiveElement
    {
        [SerializeField]
        protected List<T> _items;

        [NonSerialized]
        protected Transform _container;

        /// <summary>
        /// Reference to the list of elements
        /// </summary>
        public List<T> items
        {
            get
            {
                if (_items == null) _items = new List<T>();
                return _items;
            }
        }

        /// <summary>
        /// Gets and sets item by index
        /// </summary>
        /// <param name="index">Index of item</param>
        /// <returns>Item</returns>
        public T this[int index]
        {
            get { return items[index];}
            set { items[index] = value; }
        }

        /// <summary>
        /// Count of items
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">Item</param>
        public virtual void Add(T item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        public virtual void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Checks if the list contains an item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>True - contains, false - otherwise</returns>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copies the entire list to a compatible one-dimensional array, starting at the specified index of the target array
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from list</param>
        /// <param name="arrayIndex">Index in array at which copying begins</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Get items
        /// </summary>
        /// <returns>List of items</returns>
        public IList GetItems()
        {
            return items;
        }

        /// <summary>
        /// Get item by index
        /// </summary>
        /// <param name="index">Item index</param>
        /// <returns>Interactive element</returns>
        public InteractiveElement GetItemAt(int index)
        {
            return items[index];
        }

        /// <summary>
        /// Searches for the specified object and returns the index of the first occurrence within the entire list
        /// </summary>
        /// <param name="item">The object to locate in the list</param>
        /// <returns>The index of the first occurrence of item within the entire list, if found; otherwise, �1</returns>
        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        protected virtual void InitContainer(string name)
        {
            if (_container != null) return;

            _container = transform.Find(name);
            if (_container != null) return;

            GameObject go = new GameObject(name);
            _container = go.transform;
            _container.SetParent(transform, false);
            go.layer = gameObject.layer;
        }

        /// <summary>
        /// Inserts an element into the list at the specified index.
        /// </summary>
        /// <param name="index">Index at which item should be inserted</param>
        /// <param name="item">The object to insert</param>
        public virtual void Insert(int index, T item)
        {
            items.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list
        /// </summary>
        /// <param name="item">The object to remove from the list</param>
        /// <returns>True if item is successfully removed; otherwise, false</returns>
        public virtual bool Remove(T item)
        {
            return items.Remove(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the list
        /// </summary>
        /// <param name="index">Index of the element to remove</param>
        public virtual void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to remove</param>
        /// <returns>The number of elements removed from the list</returns>
        public abstract int RemoveAll(Predicate<T> match);
    }
}