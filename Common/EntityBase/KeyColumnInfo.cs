using System;
using System.Collections.Generic;
using System.Text;
using SystemFramework.Common.ObjectCollectionBase;
using System.Reflection;

namespace SystemFramework.Common.EntityBase
{
    public class KeyColumnInfo : ColumnInfo
    {
        private bool autoIncrement;

        public KeyColumnInfo()
        {
        }

        public KeyColumnInfo(string colName, PropertyInfo prop, bool autoIncrement) 
            : base(colName, false, prop)
        {
            this.autoIncrement = autoIncrement;
        }

        public bool AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; }
        }
    }
}
