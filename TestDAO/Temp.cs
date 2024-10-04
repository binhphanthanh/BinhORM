using System;
using System.Collections.Generic;
using System.Text;

namespace TestDAO
{
    public class Temp
    {
        private int _id;
        private string _name;
        private string _address;

        public int Id
        {
            get { return this._id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { _name = value; }
        }

        public string Address
        {
            get { return this._address; }
            set { _address = value; }
        }
    }
}
