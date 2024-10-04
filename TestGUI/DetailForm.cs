using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TestDAO;
using BusinessEntity;
using SystemFramework.Common;

namespace TestGUI
{
    public delegate void ItemUpdatedDelegate(BaseEntity entity);

    public partial class DetailForm : Form
    {
        private Customer kh;

        public event ItemUpdatedDelegate UpdateEvent;

        public DetailForm(long maKH)
        {
            InitializeComponent();
            if (maKH > 0)
                kh = new CustomerBO().GetCustomer(maKH);
            else
                kh = new Customer();
            txtname.DataBindings.Add("Text", kh, Customer.NameProperty, false, DataSourceUpdateMode.OnPropertyChanged);
            txtaddress.DataBindings.Add("Text", kh, Customer.AddressProperty, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CustomerBO().Save(kh);
            this.Close();
            if (UpdateEvent != null)
                UpdateEvent(kh);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            new CustomerBO().Delete(kh.Id);
            this.Close();
            if (UpdateEvent != null)
                UpdateEvent(kh);
        }
    }
}