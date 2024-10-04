
using System;
using System.Data;
using System.Data.Common;

namespace SystemFramework.Common
{
    public abstract class BaseBusiness
    {
       // private DataManager _dataManager;
        private DbConnection _connection;
        private DbTransaction _transaction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseBusiness()
        {
            _connection = ConnectionManager.GetConnection();
        }

        /// <summary>
        /// Gets the DataManager of this business component. This DataManager is used in the whole data access component of this business component.
        /// </summary>
        internal DbConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        internal DbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
        }

        /// <summary>
        /// This function is used for load a large amount of data those associate with many DataAccess classes
        /// </summary>
        public void OptimizeForLoadData()
        {
            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method is called when data is loaded and after call OptimizeForLoadData().
        /// </summary>
        public void CompleteLoadData()
        {
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Commits a transaction if all the data operation is successfully.
        /// </summary>
        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Commit();
                    _connection.Close();
                    _transaction = null;
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Rolls back all data if there is at least one operation fail.
        /// </summary>
        public void RollBackTransaction()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                    _connection.Close();
                    _transaction = null;
                }
                catch (Exception)
                {
                }
            }
        }

        public void Join(BaseBusiness joinBC)
        {
            joinBC._connection = this._connection;
        }

        public void UnJoin(BaseBusiness joinBC)
        {
            joinBC._connection = ConnectionManager.GetConnection();
        }
    }
}
