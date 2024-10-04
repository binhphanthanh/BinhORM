using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SystemFramework.Common;
using Common;
using TestBinhORM.Common;

namespace TestBinhORM
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class BaseDataAccessTest
    {
        public BaseDataAccessTest()
        {

        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private Customer2 PrepareCustomer2()
        {
            Customer2 c = new Customer2(0, "binh phan", "nha tui", "123456789", 100000000.0, false);
            DataUtils.ADOInsertCustomer2(c);
            return c;
        }

        private void AssertCustomer2(Customer2 c1, Customer2 c2)
        {
            Assert.AreEqual(c1.Id, c2.Id);
            Assert.AreEqual(c1.Name, c2.Name);
            Assert.AreEqual(c1.Address, c2.Address);
            Assert.AreEqual(c1.Deleted, c2.Deleted);
        }

        private void Asserttemp(Temp c1, Customer2 c2)
        {
            Assert.AreEqual(c1.Id, c2.Id);
            Assert.AreEqual(c1.Name, c2.Name);
            Assert.AreEqual(c1.Address, c2.Address);
        }

        private void AssertCustomer(Customer c1, Customer c2)
        {
            Assert.AreEqual(c1.Id, c2.Id);
            Assert.AreEqual(c1.Name, c2.Name);
            Assert.AreEqual(c1.Address, c2.Address);
            Assert.AreEqual(c1.Deleted, c2.Deleted);
        }

        [TestMethod]
        public void TestInsert()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = new Customer2(0, "binh phan", "nha tui", "123456789", 100000000.0, false);

                dataAccess.Insert<Customer2>(c);

                Assert.IsTrue(c.Id > 0);
                id = c.Id;
                Customer2 c2 = DataUtils.ADOGetCustomer2(id);
                Assert.IsNotNull(c2);
                AssertCustomer2(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestUpdate()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                id = c.Id;
                c.Name = "phan thanh binh";

                dataAccess.Update<Customer2>(c);

                Customer2 c2 = DataUtils.ADOGetCustomer2(id);
                AssertCustomer2(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestDelete()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                id = c.Id;

                dataAccess.Delete<Customer2>(c.Id);

                Customer2 c2 = DataUtils.ADOGetCustomer2(id);
                Assert.IsNull(c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestExecuteNonQuery()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                c.Name = "binh bong";
                c.Address = "my home";
                id = c.Id;
                string sql =
                    @"update Customer_1 
                      set name = @name, address=@address where id=@id";
                dataAccess.ExecuteNonQuery(sql, "@name", c.Name, "@address", c.Address, "@id", c.Id);

                Customer2 c2 = DataUtils.ADOGetCustomer2(id);
                AssertCustomer2(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestExecuteNonQueryWithEntity()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                c.Name = "binh bong";
                id = c.Id;
                string sql =
                    @"update Customer_1 
                      set name = @name, address=@address, phone=@phone,money=@money,deleted=@deleted
                      where id=@id";
                dataAccess.ExecuteNonQuery(sql, c);

                Customer2 c2 = DataUtils.ADOGetCustomer2(id);
                AssertCustomer2(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestExecuteScalar()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                string sql = "select count(*) from Customer_1 where id > @id";
                int i1 = Convert.ToInt32(DataUtils.ADOExecuteScalar(sql, "@id", 3));
                int i2 = Convert.ToInt32(dataAccess.ExecuteScalar(sql, "@id", 3));

                Assert.AreEqual(i1, i2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestFind()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                id = c.Id;

                Customer2 c2 = dataAccess.Find<Customer2>(c.Id);
                Assert.IsNotNull(c2);
                AssertCustomer2(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestLoadData()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                id = c.Id;

                Customer2 c2 = dataAccess.LoadData<Customer2>("select * from Customer_1 where name = @name", "name", "binh phan");
                Assert.IsNotNull(c2);
                AssertCustomer2(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestLoadDataWithoutEntity()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                id = c.Id;

                Temp c2 = dataAccess.LoadData<Temp>("select * from Customer_1 where name = @name", "name", "binh phan");
                Assert.IsNotNull(c2);
                Asserttemp(c2, c);
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestLoadDataWithConstructor()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer2 c = PrepareCustomer2();
                id = c.Id;

                Temp c2 = dataAccess.LoadData<Temp>("select new Common.Temp(id, name, address, 'what ever') from Customer_1 where name = @name", "name", "binh phan");
                Assert.IsNotNull(c2);
                Asserttemp(c2, c);
                Assert.AreEqual(c2.WhatEver, "what ever");
            }
            finally
            {
                DataUtils.ADODeleteCustomer2(id);
            }
        }

        [TestMethod]
        public void TestLoadListOfData()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);

            IList<Customer2> list = dataAccess.GetList<Customer2>("select * from Customer_1 where name = @name", "name", "Thanh Binh");
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 32);
        }

        [TestMethod]
        public void TestLoadListOfDataWithoutEntity()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);

            IList<Temp> list = dataAccess.GetList<Temp>("select * from Customer_1 where name = @name", "name", "Thanh Binh");
            IList<Customer2> list2 = dataAccess.GetList<Customer2>("select * from Customer_1 where name = @name", "name", "Thanh Binh");

            Assert.IsNotNull(list);
            Assert.IsNotNull(list2);
            Assert.AreEqual(list.Count, 32);
            Assert.AreEqual(list2.Count, 32);
            for(int i=0; i<list.Count; i++)
            {
                Asserttemp(list[i], list2[i]);
            }
        }

        [TestMethod]
        public void TestLoadListOfDataWithConstructor()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);

            IList<Temp> list = dataAccess.GetList<Temp>("select new Common.Temp(id, name, address, 'what ever') from Customer_1 where name = @name", "name", "Thanh Binh");
            IList<Customer2> list2 = dataAccess.GetList<Customer2>("select * from Customer_1 where name = @name", "name", "Thanh Binh");

            Assert.IsNotNull(list);
            Assert.IsNotNull(list2);
            Assert.AreEqual(list.Count, 32);
            Assert.AreEqual(list2.Count, 32);
            for (int i = 0; i < list.Count; i++)
            {
                Asserttemp(list[i], list2[i]);
                Assert.AreEqual(list[i].WhatEver, "what ever");
            }
        }

        [TestMethod]
        public void TestLoadListOfDataPaging()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);

            IList<Customer2> list = dataAccess.GetList<Customer2>("select * from Customer_1 where name = @name", 3, 10, "name", "Thanh Binh");
            Assert.IsNotNull(list);
            Assert.AreEqual(list.Count, 10);
            Assert.AreEqual(list[0].Id, 6);
        }

        [TestMethod]
        public void TestInsertMultiLingual()
        {
            long id = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer c = new Customer(0, "binh phan", "nha tui", "123456789", 100000000.0, false);
                c.LanguageKey = "en-US";
                dataAccess.Insert<Customer>(c);
                id = c.Id;

                Assert.IsTrue(c.Id > 0);
                
                Customer c2 = DataUtils.ADOGetCustomer(id, c.LanguageKey);
                Assert.IsNotNull(c2);
                AssertCustomer(c, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer(id);
            }
        }

        [TestMethod]
        public void TestInsertUpdateMultiLingual2()
        {
            long id1 = 0, id2 = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                Customer c1 = new Customer(0, "binh phan", "nha tui", "123456789", 100000000.0, false);
                c1.LanguageKey = "en-US";
                dataAccess.Insert<Customer>(c1);
                id1 = c1.Id;

                Customer c2 = new Customer(c1.Id, "binh phan vn", "nha tui vn", "987654321", 100000000.0, false);
                c2.LanguageKey = "vi-VN";
                dataAccess.Update<Customer>(c2);
                id2 = c2.Id;

                Assert.IsTrue(c2.Id > 0);
                
                Customer c3 = DataUtils.ADOGetCustomer(c1.Id, c1.LanguageKey);
                Customer c4 = DataUtils.ADOGetCustomer(c2.Id, c2.LanguageKey);
                Assert.IsNotNull(c3);
                Assert.IsNotNull(c4);
                Assert.AreEqual(c1.Id, c2.Id);
                Assert.AreEqual(c3.LanguageKey, c1.LanguageKey);
                Assert.AreEqual(c4.LanguageKey, c2.LanguageKey);
                AssertCustomer(c3, c1);
                AssertCustomer(c4, c2);
            }
            finally
            {
                DataUtils.ADODeleteCustomer(id1);
                DataUtils.ADODeleteCustomer(id2);
            }
        }

        [TestMethod]
        public void TestDeleteMultiLingual()
        {
            long id1 = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                id1 = Convert.ToInt32(DataUtils.ADOExecuteScalar(@"insert into Customer(phone, money, deleted) values('0945616', 83464, 0)
                                                                    select scope_identity()"));
                DataUtils.ADOExecuteNoneQuery("insert into Customer_Lang values(@id, 'en-US', 'binh1', 'sss')", "@id", id1);
                DataUtils.ADOExecuteNoneQuery("insert into Customer_Lang values(@id, 'vi-VN', 'binh2', 'zzzz')", "@id", id1);

                dataAccess.Delete<Customer>(id1);

                Customer c3 = DataUtils.ADOGetCustomer(id1, "en-US");
                Customer c4 = DataUtils.ADOGetCustomer(id1, "vi-VN");
                int count = Convert.ToInt32(DataUtils.ADOExecuteScalar("select count(*) from Customer where id=" + id1));

                Assert.AreEqual(count, 0);
                Assert.IsNull(c3);
                Assert.IsNull(c4);
            }
            finally
            {
                DataUtils.ADODeleteCustomer(id1);
            }
        }

        [TestMethod]
        public void TestFindByIdMultiLingual()
        {
            long id1 = 0;
            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);
            try
            {
                id1 = Convert.ToInt32(DataUtils.ADOExecuteScalar(@"insert into Customer(phone, money, deleted) values('0945616', 83464, 0)
                                                                    select scope_identity()"));
                DataUtils.ADOExecuteNoneQuery("insert into Customer_Lang values(@id, 'en-US', 'binh1', 'sss')", "@id", id1);
                DataUtils.ADOExecuteNoneQuery("insert into Customer_Lang values(@id, 'vi-VN', 'binh2', 'zzzz')", "@id", id1);

                Customer c = dataAccess.FindByLanguage<Customer>("en-US", id1);
                Assert.IsNotNull(c);
                Assert.AreEqual(c.Money, 83464);
                Assert.AreEqual(c.Name, "binh1");

                Customer c2 = dataAccess.FindByLanguage<Customer>("vi-VN", id1);
                Assert.IsNotNull(c2);
                Assert.AreEqual(c2.Money, 83464);
                Assert.AreEqual(c2.Name, "binh2");
            }
            finally
            {
                DataUtils.ADODeleteCustomer(id1);
            }
        }

        [TestMethod]
        public void TestGetListMultiLingual()
        {

            MockupBO bo = new MockupBO();
            MockupDAO dataAccess = new MockupDAO(bo);

            int count = Convert.ToInt32(DataUtils.ADOExecuteScalar(@"select count(*) from Customer_Lang 
                                        where name='Thanh Binh' and LanguageKey='en-US'"));

            IList<Customer> c = dataAccess.GetListByLanguage<Customer>("name = @name", "en-US", "name", "Thanh Binh");
            Assert.AreEqual(c.Count, count);
        }
    }
}
