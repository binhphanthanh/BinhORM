using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SystemFramework.Common.EntityBase
{
    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableAttribute : Attribute
    {
        private string name;
        private bool multiLanguage = false;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool MultiLanguage
        {
            get { return multiLanguage; }
            set { multiLanguage = value; }
        } 
    }
}
