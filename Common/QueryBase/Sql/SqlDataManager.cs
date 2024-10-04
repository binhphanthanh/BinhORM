using System;
using SystemFramework.Common.EntityBase;
using SystemFramework.Common.ObjectCollectionBase;

namespace SystemFramework.Common.QueryBase.Sql
{
    public class SqlDataManager : DataManager
    {
        public SqlDataManager(BaseBusiness baseBusiness) 
            : base(baseBusiness)
        {
            
        }

        override protected string PreparePaging(string sql, int startRecord, int maxResult)
        {
            int startIndex = sql.IndexOf("select", StringComparison.OrdinalIgnoreCase) + 6;
            if (startIndex < 0) throw new Exception("No SELECT statement in the query");

            int endIndex = sql.IndexOf("from", StringComparison.OrdinalIgnoreCase);
            if (endIndex < 0) throw new Exception("No FROM statement in the query");

            string columns = sql.Substring(startIndex, endIndex - startIndex);

            string orders = "";
            int orderIndex = sql.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
            if (orderIndex > 0)
            {
                orders = sql.Substring(orderIndex);
                sql = sql.Substring(0, orderIndex);
            }
            else
            {
                orders = "order by (select null)";
            }
            string newSql =
                   @"SELECT {0} 
                     FROM ( SELECT  ROW_NUMBER() OVER ({1}) as row, t.* 
                            FROM ( {2} ) t) as aaa_table
                     WHERE row > {3} and row <= {4}";
            return string.Format(newSql, columns, orders, sql, startRecord, maxResult + startRecord);
        }

        protected override ParameterManager CreateParameterManager()
        {
            return new SqlParameterManager();
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
                    selectId = "select Scope_Identity()";
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
                //Insert for auto inscreasement key
                object id = ExecuteScalar(sql, entity);
                if (id != null)
                {
                    entity.SetValue(entity.KeysCollection[0].Property, Decimal.ToInt32((Decimal)id));
                    result = 1;
                }
            }
            else
            {
                //Insert for normal key
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
