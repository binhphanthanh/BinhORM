using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using QueryBase;
using SystemFramework.Common.Exceptions;
using SystemFramework.Common.ObjectCollectionBase;
using SystemFramework.Common.EntityBase;
using System.Text.RegularExpressions;
using System.Text;

namespace SystemFramework.Common.QueryBase
{
    public enum SqlType
    {
        Normal,
        HasConstructor
    }

    public enum ConnectionStatus
    {
        TransactionStated,
        Connected,
        Available,
        Unknow
    }

    public abstract class DataManager : IDataManager
    {
        protected BaseBusiness _business;
        protected ParameterManager _parameterManager;

        abstract protected ParameterManager CreateParameterManager();

        abstract public int Insert<T>(T entity) where T : BaseEntity;

        protected virtual string PreparePaging(string sql, int startRecord, int maxResult)
        {
            return sql;
        }

        #region Properties

        /// <summary>
        /// Construct tha DataManager and bind a business object to it
        /// </summary>
        /// <param name="business"></param>
        public DataManager(BaseBusiness business)
        {
            this._business = business;
            _parameterManager = CreateParameterManager();
        }

        #endregion

        /// <summary>
        /// Build the condition in where clause. This condition is only contains condition for the id.
        /// This if helpful for update, delete sql. All column name will bw\e preceed by the table name
        /// e.g: Customer.Id = @Id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string BuildIDCondition(Type type, KeyValue id, bool isMultiLingualTable, out object[] parameters)
        {
            string condition = "";
            KeysCollection keys = EntityManager.GetKeysCollection(type);
            EntityInfo ei = EntityManager.GetEntityInfo(type);
            parameters = new object[keys.Count * 2];
            for (int i = 0; i < keys.Count; i++)
            {
                string fieldname = keys[i].ColumnName;
                string propName = keys[i].Property.Name;
                if (condition.Length > 0)
                    condition += " AND ";
                if (isMultiLingualTable)
                {
                    condition += ei.Name + "_Lang.L";
                }
                else
                {
                    condition += ei.Name + ".";
                }
                condition += fieldname + " = @" + propName;
                parameters[i * 2] = fieldname;
                parameters[i * 2 + 1] = id[fieldname];
            }
            return condition;
        }

        /// <summary>
        /// The same as method BuildIDCondition but the column name does not have the table name preceeding.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string BuildIDConditionWithoutObjectName(Type type, KeyValue id, bool isMultiLingualTable, out object[] parameters)
        {
            string condition = "";
            KeysCollection keys = EntityManager.GetKeysCollection(type);
            EntityInfo ei = EntityManager.GetEntityInfo(type);
            parameters = new object[keys.Count * 2];
            for (int i = 0; i < keys.Count; i++)
            {
                string fieldname = keys[i].ColumnName;
                string propName = keys[i].Property.Name;
                if (condition.Length > 0)
                    condition += " AND ";
                if (isMultiLingualTable)
                {
                    condition += "L";
                }
                condition += fieldname + " = @" + propName;
                parameters[i * 2] = fieldname;
                parameters[i * 2 + 1] = id[fieldname];
            }
            return condition;
        }

        /// <summary>
        /// Create a join statement for a sql between main table and language table if they support
        /// multi-lingual.
        /// </summary>
        /// <param name="ei"></param>
        /// <param name="languageKey"></param>
        /// <returns></returns>
        protected string JoinLanguageExpr(EntityInfo ei, string languageKey) 
        {
            string tableLang = ei.Name + "_Lang";
            StringBuilder sb = new StringBuilder();
            sb.Append(ei.Name);
            sb.Append(" left join ");
            sb.Append(tableLang);
            sb.Append(" on ");
            for (int i = 0; i < ei.KeysCollection.Count; i++ )
            {
                KeyColumnInfo key = ei.KeysCollection[i];
                if (i > 0)
                {
                    sb.Append(" AND ");
                }
                sb.Append(ei.Name + "." + key.ColumnName);
                sb.Append("=");
                sb.Append(tableLang + ".L" + key.ColumnName);                
            }
            sb.Append(" AND ");
            sb.Append(tableLang + ".LanguageKey = '" + languageKey + "'");
            return sb.ToString();
        }

        /// <summary>
        /// Get a record by the primary key. The key may be a simple key or a compound key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public T Find<T>(params object[] ids) where T : BaseEntity
        {
            KeyValue id = BaseEntity.CreateKey<T>(ids);
            EntityInfo ei = EntityManager.GetEntityInfo(typeof(T));
            object[] param;
            string condition = BuildIDCondition(typeof(T), id, false, out param);
            string sql = "select * from " + ei.Name + " where " + condition;
            return LoadData<T>(sql, param);
        }

        /// <summary>
        /// Get a record by the primary key with support multi-lingual.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="languageKey"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public T Find<T>(string languageKey, params object[] ids) where T : BaseEntity
        {
            KeyValue id = BaseEntity.CreateKey<T>(ids);
            EntityInfo ei = EntityManager.GetEntityInfo(typeof(T));
            object[] param;
            string condition = BuildIDCondition(typeof(T), id, false, out param);
            string sql = "select * from " + JoinLanguageExpr(ei, languageKey) + " where " + condition;
            return LoadData<T>(sql, param);           
        }

        /// <summary>
        /// Update a record to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update<T>(T entity) where T : BaseEntity
        {
            EntityInfo ei = EntityManager.GetEntityInfo(entity.GetType());
            object[] param;
            string condition = BuildIDCondition(entity.GetType(), entity.KeyValue, false, out param);
            string sql = "update " + ei.Name + " set ";
            KeysCollection keys = ei.KeysCollection;
            for (int i = 0; i < ei.ListColumns.Count; i++)
            {
                ColumnInfo ci = ei.ListColumns[i];
                if (ci is KeyColumnInfo && ((KeyColumnInfo)ci).AutoIncrement || ci.MultiLanguage)
                    continue;

                sql += ci.ColumnName + "= @" + ci.Property.Name;
                if (i < ei.ListColumns.Count - 1)
                {
                    sql += ",";
                }
            }
            sql += " where " + condition;
            //update for main table
            int result = ExecuteNonQuery(sql, entity);
            //update for language table
            if (ei.MultiLanguage)
            {
                if (IsExitsLanguage<T>(entity))
                    UpdateLanguageTable<T>(entity);
                else
                    InsertLanguageTable<T>(entity);
            }
            return result;
        }

        /// <summary>
        /// A helper method for insert/update function. It will update the 
        /// language table to support multi-lingual.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        protected void UpdateLanguageTable<T>(T entity) where T : BaseEntity
        {
            EntityInfo ei = EntityManager.GetEntityInfo(entity.GetType());
            object[] param;
            string condition = BuildIDConditionWithoutObjectName(entity.GetType(), entity.KeyValue, true, out param);
            string sql = "update " + ei.Name + "_Lang set ";
            KeysCollection keys = ei.KeysCollection;
            for (int i = 0; i < ei.ListColumns.Count; i++)
            {
                ColumnInfo ci = ei.ListColumns[i];
                if (ci is KeyColumnInfo)
                {
                    sql += "L" + ci.ColumnName + "= @" + ci.Property.Name + ",";
                }
                else if (ci.MultiLanguage)
                {
                    sql += ci.ColumnName + "= @" + ci.Property.Name + ",";
                }
            }
            sql = sql.Remove(sql.Length - 1, 1);
            sql += " where LanguageKey = @LanguageKey AND " + condition;
            ExecuteNonQuery(sql, entity);
        }

        /// <summary>
        /// A helper method for insert/update function. It will insert into the 
        /// language table to support multi-lingual.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        protected void InsertLanguageTable<T>(T entity) where T : BaseEntity
        {
            EntityInfo ei = EntityManager.GetEntityInfo(entity.GetType());

            string sql = "insert into " + ei.Name + "_Lang ({0}) values({1}) \n" + "{2}";
            string table = "";
            string values = "";
            string selectId = "";
            KeysCollection keys = ei.KeysCollection;
            for (int i = 0; i < ei.ListColumns.Count; i++)
            {
                ColumnInfo ci = ei.ListColumns[i];
                if (ci is KeyColumnInfo)
                {
                    table += "L" + ci.ColumnName + ", ";
                    values += "@" + ci.Property.Name + ", ";
                }
                else if (ci.MultiLanguage)
                {
                    table += ci.ColumnName + ", ";
                    values += "@" + ci.Property.Name + ", ";
                }
            }
            table += " LanguageKey";
            values += " @LanguageKey";
            sql = String.Format(sql, table, values, selectId);
            ExecuteNonQuery(sql, entity);
        }

        /// <summary>
        /// Check the language table whether it contains the content with the language that user provided or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected bool IsExitsLanguage<T>(T entity) where T : BaseEntity
        {
            object[] param;            
            EntityInfo ei = EntityManager.GetEntityInfo(typeof(T));
            string condition = BuildIDConditionWithoutObjectName(typeof(T), entity.KeyValue, true, out param);
            string sql = "select 1 from " + ei.Name + "_Lang where LanguageKey = @LanguageKey AND " + condition;
            object[] newParams = new object[param.Length + 2];
            Array.Copy(param, newParams, param.Length);
            newParams[newParams.Length - 2] = "LanguageKey";
            newParams[newParams.Length - 1] = entity.LanguageKey;
            object value = ExecuteScalar(sql, newParams);
            return value != null;
        }

        /// <summary>
        /// Delete a record by the primary key. The primary can be a simple or compound key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Delete<T>(params object[] ids) where T : BaseEntity
        {
            KeyValue id = BaseEntity.CreateKey<T>(ids);
            return Delete<T>(id);
        }

        /// <summary>
        /// Delete a record by the KeyValue. KeyValue is the instance of all type of keys in this framework.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete<T>(KeyValue id) where T : BaseEntity
        {
            object[] param;
            EntityInfo ei = EntityManager.GetEntityInfo(typeof(T));
            string sql = "delete from " + ei.Name + " where " + BuildIDCondition(typeof(T), id, false, out param);;
            int result = ExecuteNonQuery(sql, param);
            if (ei.MultiLanguage)
            {
                sql = "delete from " + ei.Name + "_Lang where " + BuildIDCondition(typeof(T), id, true, out param); ;
                ExecuteNonQuery(sql, param);
            }
            return result;
        }              
        
        /// <summary>
        /// Execute a sql and use an entity for its parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, object entity)
        {
            try
            {
                return ExecuteNonQuery(_parameterManager.PrepareCommand(sql, entity, _business.Connection));
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return 0;
            }
        }

        /// <summary>
        /// Execute a sql and use an array of parameters
        /// </summary>
        /// <param name="sql">Sql string</param>
        /// <param name="args">Parametes</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, params object[] args)
        {
            try
            {
                return ExecuteNonQuery(_parameterManager.PrepareCommand(sql, _business.Connection, args));
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executes a command and return a number of rows affected. 
        /// This function is usually used for insert and update command 
        /// </summary>
        /// <param name="sqlCommand">The command that will be executed.</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(DbCommand sqlCommand)
        {
            try
            {
                if (_business.Transaction != null)
                {
                    sqlCommand.Transaction = _business.Transaction;
                    return sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    try
                    {
                        _business.Connection.Open();
                        return sqlCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        try
                        {
                            _business.Connection.Close();
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executes a query and returns the first value of the first row in the result set.
        /// </summary>
        /// <param name="sqlCommand">The command that will be executed</param>
        /// <returns>The first value of the first row in the result set.</returns>
        public object ExecuteScalar(DbCommand sqlCommand)
        {
            try
            {
                if (_business.Transaction != null)
                {
                    sqlCommand.Transaction = _business.Transaction;
                    return sqlCommand.ExecuteScalar();
                }
                else
                {
                    try
                    {
                        _business.Connection.Open();
                        return sqlCommand.ExecuteScalar();
                    }
                    finally
                    {
                        try
                        {
                            _business.Connection.Close();
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return 0;
            }
        }

        /// <summary>
        /// Executes a query and returns the first value of the first row in the result set.
        /// </summary>
        /// <param name="sqlCommand">The command that will be executed</param>
        /// <returns>The first value of the first row in the result set.</returns>
        public object ExecuteScalar(string sqlString, params object[] args)
        {
            try
            {
                DbCommand cmd = _parameterManager.PrepareCommand(sqlString, _business.Connection, args);
                return ExecuteScalar(cmd);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return null;
            }
        }

        /// <summary>
        /// Execute a sql and use an entity for its parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object entity)
        {
            try
            {
                return ExecuteScalar(_parameterManager.PrepareCommand(sql, entity, _business.Connection));
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return null;
            }
        }

        /// <summary>
        /// Return a DbDataReader so that consumer can get data their own.
        /// It must be call CompleteReader after read all data
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbCommand sqlCommand)
        {
            try
            {
                if (_business.Transaction != null)
                {
                    sqlCommand.Transaction = _business.Transaction;
                }
                else
                {
                    _business.Connection.Open();
                }
                return sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return null;
            }
        }

        /// <summary>
        /// Called after use ExecuteReader.
        /// </summary>
        /// <param name="reader"></param>
        public void CompleteReader(DbDataReader reader)
        {
            try
            {
                reader.Close();
                if (_business.Transaction == null)
                {
                    _business.Connection.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Update a list of entities use 3 command string insert, delete, update.
        /// If a command string is null, it will ignore that command
        /// </summary>
        /// <param name="insertSql"></param>
        /// <param name="updateSql"></param>
        /// <param name="deleteSql"></param>
        /// <param name="objectList"></param>
        /// <returns></returns>
        private int BatchUpdate(string insertSql, string updateSql, string deleteSql, IList objectList)
        {
            DbCommand insertCommand = _parameterManager.CreateCommand(insertSql, CommandType.Text, _business.Connection);
            DbCommand updateCommand = _parameterManager.CreateCommand(updateSql, CommandType.Text, _business.Connection);
            DbCommand deleteCommand = _parameterManager.CreateCommand(deleteSql, CommandType.Text, _business.Connection);
            return BatchUpdate(insertCommand, updateCommand, deleteCommand, objectList);
        }

        /// <summary>
        /// Update a list of entities use 3 command string insert, delete, update.
        /// If a command string is null, it will ignore that command
        /// </summary>
        /// <param name="objectList"></param>
        /// <returns></returns>
        public int BatchUpdate<T>(IList<T> objectList) where T : BaseEntity
        {
            int affectedRows = 0;
            bool isAutoTransaction = (_business.Transaction == null);
            if (isAutoTransaction)
            {
                _business.BeginTransaction();
            }
            try
            {
                for (int i = 0; i < objectList.Count; i++)
                {
                    BaseEntity entity = objectList[i];
                    if (entity.IsAdded)
                        affectedRows += Insert(entity);
                    else if (entity.IsModified)
                        affectedRows += Update(entity);
                    else if (entity.IsDeleted)
                        affectedRows += Delete<T>(entity.KeyValue);
                }
                if (isAutoTransaction)
                {
                    _business.CommitTransaction();
                }
            }
            catch (Exception)
            {
                if (isAutoTransaction)
                {
                    _business.RollBackTransaction();
                }
                throw;
            }
            return affectedRows;
        }

        /// <summary>
        /// Update a DataSet to Database
        /// </summary>
        /// <param name="insertCommand">The command that inserts the added rows into Database. It can be null.</param>
        /// <param name="updateCommand">The command that updates the modified rows into Database. It can be null.</param>
        /// <param name="deleteCommand">The command that deleted the deleted rows into Database. It can be null.</param>
        /// <param name="objectList">The list of object that is used to update data.</param>
        private int BatchUpdate(DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, IList objectList)
        {
            int affestedRows = 0;
            if (_business.Transaction != null)
            {
                SetTransaction(insertCommand, updateCommand, deleteCommand, _business.Transaction);
                affestedRows = Update(insertCommand, updateCommand, deleteCommand, objectList);
            }
            else
            {
                try
                {
                    _business.BeginTransaction();
                    SetTransaction(insertCommand, updateCommand, deleteCommand, _business.Transaction);
                    affestedRows = Update(insertCommand, updateCommand, deleteCommand, objectList);
                    _business.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _business.RollBackTransaction();
                    ExceptionHandler.Handles(ex);
                }
            }
            return affestedRows;
        }

        /// <summary>
        /// Load Data for one property
        /// </summary>
        /// <param name="selectCommand"></param>
        public T LoadData<T>(DbCommand selectCommand) where T : class
        {
            T entity = default(T);
            DbDataReader reader = null;
            ConnectionStatus status = ConnectionStatus.Unknow;
            try
            {
                //Open connection
                status = BeginLoadData(selectCommand);

                string sql = selectCommand.CommandText;
                SqlType type = NormalizeSql(ref sql);
                selectCommand.CommandText = sql;

                //Begin load Data
                IList<T> objectList = null;
                reader = selectCommand.ExecuteReader();
                if (type == SqlType.Normal)
                {
                    objectList = FillDataByProperty<T>(reader, 1);
                }
                else
                {
                    objectList = FillDataByConstructor<T>(reader, 1);
                }
                if(objectList.Count > 0)
                {
                    entity = objectList[0];
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
            return entity;
        }

        /// <summary>
        /// Get an entity by a sql string and parameters args.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T LoadData<T>(string selectSql, params object[] args) where T : class
        {
            try
            {
                return LoadData<T>(_parameterManager.PrepareCommand(selectSql, _business.Connection, args));
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return default(T);
            }
        }

        public T LoadDataByLanguage<T>(string condition, string languageKey, params object[] args) where T : BaseEntity
        {
            IList<T> list = GetList<T>(condition, languageKey, 0, 1, args);
            if(list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        /// <summary>
        /// Get an entity by a sql string and parameters args.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        virtual public IList<T> LoadListOfData<T>(DbCommand selectCommand, int startRecord, int maxResult) where T:class
        {
            DbDataReader reader = null;
            ConnectionStatus status = ConnectionStatus.Unknow;
            IList<T> objectList = null;
            try
            {
                //Normalize th2 sql
                string sql = selectCommand.CommandText;
                SqlType type = NormalizeSql(ref sql);

                //Paging
                if (startRecord > 0 || maxResult < int.MaxValue)
                {
                    sql = PreparePaging(sql, startRecord, maxResult);
                }
                selectCommand.CommandText = sql;

                //Open connection
                status = BeginLoadData(selectCommand);
                //Begin load Data
                reader = selectCommand.ExecuteReader();
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

        /// <summary>
        /// Load a list of data
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <param name="objectList"></param>
        public IList<T> LoadListOfData<T>(DbCommand selectCommand) where T:class
        {
            return LoadListOfData<T>(selectCommand, 0, int.MaxValue);
        }

        /// <summary>
        /// Load a list of entities a sql string and parameters args.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IList<T> LoadListOfData<T>(string selectSql, params object[] args) where T:class
        {
            try
            {                
                return LoadListOfData<T>(_parameterManager.PrepareCommand(selectSql, _business.Connection, args));
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return null;
            }
        }

        public IList<T> LoadListOfData<T>(string selectSql, int startRecord, int maxResult, params object[] args) where T : class
        {
            try
            {
                return LoadListOfData<T>(_parameterManager.PrepareCommand(selectSql, _business.Connection, args), startRecord, maxResult);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handles(ex);
                return null;
            }
        }

        /// <summary>
        /// Support multilanguage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">The where clause only</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IList<T> GetList<T>(string condition, string languageKey, params object[] args) where T : BaseEntity
        {
            return GetList<T>(condition, languageKey, 0, int.MaxValue, args);
        }

        public IList<T> GetList<T>(string condition, string languageKey, int startRecord, int maxResult, params object[] args) where T : BaseEntity
        {
            EntityInfo ei = EntityManager.GetEntityInfo(typeof(T));
            string sql;
            if (ei.MultiLanguage)
            {
                sql = "select * from " + JoinLanguageExpr(ei, languageKey);
            }
            else
            {
                sql = "select * from " + ei.Name;
            }
            if (!string.IsNullOrEmpty(condition))
            {
                sql += " where " + condition;
            }
            return LoadListOfData<T>(sql, startRecord, maxResult, args);
        }

        #region Internal Methods

        /// <summary>
        /// Begins load a list of data. When this method is called, 
        /// EndLoadData() method must be call after loads data successful.
        /// e.g.
        /// <example>Try
        ///       BeginLoadData(selectCommand)
        /// Catch
        ///       'Handles exceptions
        /// Finally
        ///       EndLoadData()
        /// End Try
        /// </summary>
        /// <param name="selectCommand">The select command that is used to load data</param>
        internal ConnectionStatus BeginLoadData(DbCommand selectCommand)
        {
            if (_business.Transaction != null)
            {
                selectCommand.Transaction = _business.Transaction;
                return ConnectionStatus.TransactionStated;
            }
            else if (_business.Connection.State == ConnectionState.Open)
            {
                return ConnectionStatus.Connected;
            }
            else
            {
                _business.Connection.Open();
                return ConnectionStatus.Available;
            }
        }

        /// <summary>
        /// End load data is used to close the connection (if it is opened by the BeginLoadData())
        /// </summary>
        internal void EndLoadData(ConnectionStatus status)
        {
            if (status == ConnectionStatus.Available)
            {
                try
                {
                    _business.Connection.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Private Methods

        protected SqlType NormalizeSql(ref string sql)
        {
            Regex rx = new Regex(@"\s+new\s+(.|[\n\r])+\((.|[\n\r])+\)\s*");
            Match m = rx.Match(sql);
            if (m != null && m.Success)
            {
                int newIdx = sql.IndexOf(" new ");
                int leftBracketIdx = sql.IndexOf('(', newIdx);
                sql = sql.Remove(newIdx + 1, leftBracketIdx - newIdx);

                int rightBracketIdx = sql.IndexOf(')', newIdx);
                sql = sql.Remove(rightBracketIdx, 1);
                return SqlType.HasConstructor;
            }
            return SqlType.Normal;
        }

        /// <summary>
        /// Read data from a reader and set to entity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="setter"></param>
        /// <param name="entity"></param>
        /// <param name="columnsCount"></param>
        protected void SetDatatToEntity(DbDataReader reader, IPropertySetter setter, object entity)
        {
            int columnsCount = setter.ColumnCount;
            for (int i = 0; i < columnsCount; i++)
            {
                string propName = setter.GetColumnName(i);
                object value = reader[i];
                if (value == DBNull.Value)
                {
                    setter.SetValue(propName, entity, null);
                }
                else
                {
                    //if (reader.GetDataTypeName(i).ToLower().Equals("image"))
                    //    value = DataHelper.ByteArrayToImage(value as byte[]);
                    setter.SetValue(propName, entity, value);
                }
            }
        }

        protected IList<T> FillDataByProperty<T>(DbDataReader reader, int maxResult) where T : class
        {
            int count = 0;
            Type entityType = typeof(T);
            ConstructorInfo constructor = entityType.GetConstructor(new Type[] { });
            QueryPropertySetter setter = new QueryPropertySetter(reader, entityType);
            IList<T> objectsList = new List<T>();
            if (entityType.IsSubclassOf(typeof(BaseEntity)))
            {
                while (count++ < maxResult && reader.Read())
                {
                    BaseEntity entity = (BaseEntity)constructor.Invoke(null);
                    entity.BeginEdit();
                    SetDatatToEntity(reader, setter, entity);
                    entity.EndEdit();
                    entity.AcceptChanges();
                    objectsList.Add(entity as T);
                }
            }
            else
            {
                while (count++ < maxResult && reader.Read())
                {
                    T entity = (T)constructor.Invoke(null);
                    SetDatatToEntity(reader, setter, entity);
                    objectsList.Add(entity);
                }
            }
            return objectsList;
        }

        protected IList<T> FillDataByConstructor<T>(DbDataReader reader, int maxResult) where T : class
        {
            int count = 0;
            Type entityType = typeof(T);
            ConstructorInfo constructor = GetContructor(entityType, reader);
            IList<T> objectsList = new List<T>();
            while (count++ < maxResult && reader.Read())
            {
                T entity = CreateEntity<T>(reader, constructor);
                objectsList.Add(entity);
            }
            return objectsList;
        }

        /// <summary>
        /// Read data from a reader and set to entity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="setter"></param>
        /// <param name="entity"></param>
        /// <param name="columnsCount"></param>
        private T CreateEntity<T>(DbDataReader reader, ConstructorInfo constructor) where T:class
        {
            object[] data = new object[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object value = reader[i];
                if (value == DBNull.Value)
                {
                    data[i] = null;
                }
                else
                {
                    if (reader.GetDataTypeName(i).ToLower().Equals("image"))
                        value = DataHelper.ByteArrayToImage(value as byte[]);
                    data[i] = value;
                }
            }
            return constructor.Invoke(data) as T;
        }

        private ConstructorInfo GetContructor(Type entityType, DbDataReader reader)
        {
            Type[] types = new Type[reader.FieldCount];            
            for(int i=0; i<reader.FieldCount; i++)
            {
                types[i] = reader.GetFieldType(i);
            }
            ConstructorInfo constructor = entityType.GetConstructor(types);
            if (constructor != null)
            {
                return constructor;                
            }
            else
            {
                throw new Exception("No such cotructor!");
            }
        }

        /// <summary>
        /// Update a DataSet to Database
        /// </summary>
        /// <param name="insertCommand">The command that inserts the added rows into Database. It can be null.</param>
        /// <param name="updateCommand">The command that updates the modified rows into Database. It can be null.</param>
        /// <param name="deleteCommand">The command that deleted the deleted rows into Database. It can be null.</param>
        /// <param name="objectList">The list of entities that is used to update data.</param>
        private int Update(DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, IList objectList)
        {
            int affectedRows = 0;
            _parameterManager.BuildCommandParameters(insertCommand);
            _parameterManager.BuildCommandParameters(updateCommand);
            _parameterManager.BuildCommandParameters(deleteCommand);
            foreach (BaseEntity entity in objectList)
            {
                if (entity.EntityState == EntityState.Added)
                {
                    if (insertCommand != null)
                    {
                        affectedRows += ExecuteUpdate(insertCommand, entity);
                    }
                }
                else if (entity.EntityState == EntityState.Modified)
                {
                    if (updateCommand != null)
                    {
                        affectedRows += ExecuteUpdate(updateCommand, entity);
                    }
                }
                else if (entity.EntityState == EntityState.Deleted)
                {
                    if (deleteCommand != null)
                    {
                        affectedRows += ExecuteUpdate(deleteCommand, entity);
                    }
                }
            }
            return affectedRows;
        }
        
        /// <summary>
        /// Set transation for commands
        /// </summary>
        /// <param name="insertCommand">The command that inserts the added rows into Database. It can be null.</param>
        /// <param name="updateCommand">The command that updates the modified rows into Database. It can be null.</param>
        /// <param name="deleteCommand">The command that deleted the deleted rows into Database. It can be null.</param>
        /// <param name="transaction">The transaction that want to set to the commands</param>
        private void SetTransaction(DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, DbTransaction transaction)
        {
            if (insertCommand != null)
            {
                insertCommand.Transaction = transaction;
            }
            if (updateCommand != null)
            {
                updateCommand.Transaction = transaction;
            }
            if (deleteCommand != null)
            {
                deleteCommand.Transaction = transaction;
            }
        }

        /// <summary>
        /// Updates or inserts a row depend on the RowState of the row
        /// </summary>
        /// <param name="command">The update or insert command </param>
        /// <param name="entity">The row contains data.</param>
        private int ExecuteUpdate(DbCommand command, BaseEntity entity)
        {
            _parameterManager.SetParameterValues(command, entity);
            return command.ExecuteNonQuery();
        }

        #endregion
    }
}