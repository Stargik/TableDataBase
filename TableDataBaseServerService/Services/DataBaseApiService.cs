using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TableDataBase.Interfaces;
using TableDataBase.Models;
using TableDataBase.Services;

namespace TableDataBaseServerService.Services
{
	public class DataBaseApiService : TableDataBaseServise.TableDataBaseServiseBase
    {
        private readonly IDataBaseSchemaService dataBaseSchemaService;

        public DataBaseApiService(IDataBaseSchemaService dataBaseSchemaService)
		{
            this.dataBaseSchemaService = dataBaseSchemaService;
        }

        public override Task<Empty> AddJsonDbObjectSchema(AddJsonDbObjectSchemaRequest request, ServerCallContext context)
        {
            var tables = request.Tables.Select(t => new Table { Name = t.Name, AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
            {
                Name = p.Name,
                AttributeType = (AttributeType)p.AttributeType
            }).ToList() }).ToList();
            var database = new DataBase { Name = request.Name, Tables = tables };
            dataBaseSchemaService.AddJsonDbObjectSchema(database);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveJsonDbObjectSchemaByName(RemoveJsonDbObjectSchemaByNameRequest request, ServerCallContext context)
        {
            dataBaseSchemaService.RemoveJsonDbObjectSchemaByName(request.Name);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<DataBaseReply> GetDbObjectByName(GetDbObjectByNameRequest request, ServerCallContext context)
        {
            var db = dataBaseSchemaService.GetDbObjectByName(request.Name);
            var tables = new List<TableReply>();
            var tableEntities = db.Tables.Select(t => new TableReply { Name = t.Name }).ToList();
            if (tableEntities is not null)
            {
                tables.AddRange(tableEntities);
            }
            foreach (var table in tables)
            {
                var props = new List<AttributePropertyReply>();
                var propsEntities = dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(table.Name, request.Name).Select(prop => new AttributePropertyReply {
                    Name = prop.Name,
                    AttributeType = ((int)prop.AttributeType)
                }).ToList();
                if (propsEntities is not null)
                {
                    props.AddRange(propsEntities);
                }
                table.AttributeProperties.AddRange(props);
            }
            var dataBaseReply = new DataBaseReply { Name = db.Name };
            dataBaseReply.Tables.AddRange(tables);
            return Task.FromResult(dataBaseReply);
        }

        public override Task<ListDataBaseReply> GetAllDbObjects(Empty request, ServerCallContext context)
        {
            var listDataBaseReply = new ListDataBaseReply();
            var databases = dataBaseSchemaService.GetAllDbObjects();
            if (databases is not null)
            {
                foreach (var db in databases)
                {
                    var tables = new List<TableReply>();
                    var tableEntities = db.Tables.Select(t => new TableReply { Name = t.Name }).ToList();
                    if (tableEntities is not null)
                    {
                        tables.AddRange(tableEntities);
                    }
                    foreach (var table in tables)
                    {
                        var props = new List<AttributePropertyReply>();
                        var propsEntities = dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(table.Name, db.Name).Select(prop => new AttributePropertyReply
                        {
                            Name = prop.Name,
                            AttributeType = ((int)prop.AttributeType)
                        }).ToList();
                        if (propsEntities is not null)
                        {
                            props.AddRange(propsEntities);
                        }
                        table.AttributeProperties.AddRange(props);
                    }
                    var dataBaseReply = new DataBaseReply { Name = db.Name };
                    dataBaseReply.Tables.AddRange(tables);
                    listDataBaseReply.DataBases.Add(dataBaseReply);
                }
            }
            return Task.FromResult(listDataBaseReply);
        }

        public override Task<Empty> UpdateJsonDbObjectSchema(UpdateJsonDbObjectSchemaRequest request, ServerCallContext context)
        {
            var tables = request.Tables.Select(t => new Table {
                Name = t.Name,
                AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                {
                    Name = p.Name,
                    AttributeType = (AttributeType)p.AttributeType
                }).ToList()
            }).ToList();
            var database = new DataBase { Name = request.Name, Tables = tables };
            dataBaseSchemaService.UpdateJsonDbObjectSchema(database);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> AddTable(AddTableRequest request, ServerCallContext context)
        {
            var table = new Table { Name = request.Table.Name };
            dataBaseSchemaService.AddTable(table, request.DbName);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveTableByName(RemoveTableByNameRequest request, ServerCallContext context)
        {
            dataBaseSchemaService.RemoveTableByName(request.Name, request.DbName);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<TableReply> GetTableByName(GetTableByNameRequest request, ServerCallContext context)
        {
            var table = dataBaseSchemaService.GetTableByName(request.Name, request.DbName);
            var props = new List<AttributePropertyReply>();
            var propsEntities = table.AttributeProperties.Select(prop => new AttributePropertyReply
            {
                Name = prop.Name,
                AttributeType = ((int)prop.AttributeType)
            }).ToList();
            if (propsEntities is not null)
            {
                props.AddRange(propsEntities);
            }
            var tableReply = new TableReply { Name = table.Name };
            tableReply.AttributeProperties.AddRange(props);
            return Task.FromResult(tableReply);
        }

        public override Task<ListTableReply> GetAllTablesByDbName(GetAllTablesByDbNameRequest request, ServerCallContext context)
        {
            var listTableReply = new ListTableReply();
            var tables = dataBaseSchemaService.GetAllTablesByDbName(request.DbName);
            if (tables is not null)
            {
                foreach (var table in tables)
                {
                    var props = new List<AttributePropertyReply>();
                    var propsEntities = dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(table.Name, request.DbName).Select(prop => new AttributePropertyReply
                    {
                        Name = prop.Name,
                        AttributeType = ((int)prop.AttributeType)
                    }).ToList();
                    if (propsEntities is not null)
                    {
                        props.AddRange(propsEntities);
                    }
                    var tableReply = new TableReply { Name = table.Name };
                    tableReply.AttributeProperties.AddRange(props);
                    listTableReply.Tables.Add(tableReply);
                }
            }
            return Task.FromResult(listTableReply);
        }

        public override Task<Empty> UpdateTable(UpdateTableRequest request, ServerCallContext context)
        {
            var props = request.Table.AttributeProperties.Select(p => new AttributeProperty
            {
                Name = p.Name,
                AttributeType = (AttributeType)p.AttributeType
            }).ToList();
            var table = new Table { Name = request.Table.Name, AttributeProperties = props };
            dataBaseSchemaService.UpdateTable(table, request.DbName);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> AddAttributeProperty(AddAttributePropertyRequest request, ServerCallContext context)
        {
            var attributeProperty = new AttributeProperty
            {
                Name = request.AttributePropertyReply.Name,
                AttributeType = (AttributeType)request.AttributePropertyReply.AttributeType
            };
            dataBaseSchemaService.AddAttributeProperty(attributeProperty, request.TableName, request.DbName);
            dataBaseSchemaService.SaveChanges();
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var fields = dataBaseService.GetAllFieldsByTableName(request.TableName);
            foreach (var field in fields)
            {
                if((AttributeType)request.AttributePropertyReply.AttributeType == AttributeType.StringInvl)
                {
                    field.Values.Add(attributeProperty.Name, new StringInvl());
                }
                else
                {
                    field.Values.Add(attributeProperty.Name, "");
                }
            }
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveAttributePropertyByName(RemoveAttributePropertyByNameRequest request, ServerCallContext context)
        {
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var fields = dataBaseService.GetAllFieldsByTableName(request.TableName);
            foreach (var field in fields)
            {
                field.Values.Remove(request.Name);
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveAttributePropertyByName(request.Name, request.TableName, request.DbName);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<AttributePropertyReply> GetAttributePropertyByName(GetAttributePropertyByNameRequest request, ServerCallContext context)
        {
            var attributeProperty = dataBaseSchemaService.GetAttributePropertyByName(request.Name, request.TableName, request.DbName);
            var attributePropertyReply = new AttributePropertyReply
            {
                Name = attributeProperty.Name,
                AttributeType = ((int)attributeProperty.AttributeType)
            };
            return Task.FromResult(attributePropertyReply);
        }

        public override Task<ListAttributePropertyReply> GetAllAttributePropertiesByDbTableName(GetAllAttributePropertiesByDbTableNameRequest request, ServerCallContext context)
        {
            var listAttributePropertyReply = new ListAttributePropertyReply();
            var attributeProperties = dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(request.TableName, request.DbName);
            if (attributeProperties is not null)
            {
                var attributePropertyReplies = attributeProperties.Select(p => new AttributePropertyReply
                {
                    Name = p.Name,
                    AttributeType = ((int)p.AttributeType)
                });
                listAttributePropertyReply.AttributeProperties.AddRange(attributePropertyReplies);
            }
            return Task.FromResult(listAttributePropertyReply);
        }

        public override Task<Empty> UpdateAttributeProperty(UpdateAttributePropertyRequest request, ServerCallContext context)
        {
            var attributeProperty = new AttributeProperty
            {
                Name = request.AttributePropertyReply.Name,
                AttributeType = (AttributeType)request.AttributePropertyReply.AttributeType
            };
            dataBaseSchemaService.UpdateAttributeProperty(attributeProperty, request.TableName, request.DbName);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<StringReply> GetDbFileNameByName(GetDbFileNameByNameRequest request, ServerCallContext context)
        {
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.Name);
            var reply = new StringReply { StringValue = fileName };
            return Task.FromResult(reply);
        }

        public override Task<StringReply> GetDbFilePath(Empty request, ServerCallContext context)
        {
            var path = dataBaseSchemaService.GetDbFilePath();
            var reply = new StringReply { StringValue = path };
            return Task.FromResult(reply);
        }

        public override Task<Empty> AddField(AddFieldRequest request, ServerCallContext context)
        {
            var dictValues = new Dictionary<string, dynamic>();
            foreach (var value in request.Values)
            {
                if (dataBaseSchemaService.GetAttributePropertyByName(value.Name, request.TableName, request.DbName).AttributeType == AttributeType.StringInvl)
                {
                    var insertValue = JsonConvert.DeserializeObject<StringInvl>(value.Value);
                    dictValues.Add(value.Name, insertValue ?? new StringInvl());
                }
                else
                {
                    dictValues.Add(value.Name, JsonConvert.DeserializeObject<string>(value.Value));
                }
            }
            var tableField = new TableField { Guid = Guid.Parse(request.Guid), TableName = request.TableName, Values = dictValues };
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.AddField(tableField);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveFieldByGuid(RemoveFieldByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.RemoveFieldByGuid(guid);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<TableFieldReply> GetFieldByGuid(GetFieldByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var tableField = dataBaseService.GetFieldByGuid(guid);
            var tableValues = tableField.Values.Select(v => new ValueReply { Name = v.Key, Value = v.Value.ToString() }).ToList();
            var tableFieldReply = new TableFieldReply { Guid = tableField.Guid.ToString(), TableName = tableField.TableName };
            tableFieldReply.Values.Add(tableValues);
            return Task.FromResult(tableFieldReply);
        }

        public override Task<ListTableFieldReply> GetAllFieldsByTableName(GetAllFieldsByTableNameRequest request, ServerCallContext context)
        {
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var listTableFieldReply = new ListTableFieldReply();
            var tableFields = dataBaseService.GetAllFieldsByTableName(request.TableName);
            if (tableFields is not null)
            {
                foreach (var tableField in tableFields)
                {
                    var values = new List<ValueReply>();
                    var valuesEntities = tableField.Values.Select(v => new ValueReply { Name = v.Key, Value = v.Value.ToString() }).ToList();
                    if (valuesEntities is not null)
                    {
                        values.AddRange(valuesEntities);
                    }
                    var tableFieldReply = new TableFieldReply { Guid = tableField.Guid.ToString(), TableName = tableField.TableName };
                    tableFieldReply.Values.AddRange(values);
                    listTableFieldReply.TableFields.Add(tableFieldReply);
                }
            }
            return Task.FromResult(listTableFieldReply);
        }

        public override Task<Empty> UpdateField(UpdateFieldRequest request, ServerCallContext context)
        {
            var dictValues = new Dictionary<string, dynamic>();
            foreach (var value in request.Values)
            {
                if (dataBaseSchemaService.GetAttributePropertyByName(value.Name, request.TableName, request.DbName).AttributeType == AttributeType.StringInvl)
                {
                    var insertValue = JsonConvert.DeserializeObject<StringInvl>(value.Value);
                    dictValues.Add(value.Name, insertValue ?? new StringInvl());
                }
                else
                {
                    dictValues.Add(value.Name, JsonConvert.DeserializeObject<string>(value.Value));
                }
            }
            var tableField = new TableField { Guid = Guid.Parse(request.Guid), TableName = request.TableName, Values = dictValues };
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.UpdateField(tableField);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> UpdateValue(UpdateValueRequest request, ServerCallContext context)
        {
            var fieldGuid = Guid.Parse(request.FieldGuid);
            var fileName = dataBaseSchemaService.GetDbFileNameByName(request.DbName);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.UpdateValue(request.Value, request.AttributePropertyName, fieldGuid);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }
    }
}

