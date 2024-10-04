using System.Data.SqlClient;
using SystemFramework.Common.QueryBase;
using SystemFramework.Common.QueryBase.Sql;
using SystemFramework.Common.QueryBase.OleDb;
using System.Data.Common;
using System.Collections.Generic;
using QueryBase;
using SystemFramework.Common.ObjectCollectionBase;
using System.Collections;
using SystemFramework.Common.QueryBase.MySql;

namespace SystemFramework.Common
{
    /// <summary>
    /// This is the base class of all DataAccessComponent.
    /// </summary>
    public class BaseDataAccess
    {

        private IDataManager _dataManager;
        /// <summary>
        /// Constructor with its Business component.
        /// </summary>
        /// <param name="baseBusiness">The Business component that contains this DataAccess component.</param>
        public BaseDataAccess(BaseBusiness baseBusiness)
        {
            _dataManager = FactoryManager.Factory.CreateDataManager(baseBusiness);      
        }

        protected int Insert<T>(T entity) where T : BaseEntity
        {
            return _dataManager.Insert<T>(entity);
        }

        protected int Update<T>(T entity) where T : BaseEntity
        {
            return _dataManager.Update<T>(entity);
        }

        protected int Delete<T>(KeyValue id) where T : BaseEntity
        {
            return _dataManager.Delete<T>(id);
        }

        protected int Delete<T>(params object[] ids) where T : BaseEntity
        {
            return _dataManager.Delete<T>(ids);
        }

        protected int ExecuteNonQuery(string sql, object entity)
        {
            return _dataManager.ExecuteNonQuery(sql, entity);
        }

        protected int ExecuteNonQuery(string sql, params object[] args)
        {
            return _dataManager.ExecuteNonQuery(sql, args);
        }

        protected DbDataReader ExecuteReader(DbCommand sqlCommand)
        {
            return _dataManager.ExecuteReader(sqlCommand);
        }

        protected void CompleteReader(DbDataReader reader)
        {
            _dataManager.CompleteReader(reader);
        }

        protected object ExecuteScalar(string sql, object entity)
        {
            return _dataManager.ExecuteScalar(sql, entity);
        }

        protected object ExecuteScalar(string sqlString, params object[] args)
        {
            return _dataManager.ExecuteScalar(sqlString, args);
        }

        protected T Find<T>(params object[] ids) where T : BaseEntity
        {
            return _dataManager.Find<T>(ids);
        }

        public T FindByLanguage<T>(string languageKey, params object[] ids) where T : BaseEntity
        {
            return _dataManager.Find<T>(languageKey, ids);
        }
        protected T LoadData<T>(string selectSql, params object[] args) where T : class
        {
            return _dataManager.LoadData<T>(selectSql, args);
        }

        protected T LoadDataByLanguage<T>(string condition, string languageKey, params object[] args) where T : BaseEntity
        {
            return _dataManager.LoadDataByLanguage<T>(condition, languageKey, args);
        }
        
        protected IList<T> GetList<T>(string selectSql, int startRecord, int maxResult, params object[] args) where T : class
        {
            return _dataManager.LoadListOfData<T>(selectSql, startRecord, maxResult, args);
        }

        protected IList<T> GetList<T>(string selectSql, params object[] args) where T : class
        {
            return _dataManager.LoadListOfData<T>(selectSql, args);
        }

        public IList<T> GetListByLanguage<T>(string condition, string languageKey, params object[] args) where T : BaseEntity
        {
            return _dataManager.GetList<T>(condition, languageKey, args);
        }

        public IList<T> GetListByLanguage<T>(string condition, string languageKey, int startRecord, int maxResult, params object[] args) where T : BaseEntity
        {
            return _dataManager.GetList<T>(condition, languageKey, startRecord, maxResult, args);
        }

        protected int BatchUpdate<T>(IList<T> objectList) where T : BaseEntity
        {
            return _dataManager.BatchUpdate<T>(objectList);
        }
    }
}
