using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Data.SqlClient;
using System.Configuration;
using PerformanceTestBinhORM.Common;

namespace PerformanceTestBinhORM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            List<TestResult> list = new List<TestResult>();
            list.Add(TestLoadBigResult());
            list.Add(TestLoadBigResultWithConstrutor());
            list.Add(TestLoadSmallResult());
            list.Add(TestLoadSmallResultConstructor());
            dataGridView1.DataSource = list;
        }

        public TestResult TestLoadBigResult()
        {
            int runtimes = 10;
            TestResult r1 = new TestResult();

            SpeedTester t = new SpeedTester(LoadBigListTest);
            t.RunTest(runtimes);
            SpeedTester t2 = new SpeedTester(ADOLoadBigListTest);
            t2.RunTest(runtimes);

            r1.TestMethod = "Load 8125 records";
            r1.RunTimes = runtimes;
            r1.AverageRunningTime = t.AverageRunningTime;
            r1.TotalRunningTime = t.TotalRunningTime;
            r1.ADOAverageRunningTime = t2.AverageRunningTime;
            r1.ADOTotalRunningTime = t2.TotalRunningTime;

            return r1;
        }

        public TestResult TestLoadBigResultWithConstrutor()
        {
            int runtimes = 10;
            TestResult r1 = new TestResult();

            SpeedTester t = new SpeedTester(LoadBigListConstructorTest);
            t.RunTest(runtimes);
            SpeedTester t2 = new SpeedTester(ADOLoadBigListTest);
            t2.RunTest(runtimes);

            r1.TestMethod = "Load 8125 records with constructor";
            r1.RunTimes = runtimes;
            r1.AverageRunningTime = t.AverageRunningTime;
            r1.TotalRunningTime = t.TotalRunningTime;
            r1.ADOAverageRunningTime = t2.AverageRunningTime;
            r1.ADOTotalRunningTime = t2.TotalRunningTime;

            return r1;
        }

        public TestResult TestLoadSmallResult()
        {
            int runtimes = 20;
            TestResult r1 = new TestResult();

            SpeedTester t = new SpeedTester(LoadSmallListTest);
            t.RunTest(runtimes);
            SpeedTester t2 = new SpeedTester(ADOLoadSmallListTest);
            t2.RunTest(runtimes);

            r1.TestMethod = "Load 50 records";
            r1.RunTimes = runtimes;
            r1.AverageRunningTime = t.AverageRunningTime;
            r1.TotalRunningTime = t.TotalRunningTime;
            r1.ADOAverageRunningTime = t2.AverageRunningTime;
            r1.ADOTotalRunningTime = t2.TotalRunningTime;

            return r1;
        }

        public TestResult TestLoadSmallResultConstructor()
        {
            int runtimes = 20;
            TestResult r1 = new TestResult();

            SpeedTester t = new SpeedTester(LoadSmallListTestConstructor);
            t.RunTest(runtimes);
            SpeedTester t2 = new SpeedTester(ADOLoadSmallListTest);
            t2.RunTest(runtimes);

            r1.TestMethod = "Load 50 records using constructor";
            r1.RunTimes = runtimes;
            r1.AverageRunningTime = t.AverageRunningTime;
            r1.TotalRunningTime = t.TotalRunningTime;
            r1.ADOAverageRunningTime = t2.AverageRunningTime;
            r1.ADOTotalRunningTime = t2.TotalRunningTime;

            return r1;
        }

        public void LoadBigListTest()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dao = new MockupDAO(bo);
            dao.GetList<Customer>(@"select * 
                                    from Customer c inner join Customer_Lang l on c.id = l.lid 
                                    where LanguageKey = 'vi-VN'");
        }

        public void LoadBigListConstructorTest()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dao = new MockupDAO(bo);
            dao.GetList<Customer>(@"select new Common.Customer(id, name, Address, Phone, money, deleted) 
                                    from Customer c inner join Customer_Lang l on c.id = l.lid 
                                    where LanguageKey = 'vi-VN' ");
        }

        public void ADOLoadBigListTest()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter(@"select * from Customer c inner join Customer_Lang l on c.id = l.lid 
                                                    where LanguageKey = 'vi-VN'", conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
        }

        public void LoadSmallListTest()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dao = new MockupDAO(bo);
            dao.GetList<Customer>(@"select * 
                                    from Customer c inner join Customer_Lang l on c.id = l.lid 
                                    where LanguageKey = 'vi-VN' ", 0, 50);
        }

        public void LoadSmallListTestConstructor()
        {
            MockupBO bo = new MockupBO();
            MockupDAO dao = new MockupDAO(bo);
            dao.GetList<Customer>(@"select new Common.Customer(id, name, Address, Phone, money, deleted) 
                                    from Customer c inner join Customer_Lang l on c.id = l.lid 
                                    where LanguageKey = 'vi-VN' ", 0, 50);
        }

        public void ADOLoadSmallListTest()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter(@"select * from Customer c inner join Customer_Lang l on c.id = l.lid 
                                                    where LanguageKey = 'vi-VN'", conn);
            DataSet ds = new DataSet();
            da.Fill(ds, 0, 50, "customer");
        }
    }
}
