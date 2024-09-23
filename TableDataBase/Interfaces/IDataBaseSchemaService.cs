using System;
using TableDataBase.Models;

namespace TableDataBase.Interfaces
{
	public interface IDataBaseSchemaService
	{
		void AddJsonDbObjectSchema(DataBase dataBase);
		void RemoveJsonDbObjectSchemaByGuid(Guid guid);
        DataBase? GetDbObjectByGuid(Guid guid);
        List<DataBase>? GetAllDbObjects();
		void UpdateJsonDbObjectSchema(DataBase dataBase);

        void AddTable(Table table, Guid dbGuid);
        void RemoveTableByGuid(Guid guid, Guid dbGuid);
        Table? GetTableByGuid(Guid guid, Guid dbGuid);
        List<Table>? GetAllTablesByDbGuid(Guid dbGuid);
        void UpdateTable(Table table, Guid dbGuid);

        void AddAttributeProperty(AttributeProperty attributeProperty, Guid tableGuid, Guid dbGuid);
        void RemoveAttributePropertyByGuid(Guid guid, Guid tableGuid, Guid dbGuid);
        AttributeProperty? GetAttributePropertyByGuid(Guid guid, Guid tableGuid, Guid dbGuid);
		List<AttributeProperty>? GetAllAttributePropertiesByDbTableGuid(Guid tableGuid, Guid dbGuid);
        void UpdateAttributeProperty(AttributeProperty attributeProperty, Guid tableGuid, Guid dbGuid);

        void AddRelation(AttributeProperty attributeProperty, Guid tableGuid, Guid targetTableGuid, Guid dbGuid);

        string? GetDbFileNameByGuid(Guid guid);
        string? GetDbFilePath();
        void SaveChanges();
    }
}

