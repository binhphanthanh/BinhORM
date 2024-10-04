using System;
using SystemFramework.Common.EntityBase;
using SystemFramework.Common.ObjectCollectionBase;
using System.Data.Common;
using System.Collections.Generic;
using SystemFramework.Common.Exceptions;

namespace SystemFramework.Common.QueryBase.OleDb
{
    public class OleDbDataManager : DataManager
    {
        public OleDbDataManager(BaseBusiness baseBusiness)
            : base(baseBusiness)
        {
        }

        protected override ParameterManager CreateParameterManager()
        {
            return new OleDbParameterManager();
        }

        public override int Insert<T>(T entity)
        {
            EntityInfo ei = EntityManager.GetEntityInfo(entity.GetType());

            string sql = "insert into " + ei.Name + "({0}) values({1})";
            string table = "";
            string values = "";
            string selectId = "";
            KeysCollection keys = ei.KeysCollection;
            for (int i = 0; i < ei.ListColumns.Count; i++)
            {
                ColumnInfo ci = ei.ListColumns[i];
                if (ci is KeyColumnInfo && ((KeyColumnInfo)ci).AutoIncrement)
                {
                    selectId = String.Format("SELECT max({0}) FROM {1}", ci.ColumnName, ei.Name);
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
            sql = String.Format(sql, table, values);
            int retVal =  ExecuteNonQuery(sql, entity);            
            if (selectId.Length > 0)
            {
                object id = ExecuteScalar(selectId);
                if (id != null)
                {
                    entity.SetValue(entity.KeysCollection[0].Property, id);                   
                }
            }
            //Update for language table
            if (retVal > 0 && ei.MultiLanguage)
            {
                InsertLanguageTable<T>(entity);
            }
            return retVal;
        }

        public override System.Collections.Generic.IList<T> LoadListOfData<T>(System.Data.Common.DbCommand selectCommand, int startRecord, int maxResult)
        {
            DbDataReader reader = null;
            ConnectionStatus status = ConnectionStatus.Unknow;
            IList<T> objectList = null;
            try
            {
                //Normalize th2 sql
                string sql = selectCommand.CommandText;
                SqlType type = NormalizeSql(ref sql);
                selectCommand.CommandText = sql;

                //Open connection
                status = BeginLoadData(selectCommand);
                //Begin load Data
                reader = selectCommand.ExecuteReader();
                int index = -1;
                while (++index < startRecord)
                {
                    reader.Read();
                }
                if (type == SqlType.Normal)
                {
                    objectList = FillDataByProperty<T>(reader, maxResult);
                }
                else
                {
                    objectList = FillDataByConstructor<T>(reader, maxResult);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Handles(e);
            }
            finally
            {
                EndLoadData(status);
                try
                {
                    reader.Close();
                }
                catch (Exception) { }
            }
            return objectList;
        }
    }
}
