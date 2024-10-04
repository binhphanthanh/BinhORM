using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SystemFramework.Common.EntityBase
{
    public class ManyToOneColumnInfo: ColumnInfo
    {
        private FetchType fetch;

        public ManyToOneColumnInfo(string colName, PropertyInfo prop, FetchType fetch)
            : base(colName, false, prop)
        {
            this.fetch = fetch;
        }

        public FetchType Fetch
        {
            get { return fetch; }
            set { fetch = value; }
        }
    }
}
