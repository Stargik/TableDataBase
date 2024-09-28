using System;
using TableDataBase.Models;

namespace TableDataBase.Interfaces
{
	public interface IDataBaseService
	{
		void AddField(TableField tableField);
		void RemoveFieldByGuid(Guid guid);
		TableField? GetFieldByGuid(Guid guid);
		List<TableField>? GetAllFieldsByTableName(string tableName);
		void UpdateField(TableField tableField);

		void UpdateValue(dynamic Value, string attributePropertyName, Guid fieldGuid);

		void SaveChanges();
    }
}

