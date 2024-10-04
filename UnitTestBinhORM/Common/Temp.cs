using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Temp
    {
        private long _id;
        private string _name;
        private string _address;
        private string _whatEver;

        public Temp()
        {
        }

        public Temp(long id, string name, string address, string whatEver)
        {
            _id = id;
            _name = name;
            _address = address;
            _whatEver = whatEver;
        }

        public long Id
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

        public string WhatEver
        {
            get { return _whatEver; }
            set { _whatEver = value; }
        }
    }
}
