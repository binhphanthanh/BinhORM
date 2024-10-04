
using System;
using System.ComponentModel;
using System.Reflection;
using SystemFramework;
using System.Collections;
using SystemFramework.Common.ObjectCollectionBase;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Collections.Generic;
using SystemFramework.Common.EntityBase;

namespace SystemFramework.Common
{
    [Serializable]
    public class BaseEntity : ValueUpdatable, ICloneable
    {
        [NonSerialized]
        private KeysCollection keys;

        private string languageKey;

        public string LanguageKey
        {
            get { return languageKey; }
            set { languageKey = value; }
        }

        /// <summary>
        /// Constructor, set added to the entity when it was created
        /// </summary>
        public BaseEntity()
        {
            _state = EntityState.Added;
            _isEditing = false;
        }

        [Browsable(false)]
        public KeyValue KeyValue
        {
            get
            {
                return KeysCollection.GetKeyValue(this);
            }
        }

        [Browsable(false)]
        internal KeysCollection KeysCollection
        {
            get
            {
                if (keys == null)
                    keys = EntityManager.GetKeysCollection(this.GetType());
                return keys;
            }
        }

        public static KeyValue CreateKey<T>(params object[] list) where T : BaseEntity
        {
            KeysCollection k = EntityManager.GetKeysCollection(typeof(T));
            return k.CreateKey(list);
        }

        /// <summary>
        /// Gets the value at a field
        /// </summary>
        /// <param name="propertyName">The name of a field</param>
        /// <returns>Value of this field</returns>
        public object GetValue(string propertyName)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return GetValue(property);
        }

        /// <summary>
        /// Gets the value at a field
        /// </summary>
        /// <param name="propertyName">The name of a field</param>
        /// <param name="value">Value of this field</value>
        public void SetValue(string propertyName, object value)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            SetValue(property, value);
        }

        /// <summary>
        /// Gets the value at a field
        /// </summary>
        /// <param name="propertyName">The name of a field</param>
        /// <returns>Value of this field</returns>
        public object GetValue(PropertyInfo property)
        {
            if (property != null)
            {
                return property.GetValue(this, null);
            }
            return null;
        }

        /// <summary>
        /// Gets the value at a field
        /// </summary>
        /// <param name="propertyName">The name of a field</param>
        /// <param name="value">Value of this field</value>
        public void SetValue(PropertyInfo property, object value)
        {
            if (property != null)
            {
                property.SetValue(this, value, null);
            }
        }


        /// <summary>
        /// Copy all data from the source entity to this entity
        /// </summary>
        /// <param name="entity">The source contains data</param>
        public object Copy()
        {
            byte[] c = this.GetBytes();
            return Load(c);
        }

        /// <summary>
        /// Create a new entity that has the same data as this entity but
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return Copy();
        }

        public byte[] GetBytes()
        {
            Stream stream = null;
            try
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf1 = new BinaryFormatter();
                bf1.Serialize(ms, this);
                return ms.ToArray();

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }

        public object Load(byte[] content)
        {
            if (content == null)
                return null;

            Stream stream = null;
            try
            {
                MemoryStream ms = new MemoryStream(content);
                BinaryFormatter bf1 = new BinaryFormatter();
                ms.Position = 0;

                return bf1.Deserialize(ms);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }
    }
}
