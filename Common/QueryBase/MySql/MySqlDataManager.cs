using System;
using SystemFramework.Common.ObjectCollectionBase;
using SystemFramework.Common.EntityBase;

namespace SystemFramework.Common.QueryBase.MySql
{
    public class MySqlDataManager : DataManager
    {

        public MySqlDataManager(BaseBusiness baseBusiness) 
            : base(baseBusiness)
        {
            
        }

        override protected string PreparePaging(string sql, int startRecord, int maxResult)
        {
            return sql + " limit " + startRecord + ", " + maxResult;
        }

        protected override ParameterManager CreateParameterManager()
        {
            return new MySqlParameterManager();
        }

        public override int Insert<T>(T entity)
        {
            EntityInfo ei = EntityManager.GetEntityInfo(entity.GetType());

            string sql = "insert into " + ei.Name + "({0}) values({1}) \n" + "{2}";
            string table = "";
            string values = "";
            string selectId = "";
            KeysCollection keys = ei.KeysCollection;
            for (int i = 0; i < ei.ListColumns.Count; i++)
            {
                ColumnInfo ci = ei.ListColumns[i];
                if (ci is KeyColumnInfo && ((KeyColumnInfo)ci).AutoIncrement)
                {
                    selectId = " ; select LAST_INSERT_ID();";
                    continue;
                }
                if (ci.MultiLanguage)
                    continue;
                table += ci.ColumnName;
                values += "@" + ci.Property.Name;
                if (i < ei.ListColumns.Count - 1)
                {
                    table += ",";
                    values += ",";
                }
            }
            sql = String.Format(sql, table, values, selectId);
            int result = 0;
            if (selectId.Length > 0)
            {
                object id = ExecuteScalar(sql, entity);
                if (id != null)
                {
                    entity.SetValue(entity.KeysCollection[0].Property, id);
                    result = 1;
                }
            }
            else
            {
                result = ExecuteNonQuery(sql, entity);
            }
            //Update for language table
            if (result > 0 && ei.MultiLanguage)
            {
                InsertLanguageTable<T>(entity);
            }
            return result;
        }
    }
}
