using System;
using System.Collections.Generic;
using System.Text;
using SystemFramework.Common.ObjectCollectionBase;
using System.Reflection;

namespace SystemFramework.Common.EntityBase
{
    internal class EntityManager
    {
        private static Dictionary<string, EntityInfo> entityInfoPool = new Dictionary<string, EntityInfo>();

        public static EntityInfo GetEntityInfo(Type entityType)
        {
            EntityInfo ei;
            try
            {
                ei = entityInfoPool[entityType.FullName];
            }
            catch (Exception)
            {
                ei = BuildEntityInfo(entityType);
                entityInfoPool.Add(entityType.FullName ,ei);
            }
            return ei;
            
        }
                        
        public static KeysCollection GetKeysCollection(Type entityType)
        {
            KeysCollection keys;
            try
            {
                EntityInfo ei = entityInfoPool[entityType.FullName];
                keys = ei.KeysCollection;
            } 
            catch (Exception)
            {
                EntityInfo ei = BuildEntityInfo(entityType);
                entityInfoPool.Add(entityType.FullName, ei);
                keys = ei.KeysCollection;
            }
            return keys;
        }

        private static EntityInfo BuildEntityInfo(Type entityType)
        {
            EntityInfo ei = new EntityInfo();
            object[] tas = entityType.GetCustomAttributes(typeof(TableAttribute), false);
            if (tas.Length > 0)
            {
                TableAttribute ta = (TableAttribute)tas[0];
                ei.Name = ta.Name;
                ei.MultiLanguage = ta.MultiLanguage;

                PropertyInfo[] props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo p in props)
                {
                    object[] colAttrs = p.GetCustomAttributes(typeof(ColumnAttribute), true);
                    if (colAttrs.Length > 0)
                    {
                        ColumnInfo col;
                        object[] keyAttrs = p.GetCustomAttributes(typeof(KeyAttribute), true);
                        ColumnAttribute colAttr = (ColumnAttribute)colAttrs[0];
                        if (keyAttrs.Length > 0)
                        {
                            KeyAttribute keyAttr = (KeyAttribute)keyAttrs[0];
                            col = new KeyColumnInfo(colAttr.MappedName, p, keyAttr.AutoIncrement);
                            ei.KeysCollection.Add((KeyColumnInfo)col);
                        }
                        else
                        {
                            col = new ColumnInfo(colAttr.MappedName, colAttr.MultiLanguage, p);
                        }
                        ei.ListColumns.Add(col);
                    }
                    else
                    {
                        object[] oneToManyAttrs = p.GetCustomAttributes(typeof(OneToManyAttribute), true);
                        if (oneToManyAttrs.Length > 0)
                        {
                            OneToManyAttribute oneToManyAttr = (OneToManyAttribute)oneToManyAttrs[0];
                            ei.ListOneToManyColumns.Add(new OneToManyColumnInfo(oneToManyAttr.MappedName, p, oneToManyAttr.Fetch));
                        }
                        else
                        {
                            object[] manyToOneAttrs = p.GetCustomAttributes(typeof(ManyToOneAttribute), true);
                            if (manyToOneAttrs.Length > 0)
                            {
                                ManyToOneAttribute manyToOneAttr = (ManyToOneAttribute)manyToOneAttrs[0];
                                ei.ListColumns.Add(new ManyToOneColumnInfo(manyToOneAttr.MappedName, p, manyToOneAttr.Fetch));
                            }
                        }
                    }
                }
            }
            return ei;
        }
    }
}
