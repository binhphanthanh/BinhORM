
namespace BusinessEntity
{
    using SystemFramework.Common;
    using SystemFramework.Common.ObjectCollectionBase;
    using System;
    using SystemFramework.Common.EntityBase;

    [Serializable]
    [Table(Name = "REPO")]
    public class REPO : BaseEntity
    {
        public const string IdProperty = "Id";
        public const string CusIdProperty = "CusId";
        public const string AreProperty = "Are";
        public const string NameProperty = "Name";
        public const string AddressProperty = "Address";
       
     
        private int _id;
        private int? _cusId;
        private double? _are;
        private string _name;
        private string _address;
        
        public REPO()
        {
        }

        [Key(AutoIncrement = true)]
        [Column(MappedName = "ID")]
        public int Id
        {
            get { return this._id; }
            set { UpdateValue(ref _id, value, IdProperty); }
        }

        [Column(MappedName = "CUS_ID")]
        public int? CusId
        {
            get { return this._cusId; }
            set { UpdateValue(ref _cusId, value, CusIdProperty); }
        }

        [Column(MappedName = "ARE")]
        public double? Are
        {
            get { return this._are; }
            set { UpdateValue(ref _are, value, AreProperty); }
        }

        [Column(MappedName = "NAME")]
        public string Name
        {
            get { return this._name; }
            set { UpdateValue(ref _name, value, NameProperty); }
        }

        [Column(MappedName = "ADDRESS")]
        public string Address
        {
            get { return this._address; }
            set { UpdateValue(ref _address, value, AddressProperty); }
        }
    }
}