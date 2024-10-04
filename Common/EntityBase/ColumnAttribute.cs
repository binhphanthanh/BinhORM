using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SystemFramework.Common.EntityBase
{
    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        private string mappedName;
        private bool multiLanguage = false;
        
        public string MappedName
        {
            get { return mappedName; }
            set { mappedName = value; }
        }

        public bool MultiLanguage
        {
            get { return multiLanguage; }
            set { multiLanguage = value; }
        } 
    }
}
