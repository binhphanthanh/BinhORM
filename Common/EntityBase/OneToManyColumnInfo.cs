using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SystemFramework.Common.EntityBase
{
    public class OneToManyColumnInfo : ColumnInfo
    {
        private FetchType fetch;

        public OneToManyColumnInfo(string colName, PropertyInfo prop, FetchType fetch)
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
