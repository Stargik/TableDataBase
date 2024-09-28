using System.Text.RegularExpressions;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using TableDataBase.Interfaces;
using TableDataBase.Models;
using TableDataBase.Services;
using TableDataBaseMVC.Models;
using TableDataBaseMVC.Validators;

namespace TableDataBaseMVC.Controllers
{
	public class DataBasesController : Controller
    {
		private static IDataBaseSchemaService? dataBaseSchemaService;
        private static string? connectionString;
        private readonly bool isLocal;

        public DataBasesController(IConfiguration configuration)
        {
            isLocal = configuration.GetValue<bool>("IsLocal");
        }

        public IActionResult SetConnection()
        {
            return View();
        }

        public IActionResult ResetConnection()
        {
            connectionString = null;
            dataBaseSchemaService = null;
            return RedirectToAction(nameof(SetConnection));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetConnection(ConnectionModel connectionModel)
        {
            connectionString = connectionModel.ConnectionString;
            if (!String.IsNullOrEmpty(connectionString))
            {
                dataBaseSchemaService = new DataBaseSchemaClientService(connectionString);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                return RedirectToAction(nameof(SetConnection));
            }
            var dbs = dataBaseSchemaService.GetAllDbObjects();
            return View(dbs);
        }

        public IActionResult Tables(string name)
        {
            var db = dataBaseSchemaService.GetDbObjectByName(name);
            return View(db);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DataBase dataBase)
        {
            if (ModelState.IsValid)
            {
                if (dataBaseSchemaService.GetAllDbObjects().Select(x => x.Name).Contains(dataBase.Name))
                {
                    return View(dataBase);
                }
                dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
                dataBaseSchemaService.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(dataBase);
        }

        public IActionResult CreateTable(string dbName)
        {
            var tableModel = new TableModel
            {
                DbName = dbName
            };
            return View(tableModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTable(TableModel tableModel)
        {
            if (ModelState.IsValid)
            {
                if (dataBaseSchemaService.GetAllTablesByDbName(tableModel.DbName).Select(x => x.Name).Contains(tableModel.Name))
                {
                    return View(tableModel);
                }
                var table = new Table { Name = tableModel.Name, AttributeProperties = tableModel.AttributeProperties };
                dataBaseSchemaService.AddTable(table, tableModel.DbName);
                dataBaseSchemaService.SaveChanges();
                return RedirectToAction("Tables", new { name = tableModel.DbName });
            }
            return View(tableModel);
        }

        public IActionResult CreateProperty(string tableName, string dbName)
        {
            var attributePropertyModel = new AttributePropertyModel
            {
                DbName = dbName,
                TableName = tableName
            };
            return View(attributePropertyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProperty(AttributePropertyModel attributePropertyModel)
        {
            if (ModelState.IsValid)
            {
                if (dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(attributePropertyModel.TableName, attributePropertyModel.DbName).Select(x => x.Name).Contains(attributePropertyModel.Name))
                {
                    return View(attributePropertyModel);
                }
                var attrProperty = new AttributeProperty { Name = attributePropertyModel.Name, AttributeType = attributePropertyModel.AttributeType };
                dataBaseSchemaService.AddAttributeProperty(attrProperty, attributePropertyModel.TableName, attributePropertyModel.DbName);
                dataBaseSchemaService.SaveChanges();
                return RedirectToAction("EditTable", new { name = attributePropertyModel.TableName, dbName = attributePropertyModel.DbName });
            }
            return View(attributePropertyModel);
        }

        public IActionResult Delete(string name)
        {
            var tableNames = dataBaseSchemaService.GetAllTablesByDbName(name).Select(x => x.Name);
            var dataBaseService = GetDataBaseService(name);
            foreach (var tableName in tableNames)
            {
                var fieldGuids = dataBaseService.GetAllFieldsByTableName(tableName).Select(x => x.Guid);
                foreach (var fieldGuid in fieldGuids)
                {
                    dataBaseService.RemoveFieldByGuid(fieldGuid);
                }
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveJsonDbObjectSchemaByName(name);
            dataBaseSchemaService.SaveChanges();
            var dbs = dataBaseSchemaService.GetAllDbObjects();
            return View("Index", dbs);
        }

        public IActionResult DeleteTable(string name, string dbName)
        {
            var dataBaseService = GetDataBaseService(dbName);
            var fieldGuids = dataBaseService.GetAllFieldsByTableName(name).Select(x => x.Guid);
            foreach (var fieldGuid in fieldGuids)
            {
                dataBaseService.RemoveFieldByGuid(fieldGuid);
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveTableByName(name, dbName);
            dataBaseSchemaService.SaveChanges();
            var db = dataBaseSchemaService.GetDbObjectByName(dbName);
            return View("Tables", db);
        }

        public IActionResult DeleteProperty(string name, string tableName, string dbName)
        {
            var dataBaseService = GetDataBaseService(dbName);
            var fields = dataBaseService.GetAllFieldsByTableName(tableName);
            foreach (var field in fields)
            {
                field.Values.Remove(name);
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveAttributePropertyByName(name, tableName, dbName);
            dataBaseSchemaService.SaveChanges();
            ViewData["DbName"] = dbName;
            var table = dataBaseSchemaService.GetTableByName(tableName, dbName);
            return View("EditTable", table);
        }

        public IActionResult EditTable(string name, string dbName)
        {
            ViewData["DbName"] = dbName;
            var table = dataBaseSchemaService.GetTableByName(name, dbName);
            return View(table);
        }

        public IActionResult Fields(string tableName, string dbName)
        {
            var table = dataBaseSchemaService.GetTableByName(tableName, dbName);
            ViewData["DbName"] = dbName;
            ViewBag.Table = table;
            var filterValues = new Dictionary<string, string[]>();
            foreach (var filter in table.AttributeProperties)
            {
                if (filter.AttributeType == AttributeType.StringInvl)
                {
                    filterValues.Add(filter.Name, Request.Query.ContainsKey(filter.Name) ? new string[] { Request.Query[filter.Name][0].ToString(), Request.Query[filter.Name][1].ToString() } : new string[] { "", "" });
                }
                else
                {
                    filterValues.Add(filter.Name, Request.Query.ContainsKey(filter.Name) ? new string[] { Request.Query[filter.Name].ToString() } : new string[] { "" });
                }
            }
            ViewBag.FilterValues = filterValues;
            var dataBaseService = GetDataBaseService(dbName);
            var fields = dataBaseService.GetAllFieldsByTableName(tableName).Select(x => new TableFieldModel
            {
                Guid = x.Guid,
                TableName = x.TableName,
                DbName = dbName,
                Values = x.Values,
                Columns = table.AttributeProperties
            });
            foreach (var field in fields)
            {
                foreach (var value in field.Values.Where(v => field.Columns.FirstOrDefault(f => f.AttributeType == AttributeType.StringInvl).Name == v.Key))
                {
                    field.Values[value.Key] = JsonConvert.DeserializeObject<StringInvl>(value.Value);
                }
            }
            foreach (var filterValue in filterValues)
            {
                if (filterValue.Value.Count() == 1 && !String.IsNullOrEmpty(filterValue.Value[0]))
                {
                    fields = fields.Where(x => (x.Values.ContainsKey(filterValue.Key) && Regex.IsMatch(x.Values[filterValue.Key].ToString(), filterValue.Value[0]))).ToList();
                }
                else if (filterValue.Value.Count() == 2)
                {
                    if (!String.IsNullOrEmpty(filterValue.Value[0]))
                    {
                        fields = fields.Where(x => (x.Values.ContainsKey(filterValue.Key) && Regex.IsMatch(x.Values[filterValue.Key].Min.ToString(), filterValue.Value[0]))).ToList();
                    }
                    if (!String.IsNullOrEmpty(filterValue.Value[1]))
                    {
                        fields = fields.Where(x => (x.Values.ContainsKey(filterValue.Key) && Regex.IsMatch(x.Values[filterValue.Key].Max.ToString(), filterValue.Value[1]))).ToList();
                    }
                }
            }
            return View(fields);
        }

        public IActionResult CreateField(string tableName, string dbName)
        {
            var table = dataBaseSchemaService.GetTableByName(tableName, dbName);
            var tableFieldModel = new TableFieldModel
            {
                Guid = Guid.NewGuid(),
                DbName = dbName,
                Columns = table.AttributeProperties,
                TableName = tableName,
                Values = new Dictionary<string, dynamic>()
            };
            foreach (var column in tableFieldModel.Columns)
            {
                if(column.AttributeType == AttributeType.StringInvl)
                {
                    tableFieldModel.Values.Add(column.Name, new StringInvl());
                }
                else
                {
                    tableFieldModel.Values.Add(column.Name, "");
                }
            }
            return View(tableFieldModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateField(IFormCollection tableFieldModelCollection)
        {
            var tableName = tableFieldModelCollection["TableName"];
            var dbName = tableFieldModelCollection["DbName"];
            var table = dataBaseSchemaService.GetTableByName(tableName, dbName);
            var dictValues = new Dictionary<string, dynamic>();
            var keys = tableFieldModelCollection.Keys.Where(x => table.AttributeProperties.Select(prop => prop.Name).Contains(x));
            ViewBag.Table = table;
            var tableFieldModel = new TableFieldModel
            {
                Guid = Guid.NewGuid(),
                TableName = tableName,
                DbName = dbName,
                Columns = table.AttributeProperties,
                Values = new Dictionary<string, dynamic>()
            };
            foreach (var key in keys)
            {
                if (tableFieldModel.Columns.FirstOrDefault(x => x.Name == key).AttributeType == AttributeType.StringInvl)
                {
                    tableFieldModel.Values.Add(key, new StringInvl { Max = tableFieldModelCollection[key][0] ?? "", Min = tableFieldModelCollection[key][1] ?? "" } );
                }
                else
                {
                    tableFieldModel.Values.Add(key, tableFieldModelCollection[key].FirstOrDefault() ?? "");
                }
            }
            var htmlValues = tableFieldModel.Values.Where(v => table.AttributeProperties.FirstOrDefault(x => x.Name == v.Key).AttributeType == AttributeType.Html);
            foreach (var html in htmlValues)
            {
                if (!CheckHtml.IsValid(html.Value)) {
                    ViewData[$"Error_{html.Key}"] = $"Html is not valid";
                    return View(tableFieldModel);
                }
            }
            if (ModelState.IsValid)
            {
                var tableField = new TableField { Guid = tableFieldModel.Guid, TableName = tableFieldModel.TableName, Values = tableFieldModel.Values };
                var dataBaseService = GetDataBaseService(dbName);
                dataBaseService.AddField(tableField);
                dataBaseService.SaveChanges();
                return RedirectToAction("Fields", new { tableName, dbName });
            }
            return View(tableFieldModel);
        }

        public IActionResult DeleteField(Guid guid, string tableName, string dbName)
        {
            var dataBaseService = GetDataBaseService(dbName);
            dataBaseService.RemoveFieldByGuid(guid);
            dataBaseService.SaveChanges();
            return RedirectToAction("Fields", new { tableName, dbName });
        }

        public IActionResult EditField(Guid guid, string tableName, string dbName)
        {
            var table = dataBaseSchemaService.GetTableByName(tableName, dbName);
            var dataBaseService = GetDataBaseService(dbName);
            var tableField = dataBaseService.GetFieldByGuid(guid);
            ViewBag.Table = table;
            var tableFieldModel = new TableFieldModel
            {
                Guid = tableField.Guid,
                DbName = dbName,
                TableName = tableName,
                Columns = table.AttributeProperties,
                Values = tableField.Values
            };
            foreach (var value in tableFieldModel.Values.Where(v => tableFieldModel.Columns.FirstOrDefault(f => f.AttributeType == AttributeType.StringInvl).Name == v.Key))
            {
                tableFieldModel.Values[value.Key] = JsonConvert.DeserializeObject<StringInvl>(value.Value);
            }
            return View(tableFieldModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditField(IFormCollection tableFieldModelCollection)
        {
            var tableName = tableFieldModelCollection["TableName"];
            var dbName = tableFieldModelCollection["DbName"];
            var table = dataBaseSchemaService.GetTableByName(tableName, dbName);
            var dictValues = new Dictionary<Guid, dynamic>();
            var keys = tableFieldModelCollection.Keys.Where(x => table.AttributeProperties.Select(prop => prop.Name).Contains(x));
            var tableFieldModel = new TableFieldModel
            {
                Guid = Guid.Parse(tableFieldModelCollection["Guid"]),
                TableName = tableName,
                DbName = dbName,
                Columns = table.AttributeProperties,
                Values = new Dictionary<string, dynamic>()
            };
            foreach (var key in keys)
            {
                if (tableFieldModel.Columns.FirstOrDefault(x => x.Name == key).AttributeType == AttributeType.StringInvl)
                {
                    tableFieldModel.Values.Add(key, new StringInvl { Max = tableFieldModelCollection[key][0] ?? "", Min = tableFieldModelCollection[key][1] ?? "" });
                }
                else
                {
                    tableFieldModel.Values.Add(key, tableFieldModelCollection[key].FirstOrDefault() ?? "");
                }
            }
            var htmlValues = tableFieldModel.Values.Where(v => table.AttributeProperties.FirstOrDefault(x => x.Name == v.Key).AttributeType == AttributeType.Html);
            foreach (var html in htmlValues)
            {
                if (!CheckHtml.IsValid(html.Value))
                {
                    ViewData[$"Error_{html.Key}"] = $"Html is not valid";
                    return View(tableFieldModel);
                }
            }
            if (ModelState.IsValid)
            {
                var tableField = new TableField { Guid = tableFieldModel.Guid, TableName = tableFieldModel.TableName, Values = tableFieldModel.Values };
                var dataBaseService = GetDataBaseService(dbName);
                dataBaseService.UpdateField(tableField);
                dataBaseService.SaveChanges();
                return RedirectToAction("Fields", new { tableName, dbName });
            }
            return View(tableFieldModel);
        }

        private IDataBaseService GetDataBaseService(string dbName)
        {
            if (isLocal)
            {
                var fileName = dataBaseSchemaService.GetDbFileNameByName(dbName);
                var filePath = dataBaseSchemaService.GetDbFilePath();
                var dataBaseService = new DataBaseService(filePath, fileName);
                return dataBaseService;
            }
            else
            {
                var dataBaseService = new DataBaseClientService(connectionString, dbName);
                return dataBaseService;
            }
        }
    }
}

