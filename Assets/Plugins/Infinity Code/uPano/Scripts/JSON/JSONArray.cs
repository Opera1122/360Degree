/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InfinityCode.uPano.Json
{
    /// <summary>
    /// The wrapper for an array of JSON elements.
    /// </summary>
    public class JSONArray : JSONItem
    {
        private List<JSONItem> array;
        private int _count;

        /// <summary>
        /// Count elements
        /// </summary>
        public int count
        {
            get { return _count; }
        }

        public override JSONItem this[int index]
        {
            get
            {
                if (index < 0 || index >= _count) return null;
                return array[index];
            }
        }


        public override JSONItem this[string key]
        {
            get { return Get(key); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public JSONArray()
        {
            array = new List<JSONItem>();
        }

        /// <summary>
        /// Adds an element to the array.
        /// </summary>
        /// <param name="item">Element</param>
        public void Add(JSONItem item)
        {
            array.Add(item);
            _count++;
        }

        /// <summary>
        /// Adds an elements to the array.
        /// </summary>
        /// <param name="collection">Array of elements</param>
        public void AddRange(JSONArray collection)
        {
            if (collection == null) return;
            array.AddRange(collection.array);
            _count += collection._count;
        }

        public void AddRange(JSONItem collection)
        {
            AddRange(collection as JSONArray);
        }

        public override object Deserialize(Type type)
        {
            if (_count == 0) return null;

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                Array v = Array.CreateInstance(elementType, _count);
                if (array[0] is JSONObject)
                {
                    IEnumerable<MemberInfo> members = ReflectionHelper.GetMembers(elementType, BindingFlags.Instance | BindingFlags.Public);
                    for (int i = 0; i < _count; i++)
                    {
                        JSONItem child = array[i];
                        object item = (child as JSONObject).Deserialize(elementType, members);
                        v.SetValue(item, i);
                    }
                }
                else
                {
                    for (int i = 0; i < _count; i++)
                    {
                        JSONItem child = array[i];
                        object item = child.Deserialize(elementType);
                        v.SetValue(item, i);
                    }
                }

                return v;
            }

            if (ReflectionHelper.IsGenericType(type))
            {
                Type listType = ReflectionHelper.GetGenericArguments(type)[0];
                object v = Activator.CreateInstance(type);

                if (array[0] is JSONObject)
                {
                    IEnumerable<MemberInfo> members = ReflectionHelper.GetMembers(listType, BindingFlags.Instance | BindingFlags.Public);
                    for (int i = 0; i < _count; i++)
                    {
                        JSONItem child = array[i];
                        object item = (child as JSONObject).Deserialize(listType, members);
                        try
                        {
                            MethodInfo methodInfo = ReflectionHelper.GetMethod(type, "Add");
                            if (methodInfo != null) methodInfo.Invoke(v, new[] {item});
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _count; i++)
                    {
                        JSONItem child = array[i];
                        object item = child.Deserialize(listType);
                        try
                        {
                            MethodInfo methodInfo = ReflectionHelper.GetMethod(type, "Add");
                            if (methodInfo != null) methodInfo.Invoke(v, new[] {item});
                        }
                        catch
                        {
                        }
                    }
                }

                return v;
            }


            return null;
        }

        private JSONItem Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (key.StartsWith("//"))
            {
                string k = key.Substring(2);
                if (string.IsNullOrEmpty(k) || k.StartsWith("//")) return null;
                return GetAll(k);
            }

            return GetThis(key);
        }

        private JSONItem GetThis(string key)
        {
            int kindex;

            if (key.Contains("/"))
            {
                int index = key.IndexOf("/");
                string k = key.Substring(0, index);
                string nextPart = key.Substring(index + 1);

                if (k == "*")
                {
                    JSONArray arr = new JSONArray();
                    for (int i = 0; i < _count; i++)
                    {
                        JSONItem item = array[i][nextPart];
                        if (item != null) arr.Add(item);
                    }

                    return arr;
                }

                if (int.TryParse(k, out kindex))
                {
                    if (kindex < 0 || kindex >= _count) return null;
                    JSONItem item = array[kindex];
                    return item[nextPart];
                }
            }

            if (key == "*") return this;
            if (int.TryParse(key, out kindex)) return this[kindex];
            return null;
        }


        public override JSONItem GetAll(string k)
        {
            JSONItem item = GetThis(k);
            JSONArray arr = null;
            if (item != null)
            {
                arr = new JSONArray();
                arr.Add(item);
            }

            for (int i = 0; i < _count; i++)
            {
                item = array[i];
                JSONArray subArr = item.GetAll(k) as JSONArray;
                if (subArr != null)
                {
                    if (arr == null) arr = new JSONArray();
                    arr.AddRange(subArr);
                }
            }

            return arr;
        }

        public override IEnumerator<JSONItem> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        /// <summary>
        /// Parse a string that contains an array
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>Instance</returns>
        public static JSONArray ParseArray(string json)
        {
            return JSON.Parse(json) as JSONArray;
        }

        public override void ToJSON(StringBuilder b)
        {
            b.Append("[");
            for (int i = 0; i < _count; i++)
            {
                if (i != 0) b.Append(",");
                array[i].ToJSON(b);
            }

            b.Append("]");
        }

        public override object Value(Type type)
        {
            if (ReflectionHelper.IsValueType(type)) return Activator.CreateInstance(type);
            return null;

        }
    }
}