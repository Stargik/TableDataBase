using System;
using Newtonsoft.Json;
using TableDataBase.Interfaces;
using TableDataBase.Models;

namespace TableDataBase.Services
{
    public class DataBaseSchemaService : IDataBaseSchemaService
    {
        private readonly string filePath;
        private readonly string fileName;
        private readonly string fullFilePath;
        private readonly List<DataBase> dataBasesContext;
        public DataBaseSchemaService(string filePath, string fileName)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            fullFilePath = $"{filePath}/{fileName}";
            if (File.Exists(fullFilePath))
            {
                var jsonDatabases = File.ReadAllText(fullFilePath);
                dataBasesContext = JsonConvert.DeserializeObject<List<DataBase>>(jsonDatabases) ?? new List<DataBase>();
            }
            else
            {
                dataBasesContext = new List<DataBase>();
            }
        }

        public void AddJsonDbObjectSchema(DataBase dataBase)
        {
            dataBasesContext.Add(dataBase);
            var dbFileName = $"{filePath}/{GetDbFileName(dataBase)}";
            File.AppendAllText(dbFileName, "");
        }

        public void RemoveJsonDbObjectSchemaByGuid(Guid guid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == guid);
            if (dataBase is not null)
            {
                dataBasesContext.Remove(dataBase);
                var dbFileName = $"{filePath}/{GetDbFileName(dataBase)}";
                if (File.Exists(dbFileName))
                {
                    File.Delete(dbFileName);
                }
            }
        }

        public DataBase? GetDbObjectByGuid(Guid guid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == guid);
            return dataBase;
        }

        public List<DataBase>? GetAllDbObjects()
        {
            return dataBasesContext;
        }

        public void UpdateJsonDbObjectSchema(DataBase dataBase)
        {
            var oldDataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dataBase.Guid);
            if (oldDataBase is not null)
            {
                dataBase.Guid = oldDataBase.Guid;
                oldDataBase = dataBase;
            }
            else
            {
                dataBasesContext.Add(dataBase);
            }
        }

        public void AddTable(Table table, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                dataBase.Tables.Add(table);
            }
        }

        public void RemoveTableByGuid(Guid guid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == guid);
                if (table is not null)
                {
                    dataBase.Tables.Remove(table);
                }
            }
        }

        public Table? GetTableByGuid(Guid guid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == guid);
                return table;
            }
            return null;
        }

        public List<Table>? GetAllTablesByDbGuid(Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                return dataBase.Tables;
            }
            return null;
        }

        public void UpdateTable(Table table, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var oldTable = dataBase.Tables.FirstOrDefault(x => x.Guid == table.Guid);
                if (oldTable is not null)
                {
                    table.Guid = oldTable.Guid;
                    oldTable = table;
                }
                else
                {
                    dataBase.Tables.Add(table);
                }
            }
        }

        public void AddAttributeProperty(AttributeProperty attributeProperty, Guid tableGuid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == tableGuid);
                if (table is not null)
                {
                    table.AttributeProperties.Add(attributeProperty);
                }
            }
        }

        public void RemoveAttributePropertyByGuid(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == tableGuid);
                if (table is not null)
                {
                    var attributeProperty = table.AttributeProperties.FirstOrDefault(x => x.Guid == guid);
                    if (attributeProperty is not null)
                    {
                        table.AttributeProperties.Remove(attributeProperty);
                    }
                }
            }
        }

        public AttributeProperty? GetAttributePropertyByGuid(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == tableGuid);
                if (table is not null)
                {
                    var attributeProperty = table.AttributeProperties.FirstOrDefault(x => x.Guid == guid);
                    return attributeProperty;
                }
            }
            return null;
        }

        public List<AttributeProperty>? GetAllAttributePropertiesByDbTableGuid(Guid tableGuid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == tableGuid);
                if (table is not null)
                {
                    return table.AttributeProperties;
                }
            }
            return null;
        }

        public void UpdateAttributeProperty(AttributeProperty attributeProperty, Guid tableGuid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == tableGuid);
                if (table is not null)
                {
                    var oldAttributeProperty = table.AttributeProperties.FirstOrDefault(x => x.Guid == attributeProperty.Guid);
                    if (oldAttributeProperty is not null)
                    {
                        oldAttributeProperty.Guid = attributeProperty.Guid;
                        oldAttributeProperty = attributeProperty;
                    }
                    else
                    {
                        table.AttributeProperties.Add(attributeProperty);
                    }
                }
            }
        }

        public void AddRelation(AttributeProperty attributeProperty, Guid tableGuid, Guid targetTableGuid, Guid dbGuid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == dbGuid);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Guid == tableGuid);
                var targetTable = dataBase.Tables.FirstOrDefault(x => x.Guid == targetTableGuid);
                if (table is not null && targetTable is not null)
                {
                    table.AttributeProperties.Add(attributeProperty);
                }
            }
        }

        public string? GetDbFileNameByGuid(Guid guid)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Guid == guid);
            if (dataBase is not null)
            {
                return GetDbFileName(dataBase);
            }
            return null;
        }

        public string? GetDbFilePath()
        {
            return filePath;
        }

        public void SaveChanges()
        {
            var json = JsonConvert.SerializeObject(dataBasesContext, Formatting.Indented);
            File.WriteAllText(fullFilePath, json);
        }

        private string GetDbFileName(DataBase dataBase)
        {
            return $"{dataBase.Name}-{dataBase.Guid}.json";
        }
    }
}

