using System;

namespace SystemFramework.Common.Exceptions
{
    public class KeyReferencedException : Exception
    {
        public KeyReferencedException()
            : base()
        {
        }

        public KeyReferencedException(string msg)
            : base(msg)
        {
        }
    }
}
