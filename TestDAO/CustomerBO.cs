using System;
using System.Collections.Generic;
using System.Text;
using SystemFramework.Common;
using System.Collections;
using BusinessEntity;
using DataAccess;
using System.Data;

namespace TestDAO
{
    public class CustomerBO : BaseBusiness
    {
        private CustomerDAO khDAO;

        public CustomerBO()
        {
            khDAO = new CustomerDAO(this);
        }

        public IList<Customer> GetCustomers()
        {
            return khDAO.GetCustomers();
        }

        public IList<Temp> GetCustomerTemp()
        {
            return khDAO.SelectCustomerTemp();
        }

        public Customer GetCustomer(long maKH)
        {
            return khDAO.GetCustomer(maKH);
        }

        public DataSet GetCustomers2()
        {
            return khDAO.GetCustomers2();
        }

        public IList<Customer> GetCustomers3()
        {
            return khDAO.GetCustomers3();
        }

        public IList<Customer> GetCustomers4()
        {
            return khDAO.GetCustomers4();
        }
        public IList<Customer> GetCustomers_paging(int startRecord, int maxResult)
        {
            return khDAO.GetCustomers_paging(startRecord, maxResult);
        }

        public IList<REPO> GetKho()
        {
            return khDAO.SelectKho();
        }

        public void Update(List<Customer> listKH)
        {
            try
            {
                BeginTransaction();
                khDAO.UpdateCustomers(listKH);
                CommitTransaction();
            }
            catch (Exception)
            {
                RollBackTransaction();
                throw;
            }
        }

        public void Save(Customer kh)
        {
            if (kh.IsAdded)
                khDAO.InsertCustomer(kh);
            else
                khDAO.UpdateCustomer(kh);
        }

        public void Delete(long maKH)
        {
            khDAO.DeleteCustomer(maKH);
        }

        /// <summary>
        /// Hàm này chỉ cách sử dụng transaction
        /// </summary>
        public void ActionWithTransaction()
        {
            try
            {
                BeginTransaction();
                //làm nhiều thao tác inset/update/delete ở đây
                CommitTransaction();
            }
            catch (Exception)
            {
                RollBackTransaction();
            }
        }

        /// <summary>
        /// Hàm này chỉ cách sử dụng nhiều DAO trong 1 transaction
        /// </summary>
        public void ActionWithManyDAO()
        {
            try
            {
                CustomerDAO khDAO = new CustomerDAO(this);
                RepoDAO repoDAO = new RepoDAO(this);

                BeginTransaction();
                //làm nhiều thao tác inset/update/delete ở đây
                //Có thể sử dụng nhiều class DAO khác nhau
                //Transaction có tác dụng trên tất cả DAO
                //Khi 1 exception xuất hiện thì tất cả dc rollback
                CommitTransaction();
            }
            catch (Exception)
            {
                RollBackTransaction();
            }            
        }

        public void ActionWithOtherBO()
        {

            try
            {
                RepoBO repoBO = new RepoBO();
                Join(repoBO);

                BeginTransaction();
                //làm nhiều thao tác inset/update/delete ở đây
                //Có thể sử dụng nhiều class BO khác nhau
                //Transaction có tác dụng trên tất cả BO
                //Khi 1 exception xuất hiện thì tất cả dc rollback
                CommitTransaction();

                //Nếu muốn tách 1 BO ra khỏi transaction của BO hiện tại thì làm vầy:
                UnJoin(repoBO);
            }
            catch (Exception)
            {
                RollBackTransaction();
            }
        }

    }
}
