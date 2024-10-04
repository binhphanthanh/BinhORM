using System.Collections.Generic;
using System.Data.Common;
using SystemFramework.Common;
using SystemFramework.Common.ObjectCollectionBase;

namespace QueryBase
{
    public interface IDataManager
    {
        int BatchUpdate<T>(IList<T> objectList) where T : BaseEntity;

        int Insert<T>(T entity) where T : BaseEntity;
        int Update<T>(T entity) where T : BaseEntity;
        int Delete<T>(KeyValue id) where T : BaseEntity;
        int Delete<T>(params object[] ids) where T : BaseEntity;

        int ExecuteNonQuery(string sql, object entity);
        int ExecuteNonQuery(string sql, params object[] args);

        DbDataReader ExecuteReader(DbCommand sqlCommand);
        void CompleteReader(DbDataReader reader);

        object ExecuteScalar(string sql, object entity);
        object ExecuteScalar(string sqlString, params object[] args);

        T Find<T>(params object[] ids) where T : BaseEntity;
        T Find<T>(string languageKey, params object[] ids) where T : BaseEntity;
        T LoadData<T>(string selectSql, params object[] args) where T : class;
        T LoadDataByLanguage<T>(string condition, string languageKey, params object[] args) where T : BaseEntity;

        IList<T> LoadListOfData<T>(string selectSql, int startRecord, int maxResult, params object[] args) where T : class;
        IList<T> LoadListOfData<T>(string selectSql, params object[] args) where T : class;
        IList<T> GetList<T>(string condition, string languageKey, params object[] args) where T : BaseEntity;
        IList<T> GetList<T>(string condition, string languageKey, int startRecord, int maxResult, params object[] args) where T : BaseEntity;
    }
}
