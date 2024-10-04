using System;

namespace SystemFramework.Common.Exceptions
{
    public class DuplicatedException : Exception
    {
         public DuplicatedException()
            : base()
        {
        }

        public DuplicatedException(string msg)
            : base(msg)
        {
        }
    }
}
