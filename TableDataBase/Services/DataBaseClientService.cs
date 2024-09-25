using Grpc.Net.Client;
using TableDataBaseServerService;
using TableDataBase.Interfaces;
using TableDataBase.Models;

namespace TableDataBase.Services
{
	public class DataBaseClientService : IDataBaseService
	{
        private readonly TableDataBaseServise.TableDataBaseServiseClient client;
        private readonly string dbGuid;

        public DataBaseClientService(string connectionString, string dbGuid)
		{
            var channel = GrpcChannel.ForAddress(connectionString);
            client = new TableDataBaseServise.TableDataBaseServiseClient(channel);
            this.dbGuid = dbGuid;
        }

        public void AddField(TableField tableField)
        {
            var addFieldRequest = new AddFieldRequest
            {
                Guid = tableField.Guid.ToString(),
                TableGuid = tableField.TableGuid.ToString(),
                DbGuid = dbGuid
            };
            var values = tableField.Values.Select(x => new ValueReply { Guid = x.Key.ToString(), Value = x.Value.ToString() });
            addFieldRequest.Values.Add(values);
            client.AddField(addFieldRequest);
        }

        public void RemoveFieldByGuid(Guid guid)
        {
            var request = new RemoveFieldByGuidRequest { Guid = guid.ToString(), DbGuid = dbGuid };
            client.RemoveFieldByGuid(request);
        }

        public TableField? GetFieldByGuid(Guid guid)
        {
            var request = new GetFieldByGuidRequest { Guid = guid.ToString(), DbGuid = dbGuid };
            var reply = client.GetFieldByGuid(request);
            var dictValues = new Dictionary<Guid, dynamic>();
            foreach (var value in reply.Values)
            {
                dictValues.Add(Guid.Parse(value.Guid), value.Value);
            }
            var tableField = new TableField
            {
                Guid = Guid.Parse(reply.Guid),
                TableGuid = Guid.Parse(reply.TableGuid),
                Values = dictValues
            };
            return tableField;
        }

        public List<TableField>? GetAllFieldsByTableGuid(Guid tableGuid)
        {
            var request = new GetAllFieldsByTableGuidRequest { TableGuid = tableGuid.ToString(), DbGuid = dbGuid };
            var reply = client.GetAllFieldsByTableGuid(request);
            var tableFields = new List<TableField>(); reply.TableFields.Select(t => new TableField
            {
                Guid = Guid.Parse(t.Guid),
                TableGuid = Guid.Parse(t.TableGuid)
            });
            foreach (var tableFieldReply in reply.TableFields)
            {
                var dictValues = new Dictionary<Guid, dynamic>();
                foreach (var value in tableFieldReply.Values)
                {
                    dictValues.Add(Guid.Parse(value.Guid), value.Value);
                }
                var tableField = new TableField
                {
                    Guid = Guid.Parse(tableFieldReply.Guid),
                    TableGuid = Guid.Parse(tableFieldReply.TableGuid),
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
                TableGuid = tableField.TableGuid.ToString(),
                DbGuid = dbGuid
            };
            var values = tableField.Values.Select(x => new ValueReply { Guid = x.Key.ToString(), Value = x.Value.ToString() });
            updateFieldRequest.Values.Add(values);
            client.UpdateField(updateFieldRequest);
        }

        public void UpdateValue(dynamic Value, Guid attributePropertyGuid, Guid fieldGuid)
        {
            var updateValueRequest = new UpdateValueRequest { Value = Value.ToString(), AttributePropertyGuid = attributePropertyGuid.ToString(), FieldGuid = fieldGuid.ToString(), DbGuid = dbGuid };
            client.UpdateValue(updateValueRequest);
        }

        public void SaveChanges()
        {
            return;
        }
    }
}

