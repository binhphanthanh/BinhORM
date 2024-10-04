using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.QueryBase
{
    public class PropertyAccessorException : Exception
    {
        public PropertyAccessorException(string message)
            : base(message)
        {
        }
    }
}
