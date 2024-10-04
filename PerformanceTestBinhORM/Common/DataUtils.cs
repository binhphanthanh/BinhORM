using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Common;

namespace TestBinhORM.Common
{
    public class DataUtils
    {
        public static Customer2 ADOGetCustomer2(long id)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter("select * from Customer_1 where id = " + id, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow r = ds.Tables[0].Rows[0];
                Customer2 c = new Customer2();
                c.Id = (int)r["id"];
                c.Name = r["name"] as string;
                c.Address = r["address"] as string;
                c.Deleted = (bool)r["deleted"];

                return c;
            }
            return null;
        }

        public static void ADODeleteCustomer2(long id)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            try
            {
                SqlCommand delete = new SqlCommand("delete from Customer_1 where id = " + id, conn);
                conn.Open();
                delete.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public static void ADOInsertCustomer2(Customer2 c)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            try
            {
                SqlCommand insert = new SqlCommand(@"insert into Customer_1(name, address, phone,money,deleted) 
                                                    values(@name, @address, @phone, @money, @deleted)
                                                    select Scope_Identity()", conn);
                insert.Parameters.AddWithValue("@name", c.Name);
                insert.Parameters.AddWithValue("@address", c.Address);
                insert.Parameters.AddWithValue("@phone", c.Phone);
                insert.Parameters.AddWithValue("@money", c.Money);
                insert.Parameters.AddWithValue("@deleted", c.Deleted);

                conn.Open();
                int id = Convert.ToInt32(insert.ExecuteScalar());
                c.Id = id;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public static object ADOExecuteScalar(string sql, params object[] args)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            try
            {
                SqlCommand insert = new SqlCommand(sql, conn);
                for (int i = 0; i < args.Length; i += 2)
                {
                    insert.Parameters.AddWithValue((string)args[i], args[i + 1]);
                }
                conn.Open();
                return insert.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public static int ADOExecuteNoneQuery(string sql, params object[] args)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            try
            {
                SqlCommand insert = new SqlCommand(sql, conn);
                for (int i = 0; i < args.Length; i += 2)
                {
                    insert.Parameters.AddWithValue((string)args[i], args[i + 1]);
                }
                conn.Open();
                return insert.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }


        public static Customer ADOGetCustomer(long id, string language)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter(@"select * from Customer c inner join Customer_Lang l on c.id = l.id 
                                                    where c.id = " + id + " AND LanguageKey = '" + language + "'", conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow r = ds.Tables[0].Rows[0];
                Customer c = new Customer();
                c.Id = (int)r["id"];
                c.Name = r["name"] as string;
                c.Address = r["address"] as string;
                c.Deleted = (bool)r["deleted"];
                c.LanguageKey = r["LanguageKey"] as string;
                return c;
            }
            return null;
        }

        public static void ADODeleteCustomer(long id)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            try
            {
                SqlCommand delete1 = new SqlCommand("delete from Customer_Lang where id = " + id, conn);
                SqlCommand delete2 = new SqlCommand("delete from Customer where id = " + id, conn);
                conn.Open();
                delete1.ExecuteNonQuery();
                delete2.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
