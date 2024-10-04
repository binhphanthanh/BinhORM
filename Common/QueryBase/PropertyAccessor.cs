using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace SystemFramework.Common.QueryBase
{
    /// <summary>
    /// The PropertyAccessor class provides fast dynamic access
    /// to a property of a specified target class.
    /// </summary>
    public class PropertyAccessor : IPropertyAccessor
    {
        private bool _canRead;
        private bool _canWrite;
        private string _property;
        private Type _propertyType;
        private IPropertyAccessor _accessor;

        public PropertyAccessor(IPropertyAccessor accessor, Type propertyType, string property, Boolean canRead, bool canWrite)
        {
            _canRead = canRead;
            _canWrite = canWrite;
            _property = property;
            _propertyType = propertyType;
            _accessor = accessor;
        }

        public object Get(object target)
        {
            if (_canRead)
            {
                return this._accessor.Get(target);
            }
            else
            {
                throw new PropertyAccessorException(
                    string.Format("Property \"{0}\" does not have a get method.",
                    _property));
            }
        }

        /// <summary>
        /// Sets the property for the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="value">Value to set.</param>
        public void Set(object target, object value)
        {
            if (_canWrite)
            {
                this._accessor.Set(target, value);
            }
            else
            {
                throw new PropertyAccessorException(
                    string.Format("Property \"{0}\" does not have a set method.",
                    _property));
            }
        }

        /// <summary>
        /// Whether or not the Property supports read access.
        /// </summary>
        public bool CanRead
        {
            get
            {
                return this._canRead;
            }
        }

        /// <summary>
        /// Whether or not the Property supports write access.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return this._canWrite;
            }
        }
       
        /// <summary>
        /// The Type of the Property being accessed.
        /// </summary>
        public Type PropertyType
        {
            get
            {
                return this._propertyType;
            }
        }
    }
}
