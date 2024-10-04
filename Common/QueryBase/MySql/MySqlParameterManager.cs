using System.Data.Common;
using MySql.Data.MySqlClient;
using SystemFramework.Common.QueryBase.Sql;

namespace SystemFramework.Common.QueryBase.MySql
{
    public class MySqlParameterManager : SqlParameterManager
    {
        override public DbCommand CreateDbCommand()
        {
            return new MySqlCommand();
        }

        override public DbParameter CreateDbParameter()
        {
            return new MySqlParameter();
        }
    }
}
