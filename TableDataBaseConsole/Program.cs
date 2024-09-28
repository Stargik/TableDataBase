using TableDataBase.Models;
using Grpc.Net.Client;
using TableDataBaseServerService;
using TableDataBase.Interfaces;
using TableDataBase.Services;
using System.Text;
using Newtonsoft.Json;

namespace TableDataBaseConsole;

class Program
{
    static async Task Main(string[] args)
    {
        /*AttributeProperty attributeProperty1 = new AttributeProperty
        {
            Guid = Guid.NewGuid(),
            AttributeType = AttributeType.Integer,
            Name = "Id"
        };
        AttributeProperty attributeProperty2 = new AttributeProperty
        {
            Guid = Guid.NewGuid(),
            AttributeType = AttributeType.String,
            Name = "Name"
        };

        Table table1 = new Table
        {
            Guid = Guid.NewGuid(),
            Name = "table1",
            AttributeProperties = new List<AttributeProperty>
            {
                attributeProperty1, attributeProperty2
            }
        };

        Table table2 = new Table
        {
            Guid = Guid.NewGuid(),
            Name = "table2",
            AttributeProperties = new List<AttributeProperty>
            {
                attributeProperty1, attributeProperty2
            }
        };

        DataBase dataBase = new DataBase
        {
            Guid = Guid.NewGuid(),
            Name = "db1",
            Tables = new List<Table>
            {
                table1, table2
            }
        };

        var values = new Dictionary<Guid, dynamic>();
        values.Add(table2.AttributeProperties[0].Guid, 1);
        values.Add(table2.AttributeProperties[1].Guid, "name");

        var field = new TableField
        {
            Guid = Guid.NewGuid(),
            TableGuid = table2.Guid,
            Values = values
        };

        IDataBaseSchemaService dataBaseSchemaService = new DataBaseSchemaClientService("https://localhost:7099");
        dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
        /*var filePath = "TableServer";
        var fileName = "schema.json";
        IDataBaseSchemaService dataBaseSchemaService = new DataBaseSchemaService(filePath, fileName);
        dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
        dataBaseSchemaService.SaveChanges();
        var prop = dataBaseSchemaService.GetAttributePropertyByGuid(attributeProperty1.Guid, table1.Guid, dataBase.Guid);
        prop.Name = "prop2";
        dataBaseSchemaService.UpdateAttributeProperty(prop, table1.Guid, dataBase.Guid);
        dataBaseSchemaService.SaveChanges();
        Guid guid = dataBase.Guid;
        var dbFileName = dataBaseSchemaService.GetDbFileNameByGuid(dataBase.Guid);
        IDataBaseService dataBaseService = new DataBaseService(filePath, dbFileName);
        dataBaseService.AddField(field);
        dataBaseService.SaveChanges();
        dataBaseSchemaService.RemoveJsonDbObjectSchemaByGuid(dataBase.Guid);
        dataBaseSchemaService.SaveChanges();*/

        /*using var channel = GrpcChannel.ForAddress("https://localhost:7099");
        var client = new TableDataBaseServise.TableDataBaseServiseClient(channel);
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

        client.AddJsonDbObjectSchema(dataBaseRequest);*/

<<<<<<< HEAD
        // HTML-код, который нужно проверить
        string html = "<p>hello</p>";

=======
>>>>>>> 248dfe6 (Removed GUIDs, Added html type)

    }
}

