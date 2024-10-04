using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SystemFramework.Common.EntityBase
{
    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Property, Inherited = false)] 
    public class KeyAttribute : Attribute
    {
        private bool autoIncrement = false;

        public bool AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; }
        }

        
    }
}
