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

        public void RemoveJsonDbObjectSchemaByName(string name)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == name);
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

        public DataBase? GetDbObjectByName(string name)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == name);
            return dataBase;
        }

        public List<DataBase>? GetAllDbObjects()
        {
            return dataBasesContext;
        }

        public void UpdateJsonDbObjectSchema(DataBase dataBase)
        {
            var oldDataBase = dataBasesContext.FirstOrDefault(x => x.Name == dataBase.Name);
            if (oldDataBase is not null)
            {
                oldDataBase.Name = oldDataBase.Name;
                oldDataBase.Tables = dataBase.Tables;
            }
            else
            {
                dataBasesContext.Add(dataBase);
            }
        }

        public void AddTable(Table table, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                dataBase.Tables.Add(table);
            }
        }

        public void RemoveTableByName(string name, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == name);
                if (table is not null)
                {
                    dataBase.Tables.Remove(table);
                }
            }
        }

        public Table? GetTableByName(string name, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == name);
                return table;
            }
            return null;
        }

        public List<Table>? GetAllTablesByDbName(string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                return dataBase.Tables;
            }
            return null;
        }

        public void UpdateTable(Table table, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var oldTable = dataBase.Tables.FirstOrDefault(x => x.Name == table.Name);
                if (oldTable is not null)
                {
                    oldTable.Name = table.Name;
                    oldTable.AttributeProperties = table.AttributeProperties;
                }
                else
                {
                    dataBase.Tables.Add(table);
                }
            }
        }

        public void AddAttributeProperty(AttributeProperty attributeProperty, string tableName, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == tableName);
                if (table is not null)
                {
                    table.AttributeProperties.Add(attributeProperty);
                }
            }
        }

        public void RemoveAttributePropertyByName(string name, string tableName, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == tableName);
                if (table is not null)
                {
                    var attributeProperty = table.AttributeProperties.FirstOrDefault(x => x.Name == name);
                    if (attributeProperty is not null)
                    {
                        table.AttributeProperties.Remove(attributeProperty);
                    }
                }
            }
        }

        public AttributeProperty? GetAttributePropertyByName(string name, string tableName, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == tableName);
                if (table is not null)
                {
                    var attributeProperty = table.AttributeProperties.FirstOrDefault(x => x.Name == name);
                    return attributeProperty;
                }
            }
            return null;
        }

        public List<AttributeProperty>? GetAllAttributePropertiesByDbTableName(string tableName, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == tableName);
                if (table is not null)
                {
                    return table.AttributeProperties;
                }
            }
            return null;
        }

        public void UpdateAttributeProperty(AttributeProperty attributeProperty, string tableName, string dbName)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == dbName);
            if (dataBase is not null)
            {
                var table = dataBase.Tables.FirstOrDefault(x => x.Name == tableName);
                if (table is not null)
                {
                    var oldAttributeProperty = table.AttributeProperties.FirstOrDefault(x => x.Name == attributeProperty.Name);
                    if (oldAttributeProperty is not null)
                    {
                        oldAttributeProperty.Name = attributeProperty.Name;
                        oldAttributeProperty.AttributeType = attributeProperty.AttributeType;
                    }
                    else
                    {
                        table.AttributeProperties.Add(attributeProperty);
                    }
                }
            }
        }

        public string? GetDbFileNameByName(string name)
        {
            var dataBase = dataBasesContext.FirstOrDefault(x => x.Name == name);
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
            return $"{dataBase.Name}.json";
        }
    }
}

