using System;
using System.Collections.Generic;
using System.IO;
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
            var tables = request.Tables.Select(t => new Table { Guid = Guid.NewGuid(), Name = t.Name, AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
            {
                Guid = Guid.Parse(p.Guid),
                Name = p.Name,
                AttributeType = (AttributeType)p.AttributeType,
                RelationTableGuid = !String.IsNullOrEmpty(p.RelationTableGuid) ? Guid.Parse(p.RelationTableGuid) : null
            }).ToList() }).ToList();
            var database = new DataBase { Guid = Guid.NewGuid(), Name = request.Name, Tables = tables };
            dataBaseSchemaService.AddJsonDbObjectSchema(database);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveJsonDbObjectSchemaByGuid(RemoveJsonDbObjectSchemaByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            dataBaseSchemaService.RemoveJsonDbObjectSchemaByGuid(guid);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<DataBaseReply> GetDbObjectByGuid(GetDbObjectByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var db = dataBaseSchemaService.GetDbObjectByGuid(guid);
            var tables = new List<TableReply>();
            var tableEntities = db.Tables.Select(t => new TableReply { Guid = t.Guid.ToString(), Name = t.Name }).ToList();
            if (tableEntities is not null)
            {
                tables.AddRange(tableEntities);
            }
            foreach (var table in tables)
            {
                var props = new List<AttributePropertyReply>();
                var propsEntities = dataBaseSchemaService.GetAllAttributePropertiesByDbTableGuid(Guid.Parse(table.Guid), guid).Select(prop => new AttributePropertyReply {
                    Guid = prop.Guid.ToString(),
                    Name = prop.Name,
                    RelationTableGuid = prop.RelationTableGuid.ToString(),
                    AttributeType = ((int)prop.AttributeType)
                }).ToList();
                if (propsEntities is not null)
                {
                    props.AddRange(propsEntities);
                }
                table.AttributeProperties.AddRange(props);
            }
            var dataBaseReply = new DataBaseReply { Guid = db.Guid.ToString(), Name = db.Name};
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
                    var tableEntities = db.Tables.Select(t => new TableReply { Guid = t.Guid.ToString(), Name = t.Name }).ToList();
                    if (tableEntities is not null)
                    {
                        tables.AddRange(tableEntities);
                    }
                    foreach (var table in tables)
                    {
                        var props = new List<AttributePropertyReply>();
                        var propsEntities = dataBaseSchemaService.GetAllAttributePropertiesByDbTableGuid(Guid.Parse(table.Guid), db.Guid).Select(prop => new AttributePropertyReply
                        {
                            Guid = prop.Guid.ToString(),
                            Name = prop.Name,
                            RelationTableGuid = prop.RelationTableGuid.ToString(),
                            AttributeType = ((int)prop.AttributeType)
                        }).ToList();
                        if (propsEntities is not null)
                        {
                            props.AddRange(propsEntities);
                        }
                        table.AttributeProperties.AddRange(props);
                    }
                    var dataBaseReply = new DataBaseReply { Guid = db.Guid.ToString(), Name = db.Name };
                    dataBaseReply.Tables.AddRange(tables);
                    listDataBaseReply.DataBases.Add(dataBaseReply);
                }
            }
            return Task.FromResult(listDataBaseReply);
        }

        public override Task<Empty> UpdateJsonDbObjectSchema(UpdateJsonDbObjectSchemaRequest request, ServerCallContext context)
        {
            var tables = request.Tables.Select(t => new Table {
                Guid = Guid.Parse(t.Guid),
                Name = t.Name,
                AttributeProperties = t.AttributeProperties.Select(p => new AttributeProperty
                {
                    Guid = Guid.Parse(p.Guid),
                    Name = p.Name,
                    RelationTableGuid = Guid.Parse(p.RelationTableGuid),
                    AttributeType = (AttributeType)p.AttributeType
                }).ToList()
            }).ToList();
            var database = new DataBase { Guid = Guid.Parse(request.Guid), Name = request.Name, Tables = tables };
            dataBaseSchemaService.UpdateJsonDbObjectSchema(database);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> AddTable(AddTableRequest request, ServerCallContext context)
        {
            var table = new Table { Guid = Guid.NewGuid(), Name = request.Table.Name };
            dataBaseSchemaService.AddTable(table, Guid.Parse(request.DbGuid));
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveTableByGuid(RemoveTableByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var dbGuid = Guid.Parse(request.DbGuid);
            dataBaseSchemaService.RemoveTableByGuid(guid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<TableReply> GetTableByGuid(GetTableByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var table = dataBaseSchemaService.GetTableByGuid(guid, dbGuid);
            var props = new List<AttributePropertyReply>();
            var propsEntities = table.AttributeProperties.Select(prop => new AttributePropertyReply
            {
                Guid = prop.Guid.ToString(),
                Name = prop.Name,
                RelationTableGuid = prop.RelationTableGuid.ToString(),
                AttributeType = ((int)prop.AttributeType)
            }).ToList();
            if (propsEntities is not null)
            {
                props.AddRange(propsEntities);
            }
            var tableReply = new TableReply { Guid = request.Guid, Name = table.Name };
            tableReply.AttributeProperties.AddRange(props);
            return Task.FromResult(tableReply);
        }

        public override Task<ListTableReply> GetAllTablesByDbGuid(GetAllTablesByDbGuidRequest request, ServerCallContext context)
        {
            var dbGuid = Guid.Parse(request.DbGuid);
            var listTableReply = new ListTableReply();
            var tables = dataBaseSchemaService.GetAllTablesByDbGuid(dbGuid);
            if (tables is not null)
            {
                foreach (var table in tables)
                {
                    var props = new List<AttributePropertyReply>();
                    var propsEntities = dataBaseSchemaService.GetAllAttributePropertiesByDbTableGuid(table.Guid, dbGuid).Select(prop => new AttributePropertyReply
                    {
                        Guid = prop.Guid.ToString(),
                        Name = prop.Name,
                        RelationTableGuid = prop.RelationTableGuid.ToString(),
                        AttributeType = ((int)prop.AttributeType)
                    }).ToList();
                    if (propsEntities is not null)
                    {
                        props.AddRange(propsEntities);
                    }
                    var tableReply = new TableReply { Guid = table.Guid.ToString(), Name = table.Name };
                    tableReply.AttributeProperties.AddRange(props);
                    listTableReply.Tables.Add(tableReply);
                }
            }
            return Task.FromResult(listTableReply);
        }

        public override Task<Empty> UpdateTable(UpdateTableRequest request, ServerCallContext context)
        {
            var dbGuid = Guid.Parse(request.DbGuid);
            var props = request.Table.AttributeProperties.Select(p => new AttributeProperty
            {
                Guid = Guid.Parse(p.Guid),
                Name = p.Name,
                RelationTableGuid = Guid.Parse(p.RelationTableGuid),
                AttributeType = (AttributeType)p.AttributeType
            }).ToList();
            var table = new Table { Guid = Guid.Parse(request.Table.Guid), Name = request.Table.Name, AttributeProperties = props };
            dataBaseSchemaService.UpdateTable(table, dbGuid);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> AddAttributeProperty(AddAttributePropertyRequest request, ServerCallContext context)
        {
            var tableGuid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var attributeProperty = new AttributeProperty
            {
                Guid = Guid.NewGuid(),
                Name = request.AttributePropertyReply.Name,
                RelationTableGuid = !String.IsNullOrEmpty(request.AttributePropertyReply.RelationTableGuid) ? Guid.Parse(request.AttributePropertyReply.RelationTableGuid) : null,
                AttributeType = (AttributeType)request.AttributePropertyReply.AttributeType
            };
            dataBaseSchemaService.AddAttributeProperty(attributeProperty, tableGuid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var fields = dataBaseService.GetAllFieldsByTableGuid(tableGuid);
            foreach (var field in fields)
            {
                field.Values.Add(attributeProperty.Guid, "");
            }
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveAttributePropertyByGuid(RemoveAttributePropertyByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var tableGuid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            dataBaseSchemaService.RemoveAttributePropertyByGuid(guid, tableGuid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<AttributePropertyReply> GetAttributePropertyByGuid(GetAttributePropertyByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var tableGuid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var attributeProperty = dataBaseSchemaService.GetAttributePropertyByGuid(guid, tableGuid, dbGuid);
            var attributePropertyReply = new AttributePropertyReply
            {
                Guid = attributeProperty.Guid.ToString(),
                Name = attributeProperty.Name,
                AttributeType = ((int)attributeProperty.AttributeType),
                RelationTableGuid = attributeProperty.RelationTableGuid.ToString()
            };
            return Task.FromResult(attributePropertyReply);
        }

        public override Task<ListAttributePropertyReply> GetAllAttributePropertiesByDbTableGuid(GetAllAttributePropertiesByDbTableGuidRequest request, ServerCallContext context)
        {
            var tableGuid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var listAttributePropertyReply = new ListAttributePropertyReply();
            var attributeProperties = dataBaseSchemaService.GetAllAttributePropertiesByDbTableGuid(tableGuid, dbGuid);
            if (attributeProperties is not null)
            {
                var attributePropertyReplies = attributeProperties.Select(p => new AttributePropertyReply
                {
                    Guid = p.Guid.ToString(),
                    Name = p.Name,
                    RelationTableGuid = p.RelationTableGuid.ToString(),
                    AttributeType = ((int)p.AttributeType)
                });
                listAttributePropertyReply.AttributeProperties.AddRange(attributePropertyReplies);
            }
            return Task.FromResult(listAttributePropertyReply);
        }

        public override Task<Empty> UpdateAttributeProperty(UpdateAttributePropertyRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.AttributePropertyReply.Guid);
            var tableGuid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var attributeProperty = new AttributeProperty
            {
                Guid = guid,
                Name = request.AttributePropertyReply.Name,
                RelationTableGuid = Guid.Parse(request.AttributePropertyReply.RelationTableGuid),
                AttributeType = (AttributeType)request.AttributePropertyReply.AttributeType
            };
            dataBaseSchemaService.UpdateAttributeProperty(attributeProperty, tableGuid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> AddRelation(AddRelationRequest request, ServerCallContext context)
        {
            var tableGuid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var targetTableGuid = Guid.Parse(request.TargetTableGuid);
            var attributeProperty = new AttributeProperty
            {
                Guid = Guid.NewGuid(),
                Name = request.AttributePropertyReply.Name,
                RelationTableGuid = Guid.Parse(request.AttributePropertyReply.RelationTableGuid),
                AttributeType = (AttributeType)request.AttributePropertyReply.AttributeType,
            };
            dataBaseSchemaService.AddRelation(attributeProperty, tableGuid, targetTableGuid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var fields = dataBaseService.GetAllFieldsByTableGuid(tableGuid);
            foreach (var field in fields)
            {
                field.Values.Add(attributeProperty.Guid, "");
            }
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<StringReply> GetDbFileNameByGuid(GetDbFileNameByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(guid);
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
            var dbGuid = Guid.Parse(request.DbGuid);
            var dictValues = new Dictionary<Guid, dynamic>();
            foreach (var value in request.Values)
            {
                dictValues.Add(Guid.Parse(value.Guid), value.Value);
            }
            var tableField = new TableField { Guid = Guid.Parse(request.Guid), TableGuid = Guid.Parse(request.TableGuid), Values = dictValues };
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.AddField(tableField);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveFieldByGuid(RemoveFieldByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.RemoveFieldByGuid(guid);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<TableFieldReply> GetFieldByGuid(GetFieldByGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var tableField = dataBaseService.GetFieldByGuid(guid);
            var tableValues = tableField.Values.Select(v => new ValueReply { Guid = v.Key.ToString(), Value = v.Value.ToString() }).ToList();
            var tableFieldReply = new TableFieldReply { Guid = tableField.Guid.ToString(), TableGuid = tableField.TableGuid.ToString() };
            tableFieldReply.Values.Add(tableValues);
            return Task.FromResult(tableFieldReply);
        }

        public override Task<ListTableFieldReply> GetAllFieldsByTableGuid(GetAllFieldsByTableGuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.TableGuid);
            var dbGuid = Guid.Parse(request.DbGuid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var listTableFieldReply = new ListTableFieldReply();
            var tableFields = dataBaseService.GetAllFieldsByTableGuid(guid);
            if (tableFields is not null)
            {
                foreach (var tableField in tableFields)
                {
                    var values = new List<ValueReply>();
                    var valuesEntities = tableField.Values.Select(v => new ValueReply { Guid = v.Key.ToString(), Value = v.Value.ToString() }).ToList();
                    if (valuesEntities is not null)
                    {
                        values.AddRange(valuesEntities);
                    }
                    var tableFieldReply = new TableFieldReply { Guid = tableField.Guid.ToString(), TableGuid = tableField.TableGuid.ToString() };
                    tableFieldReply.Values.AddRange(values);
                    listTableFieldReply.TableFields.Add(tableFieldReply);
                }
            }
            return Task.FromResult(listTableFieldReply);
        }

        public override Task<Empty> UpdateField(UpdateFieldRequest request, ServerCallContext context)
        {
            var dbGuid = Guid.Parse(request.DbGuid);
            var dictValues = new Dictionary<Guid, dynamic>();
            foreach (var value in request.Values)
            {
                dictValues.Add(Guid.Parse(value.Guid), value.Value);
            }
            var tableField = new TableField { Guid = Guid.Parse(request.Guid), TableGuid = Guid.Parse(request.TableGuid), Values = dictValues };
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.UpdateField(tableField);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> UpdateValue(UpdateValueRequest request, ServerCallContext context)
        {
            var dbGuid = Guid.Parse(request.DbGuid);
            var attributePropertyGuid = Guid.Parse(request.AttributePropertyGuid);
            var fieldGuid = Guid.Parse(request.FieldGuid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.UpdateValue(request.Value, attributePropertyGuid, fieldGuid);
            dataBaseService.SaveChanges();
            return Task.FromResult(new Empty());
        }
    }
}

