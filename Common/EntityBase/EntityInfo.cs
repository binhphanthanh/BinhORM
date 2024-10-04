using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SystemFramework.Common.ObjectCollectionBase;

namespace SystemFramework.Common.EntityBase
{
    public class EntityInfo
    {
        private string name;
        private bool multiLanguage;
        private KeysCollection keyColumn = new KeysCollection();
        private List<ColumnInfo> listColumns = new List<ColumnInfo>();
        private List<OneToManyColumnInfo> listOneToManyColumns = new List<OneToManyColumnInfo>();

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

        public KeysCollection KeysCollection
        {
            get { return keyColumn; }
            set { keyColumn = value; }
        }

        public List<ColumnInfo> ListColumns
        {
            get { return listColumns; }
            set { listColumns = value; }
        }

        public List<OneToManyColumnInfo> ListOneToManyColumns
        {
            get { return listOneToManyColumns; }
            set { listOneToManyColumns = value; }
        }
    }
}
