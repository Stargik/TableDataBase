using System;
namespace TableDataBase.Interfaces
{
	public interface IDataBaseServiceFactory
	{
        IDataBaseService Create(string connectionString, string dbName);
    }
}

