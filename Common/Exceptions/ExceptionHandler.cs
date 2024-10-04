using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace SystemFramework.Common.Exceptions
{
    public static class ExceptionHandler
    {
        public static void Handles(Exception inEx)
        {
            try
            {
                throw inEx;
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 547:
                        throw new KeyReferencedException(ex.Message);
                    case 2627:
                        throw new DuplicatedException(ex.Message);
                    case 2:
                        throw new CanNotConnectDatabaseException();
                    default:
                        throw;
                }
            }
        }
    }
}
