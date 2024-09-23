using System;
using TableDataBase.Models;

namespace TableDataBase.Interfaces
{
	public interface IDataBaseService
	{
		void AddField(TableField tableField);
		void RemoveFieldByGuid(Guid guid);
		TableField? GetFieldByGuid(Guid guid);
		List<TableField>? GetAllFieldsByTableGuid(Guid tableGuid);
		void UpdateField(TableField tableField);

		void UpdateValue(dynamic Value, Guid attributePropertyGuid, Guid fieldGuid);

		void SaveChanges();
    }
}

