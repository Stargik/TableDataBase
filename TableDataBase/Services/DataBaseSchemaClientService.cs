using Grpc.Net.Client;
using TableDataBaseServerService;
using TableDataBase.Interfaces;
using TableDataBase.Models;

namespace TableDataBase.Services
{
	public class DataBaseSchemaClientService : IDataBaseSchemaService
    {
        private readonly TableDataBaseServise.TableDataBaseServiseClient client;

        public DataBaseSchemaClientService(string connectionString)
		{
            var channel = GrpcChannel.ForAddress(connectionString);
            client = new TableDataBaseServise.TableDataBaseServiseClient(channel);
        }

        public void AddJsonDbObjectSchema(DataBase dataBase)
        {
            var dataBaseRequest = new AddJsonDbObjectSchemaRequest
            {
                Name = dataBase.Name,
            };

            var tables = dataBase.Tables.Select(t => new TableReply
            {
                Name = t.Name
            }).ToList();

            foreach (var table in tables)
            {
                var props = dataBase.Tables.FirstOrDefault(x => x.Name == table.Name).AttributeProperties.Select(p => new AttributePropertyReply
                {
                    Name = p.Name,
                    AttributeType = (int)p.AttributeType
                }).ToList();
                table.AttributeProperties.AddRange(props);
            }

            dataBaseRequest.Tables.AddRange(tables);

            client.AddJsonDbObjectSchema(dataBaseRequest);
        }

        public void RemoveJsonDbObjectSchemaByName(string name)
        {
            var request = new RemoveJsonDbObjectSchemaByNameRequest { Name = name };
            client.RemoveJsonDbObjectSchemaByName(request);
        }

        public DataBase? GetDbObjectByName(string name)
        {
            var request = new GetDbObjectByNameRequest { Name = name };
            var reply = client.GetDbObjectByName(request);
            var dataBase = new DataBase
            {
                Name = reply.Name,
                Tables = reply.Tables.Select(t => new Table
                {
                    Name = t.Name,
                    AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                    {
                        AttributeType = (AttributeType)p.AttributeType,
                        Name = p.Name
                    }).ToList()
                }).ToList()
            };
            return dataBase;
        }

        public List<DataBase>? GetAllDbObjects()
        {
            var reply = client.GetAllDbObjects(new Google.Protobuf.WellKnownTypes.Empty());
            var dataBases = reply.DataBases.Select(x => new DataBase
            {
                Name = x.Name,
                Tables = x.Tables.Select(t => new Table
                {
                    Name = t.Name,
                    AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                    {
                        AttributeType = (AttributeType)p.AttributeType,
                        Name = p.Name
                    }).ToList()
                }).ToList()
            }
            ).ToList();
            return dataBases;
        }

        public void UpdateJsonDbObjectSchema(DataBase dataBase)
        {
            var dataBaseRequest = new UpdateJsonDbObjectSchemaRequest
            {
                Name = dataBase.Name
            };

            var tables = dataBase.Tables.Select(t => new TableReply
            {
                Name = t.Name
            }).ToList();

            foreach (var table in tables)
            {
                var props = dataBase.Tables.FirstOrDefault(x => x.Name == table.Name).AttributeProperties.Select(p => new AttributePropertyReply
                {
                    Name = p.Name,
                    AttributeType = (int)p.AttributeType
                }).ToList();
                table.AttributeProperties.AddRange(props);
            }

            dataBaseRequest.Tables.AddRange(tables);

            client.UpdateJsonDbObjectSchema(dataBaseRequest);
        }

        public void AddTable(Table table, string dbName)
        {
            var tableReply = new TableReply { Name = table.Name };
            var props = table.AttributeProperties.Select(p => new AttributePropertyReply
            {
                Name = p.Name,
                AttributeType = (int)p.AttributeType
            }).ToList();
            tableReply.AttributeProperties.AddRange(props);

            var request = new AddTableRequest { DbName = dbName, Table = tableReply };

            client.AddTable(request);
        }

        public void RemoveTableByName(string name, string dbName)
        {
            var request = new RemoveTableByNameRequest { Name = name, DbName = dbName };
            client.RemoveTableByName(request);
        }

        public Table? GetTableByName(string name, string dbName)
        {
            var request = new GetTableByNameRequest { Name = name, DbName = dbName };
            var reply = client.GetTableByName(request);
            var table = new Table
            {
                Name = reply.Name,
                AttributeProperties = reply.AttributeProperties.Select(p => new AttributeProperty
                {
                    AttributeType = (AttributeType)p.AttributeType,
                    Name = p.Name
                }).ToList()
            };
            return table;
        }

        public List<Table>? GetAllTablesByDbName(string dbName)
        {
            var request = new GetAllTablesByDbNameRequest { DbName = dbName };
            var reply = client.GetAllTablesByDbName(request);
            var tables = reply.Tables.Select(t => new Table
            {
                Name = t.Name,
                AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                {
                    AttributeType = (AttributeType)p.AttributeType,
                    Name = p.Name
                }).ToList()
            }).ToList();
            return tables;
        }

        public void UpdateTable(Table table, string dbName)
        {
            var tableReply = new TableReply { Name = table.Name };
            var props = table.AttributeProperties.Select(p => new AttributePropertyReply
            {
                Name = p.Name,
                AttributeType = (int)p.AttributeType
            }).ToList();
            tableReply.AttributeProperties.AddRange(props);

            var request = new UpdateTableRequest { Table = tableReply };

            client.UpdateTable(request);
        }

        public void AddAttributeProperty(AttributeProperty attributeProperty, string tableName, string dbName)
        {
            var attributePropertyReply = new AttributePropertyReply {
                Name = attributeProperty.Name,
                AttributeType = (int)attributeProperty.AttributeType
            };

            var request = new AddAttributePropertyRequest { DbName = dbName, TableName = tableName, AttributePropertyReply = attributePropertyReply };

            client.AddAttributeProperty(request);
        }

        public void RemoveAttributePropertyByName(string name, string tableName, string dbName)
        {
            var request = new RemoveAttributePropertyByNameRequest { Name = name, TableName = tableName, DbName = dbName };
            client.RemoveAttributePropertyByName(request);
        }

        public AttributeProperty? GetAttributePropertyByName(string name, string tableName, string dbName)
        {
            var request = new GetAttributePropertyByNameRequest { Name = name, TableName = tableName, DbName = dbName };
            var reply = client.GetAttributePropertyByName(request);
            var attributeProperty = new AttributeProperty
            {
                AttributeType = (AttributeType)reply.AttributeType,
                Name = reply.Name
            };
            return attributeProperty;
        }

        public List<AttributeProperty>? GetAllAttributePropertiesByDbTableName(string tableName, string dbName)
        {
            var request = new GetAllAttributePropertiesByDbTableNameRequest { TableName = tableName, DbName = dbName };
            var reply = client.GetAllAttributePropertiesByDbTableName(request);
            var attributeProperties = reply.AttributeProperties.Select(x => new AttributeProperty
            {
                AttributeType = (AttributeType)x.AttributeType,
                Name = x.Name
            }).ToList();
            return attributeProperties;
        }

        public void UpdateAttributeProperty(AttributeProperty attributeProperty, string tableName, string dbName)
        {
            var attributePropertyReply = new AttributePropertyReply
            {
                Name = attributeProperty.Name,
                AttributeType = (int)attributeProperty.AttributeType
            };

            var request = new UpdateAttributePropertyRequest { DbName = dbName, TableName = tableName, AttributePropertyReply = attributePropertyReply };

            client.UpdateAttributeProperty(request);
        }

        public string? GetDbFileNameByName(string name)
        {
            var request = new GetDbFileNameByNameRequest { Name = name };
            var reply = client.GetDbFileNameByName(request);
            var fileName = reply.StringValue;
            return fileName;
        }

        public string? GetDbFilePath()
        {
            var reply = client.GetDbFilePath(new Google.Protobuf.WellKnownTypes.Empty());
            var filePath = reply.StringValue;
            return filePath;
        }

        public void SaveChanges()
        {
            return;
        }
    }
}

