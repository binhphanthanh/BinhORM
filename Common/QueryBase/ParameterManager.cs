using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Reflection;
using SystemFramework.Common.Exceptions;
using System.Text.RegularExpressions;

namespace SystemFramework.Common.QueryBase
{
    public abstract class ParameterManager
    {
       
        /// <summary>
        /// Create a new DbCommand instance.
        /// </summary>
        /// <returns></returns>
        abstract public DbCommand CreateDbCommand();
        abstract public DbParameter CreateDbParameter();

        /// <summary>
        /// Add all parameters for a DbCommand automatically, but they don't have values.
        /// </summary>
        /// <param name="command"></param>
        abstract public void BuildCommandParameters(DbCommand command);

        /// <summary>
        /// Adds a standard input SqlParameter to the command
        /// </summary>
        /// <param name="command">The command want to add the parameter.</param>
        /// <param name="paramName">The name of parameter, e.g. "@CustomerId".</param>
        /// <param name="sqlDbType">The data type of the parameter</param>
        /// <param name="size">The size of this data type.</param>
        public DbParameter AddParameter(DbCommand command, string paramName, DbType sqlDbType, int size)
        {
            DbParameter param = CreateDbParameter();
            param.ParameterName = paramName;
            param.DbType = sqlDbType;
            param.Size = size;
            param.Direction = ParameterDirection.Input;
            command.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// Adds a standard DbParameter to the command
        /// </summary>
        /// <param name="command">The command want to add the parameter.</param>
        /// <param name="paramName">The name of parameter, e.g. "@CustomerId".</param>
        /// <param name="sqlDbType">The data type of the parameter</param>
        /// <param name="size">The size of this data type.</param>
        /// <param name="direction">The direction of the parameter</param>
        public DbParameter AddParameter(DbCommand command, string paramName, DbType sqlDbType, int size, ParameterDirection direction)
        {
            DbParameter param = CreateDbParameter();
            param.ParameterName = paramName;
            param.DbType = sqlDbType;
            param.Size = size;
            param.Direction = direction;
            command.Parameters.Add(param);
            return param;
        }

        /// <summary>
        ///  Adds a standard DbParameter to the command. It is usually used for BatchUpdate function.
        /// </summary>
        /// <param name="command">The command want to add the parameter.</param>
        /// <param name="paramName">The name of parameter, e.g. "@CustomerId".</param>
        /// <param name="sqlDbType">The data type of the parameter</param>
        /// <param name="size">The size of this data type.</param>
        /// <param name="sourceColumn">The name of the column that will assign its value to this parameter.</param>
        public DbParameter AddParameter(DbCommand command, string paramName, DbType sqlDbType, int size, string sourceColumn)
        {
            DbParameter param = CreateDbParameter();
            param.ParameterName = paramName;
            param.DbType = sqlDbType;
            param.Size = size;
            param.SourceColumn = sourceColumn;
            command.Parameters.Add(param);
            return param;

        }

        /// <summary>
        ///  Adds a parameter with its value
        /// </summary>
        /// <param name="command">The command want to add the parameter.</param>
        /// <param name="paramName">The name of parameter, e.g. "@CustomerId".</param>
        /// <param name="value">The value of this command.</param>
        public DbParameter AddParameter(DbCommand command, string paramName, object value)
        {
            DbParameter param = CreateDbParameter();
            param.ParameterName = paramName;
            SetParameterValue(param, value);
            command.Parameters.Add(param);
            return param;
        }

        /// <summary>
        ///  Adds a list of parameter with its value
        /// </summary>
        /// <param name="command">The command want to add the parameter.</param>
        /// <param name="paramName">The name of parameter, e.g. "@CustomerId".</param>
        /// <param name="value">The value of this command.</param>
        public void AddParameter(DbCommand command, params object[] args)
        {
            if(args.Length < 0 || args.Length % 2 != 0)
                throw new Exception("Bạn phải truyền 'tên param, giá trị, tên param, giá trị ....'");

            for(int i=0; i < args.Length; i+=2)
            {
                DbParameter param = CreateDbParameter();
                param.ParameterName = args[i] as string;
                SetParameterValue(param, args[i+1]);
                command.Parameters.Add(param);
            }
        }

        /// <summary>
        /// Set values for all parameters in the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="entity"></param>
        public void SetParameterValues(DbCommand command, object entity)
        {
            Type type = entity.GetType();
            foreach (DbParameter param in command.Parameters)
            {
                string propName = param.ParameterName;
                if (propName.StartsWith("@"))
                    propName = propName.Substring(1);

                PropertyInfo propertyInfo = type.GetProperty(propName, BindingFlags.IgnoreCase
                                                                       | BindingFlags.Public
                                                                       | BindingFlags.Instance);
                if (propertyInfo == null)
                    throw new ParamPropertyNotFoundException(propName);

                object value = propertyInfo.GetValue(entity, null);
                SetParameterValue(param, value);
            }
        }

        /// <summary>
        /// Set a parameter for a DbCommand
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        public void SetParameterValue(DbParameter param, object value)
        {
            if (value == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                if (value is Image)
                    value = DataHelper.ImageToByteArray((value as Image));
                if (value is DateTime)
                    param.DbType = DbType.DateTime;
                else if (value is byte[])
                    param.DbType = DbType.Binary;
                param.Value = value;
            }
        }

        /// <summary>
        /// Creates a command. All commands created from this DataManager can be used in one Transaction 
        /// </summary>
        /// <param name="sql">The query string or the stored procedure name or the table name depend on the commandType</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>Return a SqlComman</returns>
        public DbCommand CreateCommand(string sql, CommandType commandType, DbConnection connection)
        {
            if (String.IsNullOrEmpty(sql))
                return null;

            DbCommand command = CreateDbCommand();
            command.CommandText = sql;
            command.CommandType = commandType;
            command.Connection = connection;
            return command;
        }

        /// <summary>
        /// Create a command and set all parameters for it.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DbCommand PrepareCommand(string sql, object entity, DbConnection connection)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            DbCommand command = CreateCommand(sql, CommandType.Text, connection);
            BuildCommandParameters(command);
            SetParameterValues(command, entity);
            return command;
        }

        /// <summary>
        /// Create a SqlCommand from an sql string and set parameters for it
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DbCommand PrepareCommand(string sql, DbConnection connection, params object[] args)
        {
            if (args.Length % 2 != 0)
                throw new Exception("Bạn phải truyền 'tên param, giá trị, tên param, giá trị ....' vào hàm ExecuteNonQuery");

            
            DbCommand command = CreateCommand(sql, CommandType.Text, connection);
            for (int i = 0; i < args.Length; i += 2)
            {
                AddParameter(command, args[i].ToString(), args[i + 1]);
            }
            return command;
        }
    }
}
