/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace InfinityCode.uPano.Json
{
    /// <summary>
    /// The base class of JSON elements.
    /// </summary>
    public abstract class JSONItem : IEnumerable<JSONItem>
    {
        /// <summary>
        /// Get the element by index
        /// </summary>
        /// <param name="index">Index of element</param>
        /// <returns>Element</returns>
        public abstract JSONItem this[int index] { get; }

        /// <summary>
        /// Get the element by index
        /// </summary>
        /// <param name="index1">Index of element</param>
        /// <param name="index2">Index of element</param>
        /// <returns>Element</returns>
        public JSONItem this[int index1, int index2]
        {
            get
            {
                JSONItem v = this[index1];
                if (v == null) return null;
                return v[index2];
            }
        }

        /// <summary>
        /// Get the element by index
        /// </summary>
        /// <param name="index1">Index of element</param>
        /// <param name="index2">Index of element</param>
        /// <param name="index3">Index of element</param>
        /// <returns>Element</returns>
        public JSONItem this[int index1, int index2, int index3]
        {
            get
            {
                JSONItem v = this[index1];
                if (v == null) return null;
                v = v[index2];
                if (v == null) return null;
                return v[index3];
            }
        }

        /// <summary>
        /// Get the element by index
        /// </summary>
        /// <param name="index1">Index of element</param>
        /// <param name="index2">Index of element</param>
        /// <param name="index3">Index of element</param>
        /// <param name="index4">Index of element</param>
        /// <returns>Element</returns>
        public JSONItem this[int index1, int index2, int index3, int index4]
        {
            get
            {
                JSONItem v = this[index1];
                if (v == null) return null;
                v = v[index2];
                if (v == null) return null;
                v = v[index3];
                if (v == null) return null;
                return v[index4];
            }
        }

        public JSONItem this[int i1, int i2, int i3, int i4, int i5]
        {
            get
            {
                JSONItem v = this[i1];
                if (v == null) return null;
                v = v[i2];
                if (v == null) return null;
                v = v[i3];
                if (v == null) return null;
                v = v[i4];
                if (v == null) return null;
                return v[i5];
            }
        }

        public JSONItem this[int i1, int i2, int i3, int i4, int i5, int i6]
        {
            get
            {
                JSONItem v = this[i1];
                if (v == null) return null;
                v = v[i2];
                if (v == null) return null;
                v = v[i3];
                if (v == null) return null;
                v = v[i4];
                if (v == null) return null;
                v = v[i5];
                if (v == null) return null;
                return v[i6];
            }
        }

        public JSONItem this[int i1, int i2, int i3, int i4, int i5, int i6, int i7]
        {
            get
            {
                JSONItem v = this[i1];
                if (v == null) return null;
                v = v[i2];
                if (v == null) return null;
                v = v[i3];
                if (v == null) return null;
                v = v[i4];
                if (v == null) return null;
                v = v[i5];
                if (v == null) return null;
                v = v[i6];
                if (v == null) return null;
                return v[i7];
            }
        }

        /// <summary>
        /// Get the element by key.\n
        /// Supports XPath like selectors:\n
        /// ["key"] - get element by key.\n
        /// ["key1/key2"] - get element key2, which is a child of the element key1.\n
        /// ["key/N"] - where N is number. Get array element by index N, which is a child of the element key1.\n
        /// ["key/*"] - get all array elements, which is a child of the element key1.\n
        /// ["//key"] - get all elements with the key on the first or the deeper levels of the current element. \n
        /// </summary>
        /// <param name="key">Element key</param>
        /// <returns>Element</returns>
        public abstract JSONItem this[string key] { get; }

        /// <summary>
        /// Serializes the object and adds to the current json node.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Current json node</returns>
        public virtual JSONItem AppendObject(object obj)
        {
            throw new Exception("AppendObject is only allowed for JSONObject.");
        }

        /// <summary>
        /// Returns the value of the child element, converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of variable</typeparam>
        /// <param name="childName">Child element key</param>
        /// <returns>Value</returns>
        public T ChildValue<T>(string childName)
        {
            JSONItem el = this[childName];
            if (el == null) return default(T);
            return el.Value<T>();
        }

        /// <summary>
        /// Deserializes current element
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Object</returns>
        public T Deserialize<T>()
        {
            return (T) Deserialize(typeof(T));
        }

        /// <summary>
        /// Deserializes current element
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Object</returns>
        public abstract object Deserialize(Type type);

        /// <summary>
        /// Get all elements with the key on the first or the deeper levels of the current element.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Elements</returns>
        public abstract JSONItem GetAll(string key);

        /// <summary>
        /// Converts the current and the child elements to JSON string.
        /// </summary>
        /// <param name="b">StringBuilder instance</param>
        public abstract void ToJSON(StringBuilder b);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<JSONItem> GetEnumerator()
        {
            return null;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            ToJSON(b);
            return b.ToString();
        }

        /// <summary>
        /// Returns the value of the element, converted to the specified type.
        /// </summary>
        /// <param name="type">Type of variable</param>
        /// <returns>Value</returns>
        public abstract object Value(Type type);

        /// <summary>
        /// Returns the value of the element, converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of variable</typeparam>
        /// <returns>Value</returns>
        public virtual T Value<T>()
        {
            return (T) Value(typeof(T));
        }

        /// <summary>
        /// Returns the value of the element, converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of variable</typeparam>
        /// <returns>Value</returns>
        public T V<T>()
        {
            return Value<T>();
        }

        /// <summary>
        /// Returns the value of the child element, converted to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of variable</typeparam>
        /// <param name="childName">Child element key</param>
        /// <returns>Value</returns>
        public T V<T>(string childName)
        {
            return ChildValue<T>(childName);
        }
    }
}