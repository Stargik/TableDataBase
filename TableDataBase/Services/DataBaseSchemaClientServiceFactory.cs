using System;
using TableDataBase.Interfaces;

namespace TableDataBase.Services
{
    public class DataBaseSchemaClientServiceFactory : IDataBaseSchemaServiceFactory
    {
        public IDataBaseSchemaService Create(string connectionString)
        {
            return new DataBaseSchemaClientService(connectionString);
        }
    }
}

