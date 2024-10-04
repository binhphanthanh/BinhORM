using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SystemFramework.Common.EntityBase
{
    [ComVisible(true)]
    [AttributeUsage(AttributeTargets.Property, Inherited = false)] 
    public class ManyToOneAttribute : ColumnAttribute
    {
        private FetchType fetch;

        public FetchType Fetch
        {
            get { return fetch; }
            set { fetch = value; }
        }
    }
}
