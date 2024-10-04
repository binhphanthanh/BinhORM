using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.ObjectCollectionBase
{
    public class KeyParametersNotValidException : Exception
    {
        public KeyParametersNotValidException()
            : base("Number of key collums diference from number of key value objects.")
        {
        }
    }
}
