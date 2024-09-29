using Microsoft.AspNetCore.Mvc;
using Moq;
using TableDataBase.Interfaces;
using TableDataBase.Models;
using TableDataBase.Services;
using TableDataBaseMVC.Controllers;
using TableDataBaseMVC.Models;

namespace TableDatabaseMVC.Tests;

public class DataBasesControllerTests
{
    private Mock<IDataBaseSchemaServiceFactory> dataBaseSchemaServiceFactoryMock;
    private Mock<IDataBaseServiceFactory> dataBaseServiceFactoryMock;
    private Mock<IDataBaseSchemaService> dataBaseSchemaServiceMock;
    private Mock<IDataBaseService> dataBaseServiceMock;
    private DataBasesController dataBasesController;

    [SetUp]
    public void Setup()
    {
        dataBaseSchemaServiceFactoryMock = new Mock<IDataBaseSchemaServiceFactory>();
        dataBaseServiceFactoryMock = new Mock<IDataBaseServiceFactory>();
        dataBaseSchemaServiceMock = new Mock<IDataBaseSchemaService>();
        dataBaseServiceMock = new Mock<IDataBaseService>();
        dataBaseSchemaServiceFactoryMock.Setup(x => x.Create(It.IsAny<string>())).Returns(dataBaseSchemaServiceMock.Object);
        dataBaseServiceFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(dataBaseServiceMock.Object);
        dataBasesController = new DataBasesController(dataBaseSchemaServiceFactoryMock.Object, dataBaseServiceFactoryMock.Object);
        dataBasesController.SetConnection(new ConnectionModel { ConnectionString = "connection" });
    }

    [Theory]
    [TestCaseSource(nameof(filterPatterns))]
    public void Fields_HasFilters_ShouldReturnFilteredFields(Dictionary<string, string[]> patterns)
    {
        //Arrange
        var dbName = "db1";
        var tableName = "table1";
        AttributeProperty attributeProperty1 = new AttributeProperty
        {
            AttributeType = AttributeType.Integer,
            Name = "Id"
        };
        AttributeProperty attributeProperty2 = new AttributeProperty
        {
            AttributeType = AttributeType.String,
            Name = "Name"
        };
        AttributeProperty attributeProperty3 = new AttributeProperty
        {
            AttributeType = AttributeType.StringInvl,
            Name = "Ip"
        };
        Table table1 = new Table
        {
            Name = tableName,
            AttributeProperties = new List<AttributeProperty>
            {
                attributeProperty1, attributeProperty2, attributeProperty3
            }
        };
        var fields = new List<TableField>
        {
            new TableField { Guid = Guid.NewGuid(), TableName = table1.Name, Values =
                {
                    { "Id", 1 },
                    { "Name", "DESCTOP-1" },
                    { "Ip", "{ \"Min\": \"192.164.0.0\", \"Max\": \"192.164.0.100\" }" }
                },
            },
            new TableField { Guid = Guid.NewGuid(), TableName = table1.Name, Values =
                {
                    { "Id", 2 },
                    { "Name", "MOBILE-1" },
                    { "Ip", "{ \"Min\": \"192.164.1.0\", \"Max\": \"192.164.1.90\" }" }
                },
            },
            new TableField { Guid = Guid.NewGuid(), TableName = table1.Name, Values =
                {
                    { "Id", 3},
                    { "Name", "MOBILEPHONE-1" },
                    { "Ip", "{ \"Min\": \"192.164.0.0\", \"Max\": \"192.164.0.1\" }" }
                },
            },
        };
        dataBaseSchemaServiceMock.Setup(x => x.GetTableByName(It.IsAny<string>(), It.IsAny<string>())).Returns(table1);
        dataBaseServiceMock.Setup(x => x.GetAllFieldsByTableName(It.IsAny<string>())).Returns(fields);

        //Act
        var result = dataBasesController.Fields(tableName, dbName, patterns);

        //Assert
        Assert.IsInstanceOf<ViewResult>(result);
        var model = ((ViewResult)result).Model;
        Assert.IsInstanceOf<IEnumerable<TableFieldModel>>(model);
        var tableFieldModels = (IEnumerable<TableFieldModel>)model;
        Assert.AreEqual(2, tableFieldModels.Count());
    }

    [Test]
    public void Fields_HasNotFilters_ShouldReturnAllFields()
    {
        //Arrange
        var dbName = "db1";
        var tableName = "table1";
        AttributeProperty attributeProperty1 = new AttributeProperty
        {
            AttributeType = AttributeType.Integer,
            Name = "Id"
        };
        AttributeProperty attributeProperty2 = new AttributeProperty
        {
            AttributeType = AttributeType.String,
            Name = "Name"
        };
        AttributeProperty attributeProperty3 = new AttributeProperty
        {
            AttributeType = AttributeType.StringInvl,
            Name = "Ip"
        };
        Table table1 = new Table
        {
            Name = tableName,
            AttributeProperties = new List<AttributeProperty>
            {
                attributeProperty1, attributeProperty2, attributeProperty3
            }
        };
        var fields = new List<TableField>
        {
            new TableField { Guid = Guid.NewGuid(), TableName = table1.Name, Values =
                {
                    { "Id", 1 },
                    { "Name", "DESCTOP-1" },
                    { "Ip", "{ \"Min\": \"192.164.0.0\", \"Max\": \"192.164.0.100\" }" }
                },
            },
            new TableField { Guid = Guid.NewGuid(), TableName = table1.Name, Values =
                {
                    { "Id", 2 },
                    { "Name", "MOBILE-1" },
                    { "Ip", "{ \"Min\": \"192.164.1.0\", \"Max\": \"192.164.1.90\" }" }
                },
            },
            new TableField { Guid = Guid.NewGuid(), TableName = table1.Name, Values =
                {
                    { "Id", 3},
                    { "Name", "MOBILEPHONE-1" },
                    { "Ip", "{ \"Min\": \"192.164.0.0\", \"Max\": \"192.164.0.1\" }" }
                },
            },
        };
        dataBaseSchemaServiceMock.Setup(x => x.GetTableByName(It.IsAny<string>(), It.IsAny<string>())).Returns(table1);
        dataBaseServiceMock.Setup(x => x.GetAllFieldsByTableName(It.IsAny<string>())).Returns(fields);

        //Act
        var result = dataBasesController.Fields(tableName, dbName, new Dictionary<string, string[]>());

        //Assert
        Assert.IsInstanceOf<ViewResult>(result);
        var model = ((ViewResult)result).Model;
        Assert.IsInstanceOf<IEnumerable<TableFieldModel>>(model);
        var tableFieldModels = (IEnumerable<TableFieldModel>)model;
        Assert.AreEqual(fields.Count(), tableFieldModels.Count());
    }

    [Test]
    public void Tables_ShouldReturnDatabaseSchemaObject()
    {
        //Arrange
        var dbName = "db1";
        var tableName = "table1";
        AttributeProperty attributeProperty1 = new AttributeProperty
        {
            AttributeType = AttributeType.Integer,
            Name = "Id"
        };
        AttributeProperty attributeProperty2 = new AttributeProperty
        {
            AttributeType = AttributeType.String,
            Name = "Name"
        };
        AttributeProperty attributeProperty3 = new AttributeProperty
        {
            AttributeType = AttributeType.StringInvl,
            Name = "Ip"
        };
        Table table1 = new Table
        {
            Name = tableName,
            AttributeProperties = new List<AttributeProperty>
            {
                attributeProperty1, attributeProperty2, attributeProperty3
            }
        };
        DataBase dataBase = new DataBase
        {
            Name = dbName,
            Tables = new List<Table>
            {
                table1
            }
        };
        dataBaseSchemaServiceMock.Setup(x => x.GetDbObjectByName(It.IsAny<string>())).Returns(dataBase);

        //Act
        var result = dataBasesController.Tables(dbName);

        //Assert
        Assert.IsInstanceOf<ViewResult>(result);
        var model = ((ViewResult)result).Model;
        Assert.IsInstanceOf<DataBase>(model);
        var dataBaseResult = (DataBase)model;
        dataBaseSchemaServiceMock.Verify(x => x.GetDbObjectByName(dbName), Times.Once);
        Assert.AreEqual(dataBase.Name, dataBaseResult.Name);
        Assert.AreEqual(dataBase.Tables.Count(), dataBaseResult.Tables.Count());
    }

    [Test]
    public void Index_ConnectionIsNotExist_ShouldRedirectToSetConnection()
    {
        //Arrange
        dataBasesController.ResetConnection();

        //Act
        var result = dataBasesController.Index();

        //Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var actionName = ((RedirectToActionResult)result).ActionName;
        Assert.AreEqual("SetConnection", actionName);
    }

    public static object[] filterPatterns =
    {
        new Dictionary<string, string[]>
        {
            { "Id", new string[] {"[1-9]+"} },
            { "Name", new string[] { "MOBILE(PHONE)?-[0-9]+" } },
            { "Ip", new string[] { "192\\.164\\.[01]\\.0" , "192\\.164\\.[01]\\.[0-9]{1,2}" } },
        },
        new Dictionary<string, string[]>
        {
            { "Id", new string[] {"[1-9]+"} },
            { "Name", new string[] { "MOBILE(PHONE)?-[0-9]+" } },
            { "Ip", new string[] { "" , "192\\.164\\.[01]\\.[0-9]{1,2}" } },
        },
        new Dictionary<string, string[]>
        {
            { "Id", new string[] {"[1-9]+"} },
            { "Name", new string[] { "MOBILE(PHONE)?-[0-9]+" } },
            { "Ip", new string[] { "192\\.164\\.[01]\\.0", null } },
        },
    };
}
