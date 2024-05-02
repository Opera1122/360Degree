/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace InfinityCode.uPano.Json
{
    /// <summary>
    /// The wrapper for JSON dictionary.
    /// </summary>
    public class JSONObject : JSONItem
    {
        private Dictionary<string, JSONItem> _table;

        /// <summary>
        /// Dictionary of items
        /// </summary>
        public Dictionary<string, JSONItem> table
        {
            get { return _table; }
        }

        public override JSONItem this[string key]
        {
            get { return Get(key); }
        }

        public override JSONItem this[int index]
        {
            get
            {
                if (index < 0) return null;

                int i = 0;
                foreach (KeyValuePair<string, JSONItem> pair in _table)
                {
                    if (i == index) return pair.Value;
                    i++;
                }
                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public JSONObject()
        {
            _table = new Dictionary<string, JSONItem>();
        }

        /// <summary>
        /// Adds element to the dictionary
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="value">Value</param>
        public void Add(string name, JSONItem value)
        {
            _table[name] = value;
        }

        public void Add(string name, object value)
        {
            if (value is string || value is bool || value is int || value is long || value is short || value is float || value is double) _table[name] = new JSONValue(value);
            else if (value is UnityEngine.Object)
            {
                _table[name] = new JSONValue((value as UnityEngine.Object).GetInstanceID());
            }
            else _table[name] = JSON.Serialize(value);
        }

        public void Add(string name, object value, JSONValue.ValueType valueType)
        {
            _table[name] = new JSONValue(value, valueType);
        }

        public override JSONItem AppendObject(object obj)
        {
            Combine(JSON.Serialize(obj));
            return this;
        }

        /// <summary>
        /// Combines two JSON Object.
        /// </summary>
        /// <param name="other">Other JSON Object</param>
        /// <param name="overwriteExistingValues">Overwrite the existing values?</param>
        public void Combine(JSONItem other, bool overwriteExistingValues = false)
        {
            JSONObject otherObj = other as JSONObject;
            if (otherObj == null) throw new Exception("Only JSONObject is allowed to be combined.");
            Dictionary<string, JSONItem> otherDict = otherObj.table;
            foreach (KeyValuePair<string, JSONItem> pair in otherDict)
            {
                if (overwriteExistingValues || !_table.ContainsKey(pair.Key)) _table[pair.Key] = pair.Value;
            }
        }

        public override object Deserialize(Type type)
        {
            IEnumerable<MemberInfo> members = ReflectionHelper.GetMembers(type, BindingFlags.Instance | BindingFlags.Public);
            return Deserialize(type, members);
        }

        /// <summary>
        /// Deserializes current element
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="members">Members of variable</param>
        /// <returns>Object</returns>
        public object Deserialize(Type type, IEnumerable<MemberInfo> members)
        {
            object v = Activator.CreateInstance(type);
            DeserializeObject(v, members);
            return v;
        }

        public void DeserializeObject(object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            IEnumerable<MemberInfo> members = ReflectionHelper.GetMembers(obj.GetType(), bindingFlags);
            DeserializeObject(obj, members);
        }

        public void DeserializeObject(object obj, IEnumerable<MemberInfo> members)
        {
            foreach (MemberInfo member in members)
            {
#if !NETFX_CORE
                MemberTypes memberType = member.MemberType;
                if (memberType != MemberTypes.Field && memberType != MemberTypes.Property) continue;
#else
                MemberTypes memberType;
                if (member is PropertyInfo) memberType = MemberTypes.Property;
                else if (member is FieldInfo) memberType = MemberTypes.Field;
                else continue;
#endif

                if (memberType == MemberTypes.Property && !((PropertyInfo)member).CanWrite) continue;
                JSONItem item;

#if !NETFX_CORE
                object[] attributes = member.GetCustomAttributes(typeof(JSON.AliasAttribute), true);
                JSON.AliasAttribute alias = attributes.Length > 0 ? attributes[0] as JSON.AliasAttribute : null;
#else
                IEnumerable<Attribute> attributes = member.GetCustomAttributes(typeof(JSON.AliasAttribute), true);
                JSON.AliasAttribute alias = null;
                foreach (Attribute a in attributes)
                {
                    alias = a as JSON.AliasAttribute;
                    break;
                }
#endif
                if (alias == null || !alias.ignoreFieldName)
                {
                    if (_table.TryGetValue(member.Name, out item))
                    {
                        Type t = memberType == MemberTypes.Field ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;
                        if (memberType == MemberTypes.Field) ((FieldInfo)member).SetValue(obj, item.Deserialize(t));
                        else ((PropertyInfo)member).SetValue(obj, item.Deserialize(t), null);
                        continue;
                    }
                }
                if (alias != null)
                {
                    for (int j = 0; j < alias.aliases.Length; j++)
                    {
                        if (_table.TryGetValue(alias.aliases[j], out item))
                        {
                            Type t = memberType == MemberTypes.Field ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;
                            if (memberType == MemberTypes.Field) ((FieldInfo)member).SetValue(obj, item.Deserialize(t));
                            else ((PropertyInfo)member).SetValue(obj, item.Deserialize(t), null);
                            break;
                        }
                    }
                }
            }
        }

        private JSONItem Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (key.Length > 2 && key[0] == '/' && key[1] == '/')
            {
                string k = key.Substring(2);
                if (string.IsNullOrEmpty(k) || k.StartsWith("//")) return null;
                return GetAll(k);
            }
            return GetThis(key);
        }

        private JSONItem GetThis(string key)
        {
            JSONItem item;
            int index = -1;
            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] == '/')
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                string k = key.Substring(0, index);
                if (!string.IsNullOrEmpty(k))
                {
                    if (_table.TryGetValue(k, out item))
                    {
                        string nextPart = key.Substring(index + 1);
                        return item[nextPart];
                    }
                }
                return null;
            }
            if (_table.TryGetValue(key, out item)) return item;
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
            var enumerator = _table.GetEnumerator();
            while (enumerator.MoveNext())
            {
                item = enumerator.Current.Value;
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
            return _table.Values.GetEnumerator();
        }

        /// <summary>
        /// Parse a string that contains JSON dictonary
        /// </summary>
        /// <param name="json">String that contains JSON dictonary</param>
        /// <returns>Instance</returns>
        public static JSONObject ParseObject(string json)
        {
            return JSON.Parse(json) as JSONObject;
        }

        public override void ToJSON(StringBuilder b)
        {
            b.Append("{");
            bool hasChilds = false;
            foreach (KeyValuePair<string, JSONItem> pair in _table)
            {
                b.Append("\"").Append(pair.Key).Append("\"").Append(":");
                pair.Value.ToJSON(b);
                b.Append(",");
                hasChilds = true;
            }
            if (hasChilds) b.Remove(b.Length - 1, 1);
            b.Append("}");
        }

        public override object Value(Type type)
        {
            if (ReflectionHelper.IsValueType(type)) return Activator.CreateInstance(type);
            return Deserialize(type);
        }
    }
}