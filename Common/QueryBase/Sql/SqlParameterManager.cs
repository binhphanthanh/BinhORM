using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace SystemFramework.Common.QueryBase.Sql
{
    public class SqlParameterManager : ParameterManager
    {

        override public DbCommand CreateDbCommand()
        {
            return new SqlCommand();
        }

        override public DbParameter CreateDbParameter()
        {
            return new SqlParameter();
        }

        override public void BuildCommandParameters(DbCommand command)
        {
            if (command == null || command.Parameters.Count > 0)
                return;
            if (command.CommandType == CommandType.Text)
            {
                string sql = command.CommandText;
                Regex reg = new Regex(@"@[a-zA-Z0-9_]+");
                MatchCollection matches = reg.Matches(sql);
                foreach (Match match in matches)
                {
                    CaptureCollection captures = match.Captures;
                    foreach (Capture capture in captures)
                    {
                        string paramName = capture.Value.Trim();
                        if (!command.Parameters.Contains(paramName))
                        {
                            DbParameter param = CreateDbParameter();
                            param.ParameterName = paramName;
                            command.Parameters.Add(param);
                        }
                    }
                }
            }
        }
    }
}
