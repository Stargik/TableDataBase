using Grpc.Net.Client;
using TableDataBaseServerService;
using TableDataBase.Interfaces;
using TableDataBase.Models;
using Newtonsoft.Json;

namespace TableDataBase.Services
{
	public class DataBaseClientService : IDataBaseService
	{
        private readonly TableDataBaseServise.TableDataBaseServiseClient client;
        private readonly string dbName;

        public DataBaseClientService(string connectionString, string dbName)
		{
            var channel = GrpcChannel.ForAddress(connectionString);
            client = new TableDataBaseServise.TableDataBaseServiseClient(channel);
            this.dbName = dbName;
        }

        public void AddField(TableField tableField)
        {
            var addFieldRequest = new AddFieldRequest
            {
                Guid = tableField.Guid.ToString(),
                TableName = tableField.TableName,
                DbName = dbName
            };
            var values = tableField.Values.Select(x => new ValueReply { Name = x.Key, Value = JsonConvert.SerializeObject(x.Value) });
            addFieldRequest.Values.Add(values);
            client.AddField(addFieldRequest);
        }

        public void RemoveFieldByGuid(Guid guid)
        {
            var request = new RemoveFieldByGuidRequest { Guid = guid.ToString(), DbName = dbName };
            client.RemoveFieldByGuid(request);
        }

        public TableField? GetFieldByGuid(Guid guid)
        {
            var request = new GetFieldByGuidRequest { Guid = guid.ToString(), DbName = dbName };
            var reply = client.GetFieldByGuid(request);
            var dictValues = new Dictionary<string, dynamic>();
            foreach (var value in reply.Values)
            {
                dictValues.Add(value.Name, value.Value);
            }
            var tableField = new TableField
            {
                Guid = Guid.Parse(reply.Guid),
                TableName = reply.TableName,
                Values = dictValues
            };
            return tableField;
        }

        public List<TableField>? GetAllFieldsByTableName(string tableName)
        {
            var request = new GetAllFieldsByTableNameRequest { TableName = tableName, DbName = dbName };
            var reply = client.GetAllFieldsByTableName(request);
            var tableFields = new List<TableField>(); reply.TableFields.Select(t => new TableField
            {
                Guid = Guid.Parse(t.Guid),
                TableName = t.TableName
            });
            foreach (var tableFieldReply in reply.TableFields)
            {
                var dictValues = new Dictionary<string, dynamic>();
                foreach (var value in tableFieldReply.Values)
                {
                    dictValues.Add(value.Name, value.Value);
                }
                var tableField = new TableField
                {
                    Guid = Guid.Parse(tableFieldReply.Guid),
                    TableName = tableFieldReply.TableName,
                    Values = dictValues
                };
                tableFields.Add(tableField);
            }
            return tableFields;
        }

        public void UpdateField(TableField tableField)
        {
            var updateFieldRequest = new UpdateFieldRequest
            {
                Guid = tableField.Guid.ToString(),
                TableName = tableField.TableName,
                DbName = dbName
            };
            var values = tableField.Values.Select(x => new ValueReply { Name = x.Key, Value = JsonConvert.SerializeObject(x.Value) });
            updateFieldRequest.Values.Add(values);
            client.UpdateField(updateFieldRequest);
        }

        public void UpdateValue(dynamic Value, string attributePropertyName, Guid fieldGuid)
        {
            var updateValueRequest = new UpdateValueRequest { Value = Value.ToString(), AttributePropertyName = attributePropertyName, FieldGuid = fieldGuid.ToString(), DbName = dbName };
            client.UpdateValue(updateValueRequest);
        }

        public void SaveChanges()
        {
            return;
        }
    }
}

