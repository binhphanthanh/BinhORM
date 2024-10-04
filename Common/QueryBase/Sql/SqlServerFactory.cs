using System.Data.Common;
using System.Data.SqlClient;
using QueryBase;
using SystemFramework.Common.QueryBase.Sql;

namespace SystemFramework.Common.QueryBase.Sql
{
    public class SqlServerFactory : IDbFactory
    {

        public DbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        public DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public DbParameter CreateParameter()
        {
            return new SqlParameter();
        }

        public IDataManager CreateDataManager(BaseBusiness business)
        {
            return new SqlDataManager(business);
        }

    }
}
