using System.Data.Common;
using System.Data.OleDb;
using QueryBase;
using SystemFramework.Common.QueryBase.OleDb;

namespace SystemFramework.Common.QueryBase.OleDb
{
    public class OleDbFactory : IDbFactory
    {

        public DbConnection CreateConnection()
        {
            return new OleDbConnection();
        }

        public DbCommand CreateCommand()
        {
            return new OleDbCommand();
        }

        public DbParameter CreateParameter()
        {
            return new OleDbParameter();
        }

        public IDataManager CreateDataManager(BaseBusiness business)
        {
            return new OleDbDataManager(business);
        }

    }
}
