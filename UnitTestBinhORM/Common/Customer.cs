using System;
using SystemFramework.Common;
using SystemFramework.Common.EntityBase;

namespace Common
{
    [Serializable]
    [Table(Name = "Customer", MultiLanguage = true)]
    public class Customer : BaseEntity
    {
        public const string IdProperty = "Id";
        public const string NameProperty = "Name";
        public const string AddressProperty = "Address";
        public const string PhoneProperty = "Phone";

        private long _id;
        private string _name;
        private string _address;
        private string _phone;
        private double _money;
        private bool _deleted;

        public Customer()
        {
        }

        public Customer(long id, string name, string address, string phone, double money, bool deleted)
        {
            _id = id;
            _name = name;
            _address = address;
            _phone = phone;
            _money = money;
            _deleted = deleted;
        }

        [Column(MappedName = "Money")]
        public double Money
        {
            get { return _money; }
            set { _money = value; }
        }        

        [Key(AutoIncrement = true)]
        [Column(MappedName = "id")]
        public long Id
        {
            get { return this._id; }
            set { UpdateValue(ref _id, value, IdProperty); }
        }

        [Column(MappedName = "name", MultiLanguage = true)]
        public string Name
        {
            get { return this._name; }
            set { UpdateValue(ref _name, value, NameProperty); }
        }

        [Column(MappedName = "ADDRESS", MultiLanguage = true)]
        public string Address
        {
            get { return this._address; }
            set { UpdateValue(ref _address, value, AddressProperty); }
        }

        [Column(MappedName = "PHONE")]
        public string Phone
        {
            get { return this._phone; }
            set { UpdateValue(ref _phone, value, PhoneProperty); }
        }

        [Column(MappedName = "Deleted")]
        public bool Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }
    }
}
