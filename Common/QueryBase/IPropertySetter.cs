using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.QueryBase
{
    public interface IPropertySetter
    {
        void SetValue(string propertyName, object entity, object value);
        object GetValue(string propertyName, object entity);
        string GetColumnName(int index);
        int ColumnCount { get; }
    }
}
