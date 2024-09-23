using System;
using Newtonsoft.Json;
using TableDataBase.Interfaces;
using TableDataBase.Models;

namespace TableDataBase.Services
{
	public class DataBaseService : IDataBaseService
	{
        private readonly string fullFilePath;
        private readonly List<TableField> dataBaseContext;
        public DataBaseService(string filePath, string fileName)
        {
            fullFilePath = $"{filePath}/{fileName}";
            if (File.Exists(fullFilePath))
            {
                var jsonDatabase = File.ReadAllText(fullFilePath);
                dataBaseContext = JsonConvert.DeserializeObject<List<TableField>>(jsonDatabase) ?? new List<TableField>();
            }
            else
            {
                dataBaseContext = new List<TableField>();
            }
        }

        public void AddField(TableField tableField)
        {
            dataBaseContext.Add(tableField);
        }

        public List<TableField>? GetAllFieldsByTableGuid(Guid tableGuid)
        {
            var fields = dataBaseContext.Where(x => x.TableGuid == tableGuid);
            return fields.ToList();
        }

        public TableField? GetFieldByGuid(Guid guid)
        {
            var field = dataBaseContext.FirstOrDefault(x => x.Guid == guid);
            return field;
        }

        public void RemoveFieldByGuid(Guid guid)
        {
            var field = dataBaseContext.FirstOrDefault(x => x.Guid == guid);
            if (field is not null)
            {
                dataBaseContext.Remove(field);
            }
        }

        public void UpdateField(TableField tableField)
        {
            var oldField = dataBaseContext.FirstOrDefault(x => x.Guid == tableField.Guid);
            if (oldField is not null)
            {
                oldField.Values = tableField.Values;
            }
            else
            {
                dataBaseContext.Add(tableField);
            }
        }

        public void UpdateValue(dynamic Value, Guid attributePropertyGuid, Guid fieldGuid)
        {
            var field = dataBaseContext.FirstOrDefault(x => x.Guid == fieldGuid);
            if (field is not null && field.Values.ContainsKey(attributePropertyGuid))
            {
                field.Values[attributePropertyGuid] = Value;
            }
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(dataBaseContext, Formatting.Indented);
            File.WriteAllText(fullFilePath, json);
        }
    }
}

