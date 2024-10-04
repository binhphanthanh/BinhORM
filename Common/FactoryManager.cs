using System;
using System.Collections.Generic;
using System.Text;
using SystemFramework.Common.QueryBase;
using System.Configuration;
using System.Reflection;

namespace SystemFramework.Common
{
    public class FactoryManager
    {
        private static IDbFactory _factory;

        public static IDbFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    string fs = ConfigurationManager.AppSettings["factory"];
                    if (string.IsNullOrEmpty(fs))
                    {
                        throw new Exception("Factory is not configure properly!");
                    }
                    Type factoryType = Type.GetType(fs);
                    ConstructorInfo constructor = factoryType.GetConstructor(new Type[0]);
                    _factory = (IDbFactory) constructor.Invoke(new object[0]);
                }
                return _factory;
            }
        }
    }
}
