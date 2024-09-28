using System;
using TableDataBase.Models;

namespace TableDataBase.Interfaces
{
	public interface IDataBaseSchemaService
	{
		void AddJsonDbObjectSchema(DataBase dataBase);
		void RemoveJsonDbObjectSchemaByName(string name);
        DataBase? GetDbObjectByName(string name);
        List<DataBase>? GetAllDbObjects();
		void UpdateJsonDbObjectSchema(DataBase dataBase);

        void AddTable(Table table, string dbName);
        void RemoveTableByName(string name, string dbName);
        Table? GetTableByName(string name, string dbName);
        List<Table>? GetAllTablesByDbName(string dbName);
        void UpdateTable(Table table, string dbName);

        void AddAttributeProperty(AttributeProperty attributeProperty, string tableName, string dbName);
        void RemoveAttributePropertyByName(string name, string tableName, string dbName);
        AttributeProperty? GetAttributePropertyByName(string name, string tableName, string dbName);
		List<AttributeProperty>? GetAllAttributePropertiesByDbTableName(string tableName, string dbName);
        void UpdateAttributeProperty(AttributeProperty attributeProperty, string tableName, string dbName);

        string? GetDbFileNameByName(string Name);
        string? GetDbFilePath();
        void SaveChanges();
    }
}

