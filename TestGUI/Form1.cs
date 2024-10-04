using System;
using System.Windows.Forms;
using BusinessEntity;
using SystemFramework.Common;
using SystemFramework.Common.ObjectCollectionBase;
using TestDAO;
using WindowsApplication1;

namespace TestGUI
{
    public partial class Form1 : Form
    {
        private ObjectCollection<Customer> listKH;
        private ObjectCollectionView<Customer> listViewKH;

        CustomerBO dao = new CustomerBO();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();

            //Performance test

            //SpeedTester t = new SpeedTester(LoadTestData);
            //t.RunTest(1);
            //MessageBox.Show("time=" + t.AverageRunningTime);
        }        

        private void LoadTestData()
        {
            dao.GetCustomers();
        }

        private void LoadData()
        {
            
            //listKH = new ObjectCollection<Customer>(dao.GetCustomers_paging(10, 20));
            listKH = new ObjectCollection<Customer>(dao.GetCustomers4());
            listViewKH = new ObjectCollectionView<Customer>(listKH);
            dataGridView1.DataSource = listViewKH;
            
        }        

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                CustomerBO dao = new CustomerBO();
                dao.Update(listKH.GetChanges());
                listKH.AcceptChanges();
                MessageBox.Show(this, "đã lưu");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0 && dataGridView1.SelectedCells[0].RowIndex < listViewKH.Count-1)
            {
                int row = dataGridView1.SelectedCells[0].RowIndex;
                Customer kh = ((ObjectView<Customer>)dataGridView1.Rows[row].DataBoundItem).Object;
                DetailForm f = new DetailForm(kh.Id);
                f.UpdateEvent += new ItemUpdatedDelegate(f_UpdateEvent);
                f.Show();
            }
        }

        void f_UpdateEvent(BaseEntity entity)
        {
            LoadData();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listViewKH.ApplyFilter(
               delegate(Customer item)
               {
                   return string.IsNullOrEmpty(textBox1.Text)
                            || item.Name.ToLower().Contains(textBox1.Text.ToLower());
               }
           );
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //DetailForm f = new DetailForm(0);
            //f.UpdateEvent += new ItemUpdatedDelegate(f_UpdateEvent);
            //f.Show();

            
        }
    }
}