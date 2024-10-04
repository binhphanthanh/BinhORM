using System;

namespace SystemFramework.Common.Exceptions
{
    public class CanNotConnectDatabaseException : Exception
    {
        public CanNotConnectDatabaseException()
            : base("Không thể kết nối cơ sở dữ liệu. Có thể SqlServer chưa được khởi động hoặc nếu cơ sở dữ liệu được lưu trên máy khác trên mạng cục bộ thì xin hãy kiểm tra lại dây mạng")
        {
        }

        public CanNotConnectDatabaseException(string msg)
            : base(msg)
        {
        }
    }
}
