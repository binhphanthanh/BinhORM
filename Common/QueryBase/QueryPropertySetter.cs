using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using SystemFramework.Common.EntityBase;

namespace SystemFramework.Common.QueryBase
{
    public class QueryPropertySetter : IPropertySetter
    {
        private static Dictionary<string, ObjectAccessor> _objectAccessors = new Dictionary<string, ObjectAccessor>();

        private string[] _columnNames;
        private int _columnCount = 0;
        private ObjectAccessor _accessor;

        public QueryPropertySetter(IDataReader reader, Type entity)
        {
            if(!_objectAccessors.ContainsKey(entity.FullName))
            {
                _accessor = EmitHelper.CreateObjectAccessor(entity);
                _objectAccessors.Add(entity.FullName, _accessor);
            }
            else
            {
                _accessor = _objectAccessors[entity.FullName];
            }
            InitCollumns(reader);
        }

        public string GetColumnName(int index)
        {
            return _columnNames[index];
        }

        public int ColumnCount
        {
            get { return _columnCount; }
        }

        public void SetValue(string propertyName, object entity, object value)
        {
            try
            {
                PropertyAccessor prop = _accessor[propertyName];
                
                if (prop != null)
                {
                    if (!prop.PropertyType.Equals(value.GetType()))
                    {
                        object targetValue = Convert.ChangeType(value, prop.PropertyType);
                        prop.Set(entity, targetValue);
                    }
                    else
                    {
                        prop.Set(entity, value);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public object GetValue(string propertyName, object entity)
        {
            try
            {
                IPropertyAccessor prop = _accessor[propertyName];
                return prop.Get(entity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void InitCollumns(IDataReader reader)
        {
            _columnCount = reader.FieldCount;
            _columnNames = new string[_columnCount];
            for (int i = 0; i < _columnCount; i++)
            {
                string propName = reader.GetName(i).ToLower();
                _columnNames[i] = propName;
            }
        }

        /*
        private void MapParamsToProperty(IDataReader reader, Type entity)
        {
            int columnsCount = reader.FieldCount;
            EntityInfo ei = EntityManager.GetEntityInfo(entity);
            for (int i = 0; i < columnsCount; i++)
            {
                string propName = reader.GetName(i);
                PropertyInfo prop = FindProperty(entity, ei, propName);
                if (prop != null)
                {
                    if(!this.ContainsKey(propName))
                        this.Add(propName, prop);
                }
            }
        }

        private PropertyInfo FindProperty(Type entity, EntityInfo ei, string propertyName)
        {
            for (int i = 0; i < ei.ListColumns.Count; i++)
            {
                if (propertyName.ToLower().Equals(ei.ListColumns[i].ColumnName.ToLower()))
                {
                    return ei.ListColumns[i].Property;
                }
            }
            return entity.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }
        */
    }
}
