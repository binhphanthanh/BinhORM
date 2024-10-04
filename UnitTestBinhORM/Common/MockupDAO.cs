using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using SystemFramework.Common;
using System.Data.Common;
using SystemFramework.Common.ObjectCollectionBase;
using Common;
using System.Configuration;
using System.Data.SqlClient;

namespace Common
{
    public class MockupDAO : BaseDataAccess
    {
        public MockupDAO(BaseBusiness baseBusiness)
            : base(baseBusiness)
        {
        }

        new public int Insert<T>(T entity) where T : BaseEntity
        {
            return base.Insert<T>(entity);
        }

        new public int Update<T>(T entity) where T : BaseEntity
        {
            return base.Update<T>(entity);
        }

        new public int Delete<T>(KeyValue id) where T : BaseEntity
        {
            return base.Delete<T>(id);
        }

        new public int Delete<T>(params object[] ids) where T : BaseEntity
        {
            return base.Delete<T>(ids);
        }

        new public int ExecuteNonQuery(string sql, object entity)
        {
            return base.ExecuteNonQuery(sql, entity);
        }

        new public int ExecuteNonQuery(string sql, params object[] args)
        {
            return base.ExecuteNonQuery(sql, args);
        }

        new public DbDataReader ExecuteReader(DbCommand sqlCommand)
        {
            return base.ExecuteReader(sqlCommand);
        }

        new public void CompleteReader(DbDataReader reader)
        {
            base.CompleteReader(reader);
        }

        new public object ExecuteScalar(string sql, object entity)
        {
            return base.ExecuteScalar(sql, entity);
        }

        new public object ExecuteScalar(string sqlString, params object[] args)
        {
            return base.ExecuteScalar(sqlString, args);
        }

        new public T Find<T>(params object[] ids) where T : BaseEntity
        {
            return base.Find<T>(ids);
        }

        new public T FindByLanguage<T>(string languageKey, params object[] ids) where T : BaseEntity
        {
            return base.FindByLanguage<T>(languageKey, ids);
        }

        new public T LoadData<T>(string selectSql, params object[] args) where T : class
        {
            return base.LoadData<T>(selectSql, args);
        }

        new public IList<T> GetList<T>(string selectSql, int startRecord, int maxResult, params object[] args) where T : class
        {
            return base.GetList<T>(selectSql, startRecord, maxResult, args);
        }

        new public IList<T> GetList<T>(string selectSql, params object[] args) where T : class
        {
            return base.GetList<T>(selectSql, args);
        }

        new public IList<T> GetListByLanguage<T>(string condition, string languageKey, params object[] args) where T : BaseEntity
        {
            return base.GetListByLanguage<T>(condition, languageKey, args);
        }

        new public IList<T> GetListByLanguage<T>(string condition, string languageKey, int startRecord, int maxResult, params object[] args) where T : BaseEntity
        {
            return base.GetListByLanguage<T>(condition, languageKey, startRecord, maxResult, args);
        }

        new public int BatchUpdate<T>(IList<T> objectList) where T : BaseEntity
        {
            return base.BatchUpdate<T>(objectList);
        }
    }
}
