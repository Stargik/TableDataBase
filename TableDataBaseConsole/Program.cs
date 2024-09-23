using TableDataBase.Models;
using TableDataBase.Interfaces;
using TableDataBase.Services;

namespace TableDataBaseConsole;

class Program
{
    static void Main(string[] args)
    {
        AttributeProperty attributeProperty1 = new AttributeProperty
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

        Table table = new Table
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
                table, table2
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

        var filePath = "TableServer";
        var fileName = "schema.json";
        IDataBaseSchemaService dataBaseSchemaService = new DataBaseSchemaService(filePath, fileName);
        dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
        dataBaseSchemaService.SaveChanges();
        var prop = dataBaseSchemaService.GetAttributePropertyByGuid(attributeProperty1.Guid, table.Guid, dataBase.Guid);
        prop.Name = "prop2";
        dataBaseSchemaService.UpdateAttributeProperty(prop, table.Guid, dataBase.Guid);
        dataBaseSchemaService.SaveChanges();
        Guid guid = dataBase.Guid;
        var dbFileName = dataBaseSchemaService.GetDbFileNameByGuid(dataBase.Guid);
        IDataBaseService dataBaseService = new DataBaseService(filePath, dbFileName);
        dataBaseService.AddField(field);
        dataBaseService.SaveChanges();
        dataBaseSchemaService.RemoveJsonDbObjectSchemaByGuid(dataBase.Guid);
        dataBaseSchemaService.SaveChanges();
    }
}

