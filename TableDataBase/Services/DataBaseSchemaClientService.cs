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
                Guid = dataBase.Guid.ToString(),
                Name = dataBase.Name,
            };

            var tables = dataBase.Tables.Select(t => new TableReply
            {
                Guid = t.Guid.ToString(),
                Name = t.Name
            }).ToList();

            foreach (var table in tables)
            {
                var props = dataBase.Tables.FirstOrDefault(x => x.Guid == Guid.Parse(table.Guid)).AttributeProperties.Select(p => new AttributePropertyReply
                {
                    Guid = p.Guid.ToString(),
                    Name = p.Name,
                    AttributeType = (int)p.AttributeType,
                    RelationTableGuid = p.RelationTableGuid.ToString()
                }).ToList();
                table.AttributeProperties.AddRange(props);
            }

            dataBaseRequest.Tables.AddRange(tables);

            client.AddJsonDbObjectSchema(dataBaseRequest);
        }

        public void RemoveJsonDbObjectSchemaByGuid(Guid guid)
        {
            var request = new RemoveJsonDbObjectSchemaByGuidRequest { Guid = guid.ToString() };
            client.RemoveJsonDbObjectSchemaByGuid(request);
        }

        public DataBase? GetDbObjectByGuid(Guid guid)
        {
            var request = new GetDbObjectByGuidRequest { Guid = guid.ToString() };
            var reply = client.GetDbObjectByGuid(request);
            var dataBase = new DataBase
            {
                Guid = Guid.Parse(reply.Guid),
                Name = reply.Name,
                Tables = reply.Tables.Select(t => new Table
                {
                    Guid = Guid.Parse(t.Guid),
                    Name = t.Name,
                    AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                    {
                        Guid = Guid.Parse(p.Guid),
                        AttributeType = (AttributeType)p.AttributeType,
                        Name = p.Name,
                        RelationTableGuid = !String.IsNullOrEmpty(p.RelationTableGuid) ? Guid.Parse(p.RelationTableGuid) : null
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
                Guid = Guid.Parse(x.Guid),
                Name = x.Name,
                Tables = x.Tables.Select(t => new Table
                {
                    Guid = Guid.Parse(t.Guid),
                    Name = t.Name,
                    AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                    {
                        Guid = Guid.Parse(p.Guid),
                        AttributeType = (AttributeType)p.AttributeType,
                        Name = p.Name,
                        RelationTableGuid = !String.IsNullOrEmpty(p.RelationTableGuid) ? Guid.Parse(p.RelationTableGuid) : null
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
                Guid = dataBase.Guid.ToString(),
                Name = dataBase.Name,
            };

            var tables = dataBase.Tables.Select(t => new TableReply
            {
                Guid = t.Guid.ToString(),
                Name = t.Name
            }).ToList();

            foreach (var table in tables)
            {
                var props = dataBase.Tables.FirstOrDefault(x => x.Guid == Guid.Parse(table.Guid)).AttributeProperties.Select(p => new AttributePropertyReply
                {
                    Guid = p.Guid.ToString(),
                    Name = p.Name,
                    AttributeType = (int)p.AttributeType,
                    RelationTableGuid = p.RelationTableGuid.ToString()
                }).ToList();
                table.AttributeProperties.AddRange(props);
            }

            dataBaseRequest.Tables.AddRange(tables);

            client.UpdateJsonDbObjectSchema(dataBaseRequest);
        }

        public void AddTable(Table table, Guid dbGuid)
        {
            var tableReply = new TableReply { Guid = table.Guid.ToString(), Name = table.Name };
            var props = table.AttributeProperties.Select(p => new AttributePropertyReply
            {
                Guid = p.Guid.ToString(),
                Name = p.Name,
                AttributeType = (int)p.AttributeType,
                RelationTableGuid = p.RelationTableGuid.ToString()
            }).ToList();
            tableReply.AttributeProperties.AddRange(props);

            var request = new AddTableRequest { DbGuid = dbGuid.ToString(), Table = tableReply };

            client.AddTable(request);
        }

        public void RemoveTableByGuid(Guid guid, Guid dbGuid)
        {
            var request = new RemoveTableByGuidRequest { Guid = guid.ToString(), DbGuid = dbGuid.ToString() };
            client.RemoveTableByGuid(request);
        }

        public Table? GetTableByGuid(Guid guid, Guid dbGuid)
        {
            var request = new GetTableByGuidRequest { Guid = guid.ToString(), DbGuid = dbGuid.ToString() };
            var reply = client.GetTableByGuid(request);
            var table = new Table
            {
                Guid = Guid.Parse(reply.Guid),
                Name = reply.Name,
                AttributeProperties = reply.AttributeProperties.Select(p => new AttributeProperty
                {
                    Guid = Guid.Parse(p.Guid),
                    AttributeType = (AttributeType)p.AttributeType,
                    Name = p.Name,
                    RelationTableGuid = !String.IsNullOrEmpty(p.RelationTableGuid) ? Guid.Parse(p.RelationTableGuid) : null
                }).ToList()
            };
            return table;
        }

        public List<Table>? GetAllTablesByDbGuid(Guid dbGuid)
        {
            var request = new GetAllTablesByDbGuidRequest { DbGuid = dbGuid.ToString() };
            var reply = client.GetAllTablesByDbGuid(request);
            var tables = reply.Tables.Select(t => new Table
            {
                Guid = Guid.Parse(t.Guid),
                Name = t.Name,
                AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                {
                    Guid = Guid.Parse(p.Guid),
                    AttributeType = (AttributeType)p.AttributeType,
                    Name = p.Name,
                    RelationTableGuid = !String.IsNullOrEmpty(p.RelationTableGuid) ? Guid.Parse(p.RelationTableGuid) : null
                }).ToList()
            }).ToList();
            return tables;
        }

        public void UpdateTable(Table table, Guid dbGuid)
        {
            var tableReply = new TableReply { Guid = table.Guid.ToString(), Name = table.Name };
            var props = table.AttributeProperties.Select(p => new AttributePropertyReply
            {
                Guid = p.Guid.ToString(),
                Name = p.Name,
                AttributeType = (int)p.AttributeType,
                RelationTableGuid = p.RelationTableGuid.ToString()
            }).ToList();
            tableReply.AttributeProperties.AddRange(props);

            var request = new UpdateTableRequest { DbGuid = dbGuid.ToString(), Table = tableReply };

            client.UpdateTable(request);
        }

        public void AddAttributeProperty(AttributeProperty attributeProperty, Guid tableGuid, Guid dbGuid)
        {
            var attributePropertyReply = new AttributePropertyReply {
                Guid = attributeProperty.Guid.ToString(),
                Name = attributeProperty.Name,
                AttributeType = (int)attributeProperty.AttributeType,
                RelationTableGuid = attributeProperty.RelationTableGuid.ToString()
            };

            var request = new AddAttributePropertyRequest { DbGuid = dbGuid.ToString(), TableGuid = tableGuid.ToString(), AttributePropertyReply = attributePropertyReply };

            client.AddAttributeProperty(request);
        }

        public void RemoveAttributePropertyByGuid(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var request = new RemoveAttributePropertyByGuidRequest { Guid = guid.ToString(), TableGuid = tableGuid.ToString(), DbGuid = dbGuid.ToString() };
            client.RemoveAttributePropertyByGuid(request);
        }

        public AttributeProperty? GetAttributePropertyByGuid(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var request = new GetAttributePropertyByGuidRequest { Guid = guid.ToString(), TableGuid = tableGuid.ToString(), DbGuid = dbGuid.ToString() };
            var reply = client.GetAttributePropertyByGuid(request);
            var attributeProperty = new AttributeProperty
            {
                Guid = Guid.Parse(reply.Guid),
                AttributeType = (AttributeType)reply.AttributeType,
                Name = reply.Name,
                RelationTableGuid = !String.IsNullOrEmpty(reply.RelationTableGuid) ? Guid.Parse(reply.RelationTableGuid) : null
            };
            return attributeProperty;
        }

        public List<AttributeProperty>? GetAllAttributePropertiesByDbTableGuid(Guid tableGuid, Guid dbGuid)
        {
            var request = new GetAllAttributePropertiesByDbTableGuidRequest { TableGuid = tableGuid.ToString(), DbGuid = dbGuid.ToString() };
            var reply = client.GetAllAttributePropertiesByDbTableGuid(request);
            var attributeProperties = reply.AttributeProperties.Select(x => new AttributeProperty
            {
                Guid = Guid.Parse(x.Guid),
                AttributeType = (AttributeType)x.AttributeType,
                Name = x.Name,
                RelationTableGuid = !String.IsNullOrEmpty(x.RelationTableGuid) ? Guid.Parse(x.RelationTableGuid) : null
            }).ToList();
            return attributeProperties;
        }

        public void UpdateAttributeProperty(AttributeProperty attributeProperty, Guid tableGuid, Guid dbGuid)
        {
            var attributePropertyReply = new AttributePropertyReply
            {
                Guid = attributeProperty.Guid.ToString(),
                Name = attributeProperty.Name,
                AttributeType = (int)attributeProperty.AttributeType,
                RelationTableGuid = attributeProperty.RelationTableGuid.ToString()
            };

            var request = new UpdateAttributePropertyRequest { DbGuid = dbGuid.ToString(), TableGuid = tableGuid.ToString(), AttributePropertyReply = attributePropertyReply };

            client.UpdateAttributeProperty(request);
        }

        public void AddRelation(AttributeProperty attributeProperty, Guid tableGuid, Guid targetTableGuid, Guid dbGuid)
        {
            var attributePropertyReply = new AttributePropertyReply
            {
                Guid = attributeProperty.Guid.ToString(),
                Name = attributeProperty.Name,
                AttributeType = (int)attributeProperty.AttributeType,
                RelationTableGuid = attributeProperty.RelationTableGuid.ToString()
            };

            var request = new AddRelationRequest { DbGuid = dbGuid.ToString(), TableGuid = tableGuid.ToString(), TargetTableGuid = targetTableGuid.ToString(), AttributePropertyReply = attributePropertyReply };

            client.AddRelation(request);
        }

        public string? GetDbFileNameByGuid(Guid guid)
        {
            var request = new GetDbFileNameByGuidRequest { Guid = guid.ToString() };
            var reply = client.GetDbFileNameByGuid(request);
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

