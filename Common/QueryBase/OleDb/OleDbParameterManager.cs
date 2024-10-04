using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Data;

namespace SystemFramework.Common.QueryBase.OleDb
{
    public class OleDbParameterManager : ParameterManager
    {
        public override DbCommand CreateDbCommand()
        {
            return new OleDbCommand();
        }

        public override DbParameter CreateDbParameter()
        {
            return new OleDbParameter();
        }

        public override void BuildCommandParameters(DbCommand command)
        {
            string pattern = @"@[a-zA-Z0-9_]+";
            if (command == null || command.Parameters.Count > 0)
                return;
            if (command.CommandType == CommandType.Text)
            {
                string sql = command.CommandText;
                Regex reg = new Regex(pattern);
                MatchCollection matches = reg.Matches(sql);
                foreach (Match match in matches)
                {
                    CaptureCollection captures = match.Captures;
                    foreach (Capture capture in captures)
                    {
                        string paramName = capture.Value.Trim();
                        DbParameter param = CreateDbParameter();
                        param.ParameterName = paramName;
                        command.Parameters.Add(param);
                    }
                }
                command.CommandText = Regex.Replace(sql, pattern, "?"); 
            }
        }
    }
}
