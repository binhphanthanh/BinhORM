
using System.Collections.Generic;
using BusinessEntity;
using SystemFramework.Common;
using System.Data.SqlClient;
using System.Data;
using System;
using SystemFramework.Common.Exceptions;
using TestDAO;
using System.Data.OleDb;

namespace DataAccess
{
   

    public class CustomerDAO : BaseDataAccess
    {

        public CustomerDAO(BaseBusiness baseBusiness)
            : base(baseBusiness)
        {
        }

        public IList<Customer> GetCustomers()
        {
            string sqlstring = @"SELECT new Customer(c.Id,Name,Address,Phone,Money, Deleted) FROM Customer c left join Customer_Lang cl on c.Id = cl.id";
            return GetListByLanguage<Customer>(sqlstring, "id", 3);
        }

        public IList<Customer> GetCustomers_paging(int startRecord, int maxResult)
        {
            string sqlstring = @"SELECT new Customer(Id,Name,Address,Phone,Money, Deleted) FROM Customer c left join Customer_Lang cl on c.Id = cl.id";
            return GetList<Customer>(sqlstring, startRecord, maxResult, "id", 3);
        }

        public DataSet GetCustomers2()
        {
            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=G:\My projects\BinhORM\TestFrameWork.mdb;User Id=admin;Password=;");
            OleDbDataAdapter da = new OleDbDataAdapter("select * from Customer", conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public IList<Customer> GetCustomers3()
        {
            string sqlstring = @"SELECT * FROM Customer c left join Customer_Lang cl on c.Id = cl.id";
            return GetListByLanguage<Customer>(sqlstring, "id", 3);
        }

        public IList<Customer> GetCustomers4()
        {
            string sqlstring = @" Customer.id < @id";
            return GetListByLanguage<Customer>(sqlstring, "vi-VN", "id", 100000);
        }

        public IList<Temp> SelectCustomerTemp()
        {
            string sqlstring = @"SELECT Id, Name, Address FROM Customer  c left join Customer_Lang cl on c.Id = cl.id where id < 100";
            return GetList<Temp>(sqlstring);
        }

        public IList<REPO> SelectKho()
        {
            string sqlstring = @"SELECT Id, Name, Address, Are, cus_id as CusId FROM REPO";
            return GetList<REPO>(sqlstring);
        }

        public int InsertCustomer(Customer entity)
        {
            return Insert(entity);
        }

        public int UpdateCustomer(Customer entity)
        {
            return Update(entity);
        }

        public int DeleteCustomer(long makh)
        {
            return base.Delete<Customer>(makh);
        }

        public int UpdateCustomers(List<Customer> listKH)
        {
            return BatchUpdate(listKH);
        }

        public Customer GetCustomer(long id)
        {
            return FindByLanguage<Customer>("vi-VN", id);
        }
    }
}
