using System;
namespace TableDataBase.Interfaces
{
	public interface IDataBaseSchemaServiceFactory
	{
		IDataBaseSchemaService Create(string connectionString);
	}
}

