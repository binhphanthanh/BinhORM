using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SystemFramework.Common.EntityBase
{
    public class ColumnInfo
    {
        private string columnName;
        private bool multiLanguage;

        private PropertyInfo property;

        public ColumnInfo() { }

        public ColumnInfo(string colName, bool multi, PropertyInfo prop)
        {
            this.columnName = colName;
            this.multiLanguage = multi;
            this.property = prop;
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public bool MultiLanguage
        {
            get { return multiLanguage; }
            set { multiLanguage = value; }
        } 

        public PropertyInfo Property
        {
            get { return property; }
            set { property = value; }
        }        
    }
}
