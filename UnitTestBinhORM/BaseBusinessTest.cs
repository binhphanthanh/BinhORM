using SystemFramework.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Common;
using TestBinhORM.Common;

namespace TestBinhORM
{
    
    /// <summary>
    ///This is a test class for BaseBusinessTest and is intended
    ///to contain all BaseBusinessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BaseBusinessTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private void AssertCustomer2(Customer2 c1, Customer2 c2)
        {
            Assert.AreEqual(c1.Id, c2.Id);
            Assert.AreEqual(c1.Name, c2.Name);
            Assert.AreEqual(c1.Address, c2.Address);
            Assert.AreEqual(c1.Deleted, c2.Deleted);
        }

        /// <summary>
        ///A test for BeginTransaction
        ///</summary>
        [TestMethod()]
        public void TransactionCommitTest()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dao = new MockupDAO(bo);
            long id1 = 0, id2 = 0;
            try
            {
                Customer2 c1 = new Customer2(0, "binh binh", "nha tui", "123456789", 100000000.0, false);
                Customer2 c2 = new Customer2(0, "binh binh2", "nha tui2", "123456789", 100000000.0, false);
                
                bo.BeginTransaction();
                dao.Insert<Customer2>(c1);
                id1 = c1.Id;
                dao.Insert<Customer2>(c2);
                id2 = c2.Id;
                bo.CommitTransaction();

                Customer2 c3 = DataUtils.ADOGetCustomer2(id1);
                Customer2 c4 = DataUtils.ADOGetCustomer2(id2);
                Assert.IsNotNull(c3);
                Assert.IsNotNull(c4);
                AssertCustomer2(c1, c3);
                AssertCustomer2(c2, c4);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id1);
                DataUtils.ADODeleteCustomer2(id2);
            }
        }

        [TestMethod()]
        public void TransactionRollbackTest()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dao = new MockupDAO(bo);
            long id1 = 0, id2 = 0;

            Customer2 c1 = new Customer2(0, "binh binh", "nha tui", "123456789", 100000000.0, false);
            Customer2 c2 = new Customer2(0, "binh binh2", "nha tui2", "123456789", 100000000.0, false);
            try
            {
                bo.BeginTransaction();
                dao.Insert<Customer2>(c1);
                id1 = c1.Id;
                dao.Insert<Customer2>(c2);
                id2 = c2.Id;
                throw new Exception("My intended exception");
                bo.CommitTransaction();
            }
            catch (Exception)
            {
                bo.RollBackTransaction();
            }
            Customer2 c3 = DataUtils.ADOGetCustomer2(id1);
            Customer2 c4 = DataUtils.ADOGetCustomer2(id2);
            Assert.IsNull(c3);
            Assert.IsNull(c4);
        }
    }
}
