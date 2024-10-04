using System;
using System.Collections.Generic;
using System.Text;

namespace SystemFramework.Common.Exceptions
{
    public class ParamPropertyNotFoundException : Exception
    {
          public ParamPropertyNotFoundException()
            : base("Không tìm thấy property cho parameter.")
        {
        }

        public ParamPropertyNotFoundException(string prop)
            : base("Không tìm thấy property '" + prop + "' cho parameter '@" + prop + "'")
        {
        }
    }
}
