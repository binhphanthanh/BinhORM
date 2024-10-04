using System;
using System.Collections.Generic;
using System.ComponentModel;
using SystemFramework.Common;
using System.Collections;
using System.Reflection;
using SystemFramework.Common.EntityBase;

namespace SystemFramework.Common.ObjectCollectionBase
{
    /// <summary>
    /// KeysCollection provides functions to manage the key of an entity. A key can has many Property.
    /// </summary>
    [Serializable]
    public class KeysCollection : List<KeyColumnInfo>
    {
        public KeysCollection() 
            : base()
        {
        }

        public KeysCollection(List<KeyColumnInfo> keyFields)
            : base(keyFields)
        {
        }

        public KeyValue GetKeyValue(BaseEntity entity)
        {
            if (entity == null)
                return null;

            KeyValue keyValue = new KeyValue();
            foreach (KeyColumnInfo info in this)
            {
                keyValue.Add(info.ColumnName, info.Property.GetValue(entity, null));
            }
            return keyValue;
        }

        public KeyValue CreateKey(params object[] list)
        {
            if (list.Length != this.Count)
            {
                throw new KeyParametersNotValidException();
            }

            KeyValue keyValue = new KeyValue();
            for (int i = 0; i < this.Count; i++)
            {
                keyValue.Add(this[i].ColumnName, list[i]);
            }
            return keyValue;
        }

        public bool Contain(string fieldName)
        {
            foreach (KeyColumnInfo info in this)
            {
                if (info.ColumnName.ToLower().Equals(fieldName.ToLower().Trim()))
                    return true;
            }
            return false;
        }
    }

    public class KeyValue : Dictionary<string, object>
    {
        public static bool operator ==(KeyValue value1, KeyValue value2)
        {
            if (value1.Count == 0)
                return false;

            KeyCollection keys = value1.Keys;
            foreach (string key in keys)
            {
                if (value1[key] == null || value2[key] == null)
                    return false;
                if (!value1[key].Equals(value2[key]))
                    return false;
            }
            return true;
        }

        public static bool operator !=(KeyValue value1, KeyValue value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return (this == (obj as KeyValue));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// If key type is number, it must greater than 0
        /// </summary>
        public bool IsKeyNull
        {
            get
            {
                ValueCollection values = this.Values;
                foreach (object value in values)
                {
                    if (value is string && string.IsNullOrEmpty((string)value))
                        return true;
                    else if ((value is int && 1 > (int)value) || (value is long && 1 > (long)value))
                        return true;
                }
                return false;
            }
        }
    }
}
