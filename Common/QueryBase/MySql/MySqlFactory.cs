using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using QueryBase;

namespace SystemFramework.Common.QueryBase.MySql
{
    public class MySqlFactory : IDbFactory
    {

        public DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        public DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        public DbParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        public IDataManager CreateDataManager(BaseBusiness business)
        {
            return new MySqlDataManager(business);
        }

    }
}
