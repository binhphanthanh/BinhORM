using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using QueryBase;

namespace SystemFramework.Common.QueryBase
{
    public interface IDbFactory
    {
        DbConnection CreateConnection();
        DbCommand CreateCommand();
        DbParameter CreateParameter();
        IDataManager CreateDataManager(BaseBusiness business);
    }
}
