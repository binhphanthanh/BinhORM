using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.Exceptions
{
    public class RequiredFieldException : Exception
    {
        private string _filedName;

        public RequiredFieldException()
            : base()
        {
        }

        public RequiredFieldException(string msg)
            : base(msg)
        {
        }

        public RequiredFieldException(string msg, string fieldName)
            : base(msg)
        {
            _filedName = fieldName;
        }

        public string FieldName
        {
            get { return _filedName; }
        }
    }
}
