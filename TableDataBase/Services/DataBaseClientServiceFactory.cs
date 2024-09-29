using System;
using TableDataBase.Interfaces;

namespace TableDataBase.Services
{
    public class DataBaseClientServiceFactory : IDataBaseServiceFactory
    {
        public IDataBaseService Create(string connectionString, string dbName)
        {
            return new DataBaseClientService(connectionString, dbName);
        }
    }
}

