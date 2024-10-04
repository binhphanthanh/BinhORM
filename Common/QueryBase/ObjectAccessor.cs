using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.QueryBase
{
    public class ObjectAccessor
    {
        private Dictionary<string, PropertyAccessor> _propertyAccessors = new Dictionary<string, PropertyAccessor>();

        public PropertyAccessor this[string propertyName]
        {
            get
            {
                PropertyAccessor value = null;
                _propertyAccessors.TryGetValue(propertyName, out value);
                return value;
            }
            set
            {
                propertyName = propertyName.ToLower();
                if (!_propertyAccessors.ContainsKey(propertyName))
                {
                    _propertyAccessors.Add(propertyName, value);
                }
            }
        }
    }
}
