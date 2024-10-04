using System.Data.SqlClient;
using System;
using System.IO;
using System.Data.OleDb;
using System.Data.Common;
using System.Configuration;
using SystemFramework.Properties;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace SystemFramework.Common
{
    public class ConnectionManager
    {
        public const string SqlServer = "System.Data.SqlClient";
        public const string OleDb = "System.Data.OleDb";
        public const string MySql = "MySql.Data.MySqlClient";

        public static string Server = @".\sqlexpress";
        public static string UserName = "sa";
        public static string Password = "binh";
        private static String connStr = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
        public static String provider = ConfigurationManager.ConnectionStrings["connection"].ProviderName;

        public static DbConnection GetConnection()
        {
            DbConnection conn =  FactoryManager.Factory.CreateConnection();
            conn.ConnectionString = connStr;
            return conn;
        }

        public static String ConnectionString
        {
            get { return connStr; }
            set { connStr = value; }
        }
    }

    public enum Provider
    {
       SqlServer = 1, OleDb = 2, MySql = 3
    }
}
