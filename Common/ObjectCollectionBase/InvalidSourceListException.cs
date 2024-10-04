using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.ObjectCollectionBase
{
    [Serializable]
    public class InvalidSourceListException : Exception
    {
        public InvalidSourceListException()
           : base(SystemFramework.Properties.Resources.InvalidSourceList)
        {
            
        }

        public InvalidSourceListException(string message)
            : base(message)
        {

        }

        public InvalidSourceListException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {

        }
    }
}
