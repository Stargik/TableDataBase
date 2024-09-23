using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Syncfusion.EJ2.Grids;
using TableDataBase.Interfaces;
using TableDataBase.Models;
using TableDataBase.Services;
using TableDataBaseMVC.Models;

namespace TableDataBaseMVC.Controllers
{
	public class DataBasesController : Controller
    {
		public readonly IDataBaseSchemaService dataBaseSchemaService;

        public DataBasesController(IDataBaseSchemaService dataBaseSchemaService)
        {
            this.dataBaseSchemaService = dataBaseSchemaService;
        }

        public IActionResult Index()
        {
            var dbs = dataBaseSchemaService.GetAllDbObjects();
            return View(dbs);
        }

        public IActionResult Tables(Guid guid)
        {
            var db = dataBaseSchemaService.GetDbObjectByGuid(guid);
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
                dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
                dataBaseSchemaService.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(dataBase);
        }

        public IActionResult CreateTable(Guid dbGuid)
        {
            ViewData["DbGuid"] = dbGuid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTable(TableModel tableModel)
        {
            if (ModelState.IsValid)
            {
                var table = new Table { Name = tableModel.Name, Guid = Guid.NewGuid(), AttributeProperties = tableModel.AttributeProperties };
                dataBaseSchemaService.AddTable(table, tableModel.DbGuid);
                dataBaseSchemaService.SaveChanges();
                return RedirectToAction("Tables", new { guid = tableModel.DbGuid });
            }
            return View(tableModel);
        }

        public IActionResult CreateProperty(Guid tableGuid, Guid dbGuid)
        {
            ViewData["DbGuid"] = dbGuid;
            ViewData["TableGuid"] = tableGuid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProperty(AttributePropertyModel attributePropertyModel)
        {
            if (ModelState.IsValid)
            {
                var attrProperty = new AttributeProperty { Name = attributePropertyModel.Name, Guid = Guid.NewGuid(), AttributeType = attributePropertyModel.AttributeType };
                dataBaseSchemaService.AddAttributeProperty(attrProperty, attributePropertyModel.TableGuid, attributePropertyModel.DbGuid);
                dataBaseSchemaService.SaveChanges();
                var fileName = dataBaseSchemaService.GetDbFileNameByGuid(attributePropertyModel.DbGuid);
                var filePath = dataBaseSchemaService.GetDbFilePath();
                var dataBaseService = new DataBaseService(filePath, fileName);
                var fields = dataBaseService.GetAllFieldsByTableGuid(attributePropertyModel.TableGuid);
                foreach (var field in fields)
                {
                    field.Values.Add(attrProperty.Guid, "");
                }
                dataBaseService.SaveChanges();
                return RedirectToAction("EditTable", new { guid = attributePropertyModel.TableGuid, dbGuid = attributePropertyModel.DbGuid });
            }
            return View(attributePropertyModel);
        }

        public IActionResult CreateRelation(Guid tableGuid, Guid dbGuid)
        {
            ViewData["DbGuid"] = dbGuid;
            ViewData["TableGuid"] = tableGuid;
            var tables = dataBaseSchemaService.GetAllTablesByDbGuid(dbGuid);
            ViewData["RelationTableGuid"] = new SelectList(tables, "Guid", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRelation(RelationModel relationModel)
        {
            if (ModelState.IsValid)
            {
                var relationTable = dataBaseSchemaService.GetTableByGuid(relationModel.RelationTableGuid, relationModel.DbGuid);
                var attrProperty = new AttributeProperty { Name = relationModel.Name, Guid = Guid.NewGuid(), AttributeType = AttributeType.Relation, RelationTableGuid = relationModel.RelationTableGuid };
                dataBaseSchemaService.AddRelation(attrProperty, relationModel.TableGuid, relationTable.Guid, relationModel.DbGuid);
                dataBaseSchemaService.SaveChanges();
                var fileName = dataBaseSchemaService.GetDbFileNameByGuid(relationModel.DbGuid);
                var filePath = dataBaseSchemaService.GetDbFilePath();
                var dataBaseService = new DataBaseService(filePath, fileName);
                var fields = dataBaseService.GetAllFieldsByTableGuid(relationModel.TableGuid);
                foreach (var field in fields)
                {
                    field.Values.Add(attrProperty.Guid, "");
                }
                dataBaseService.SaveChanges();
                return RedirectToAction("EditTable", new { guid = relationModel.TableGuid, dbGuid = relationModel.DbGuid });
            }
            ViewData["DbGuid"] = relationModel.DbGuid;
            ViewData["TableGuid"] = relationModel.TableGuid;
            var tables = dataBaseSchemaService.GetAllTablesByDbGuid(relationModel.DbGuid);
            ViewData["RelationTableGuid"] = new SelectList(tables, "Guid", "Name");
            return View(relationModel);
        }

        public IActionResult Delete(Guid guid)
        {
            var tableGuids = dataBaseSchemaService.GetAllTablesByDbGuid(guid).Select(x => x.Guid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(guid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            foreach (var tableGuid in tableGuids)
            {
                var fieldGuids = dataBaseService.GetAllFieldsByTableGuid(tableGuid).Select(x => x.Guid);
                foreach (var fieldGuid in fieldGuids)
                {
                    dataBaseService.RemoveFieldByGuid(fieldGuid);
                }
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveJsonDbObjectSchemaByGuid(guid);
            dataBaseSchemaService.SaveChanges();
            var dbs = dataBaseSchemaService.GetAllDbObjects();
            return View("Index", dbs);
        }

        public IActionResult DeleteTable(Guid guid, Guid dbGuid)
        {
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var fieldGuids = dataBaseService.GetAllFieldsByTableGuid(guid).Select(x => x.Guid);
            foreach (var fieldGuid in fieldGuids)
            {
                dataBaseService.RemoveFieldByGuid(fieldGuid);
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveTableByGuid(guid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            var db = dataBaseSchemaService.GetDbObjectByGuid(dbGuid);
            return View("Tables", db);
        }

        public IActionResult DeleteProperty(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var fields = dataBaseService.GetAllFieldsByTableGuid(tableGuid);
            foreach (var field in fields)
            {
                field.Values.Remove(guid);
            }
            dataBaseService.SaveChanges();
            dataBaseSchemaService.RemoveAttributePropertyByGuid(guid, tableGuid, dbGuid);
            dataBaseSchemaService.SaveChanges();
            ViewData["DbGuid"] = dbGuid;
            var table = dataBaseSchemaService.GetTableByGuid(tableGuid, dbGuid);
            return View("EditTable", table);
        }

        public IActionResult EditTable(Guid guid, Guid dbGuid)
        {
            ViewData["DbGuid"] = dbGuid;
            var table = dataBaseSchemaService.GetTableByGuid(guid, dbGuid);
            return View(table);
        }

        public IActionResult Fields(Guid tableGuid, Guid dbGuid)
        {
            var table = dataBaseSchemaService.GetTableByGuid(tableGuid, dbGuid);
            ViewData["DbGuid"] = dbGuid;
            ViewBag.Table = table;
            var filterValues = new Dictionary<Guid, string>();
            filterValues.Add(table.Guid, Request.Query.ContainsKey(table.Guid.ToString()) ? Request.Query[table.Guid.ToString()].ToString() : "");
            foreach (var filter in table.AttributeProperties)
            {
                filterValues.Add(filter.Guid, Request.Query.ContainsKey(filter.Guid.ToString()) ? Request.Query[filter.Guid.ToString()].ToString() : "");
            }
            ViewBag.FilterValues = filterValues;
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var properties = dataBaseService.GetAllFieldsByTableGuid(tableGuid);
            foreach (var filterValue in filterValues)
            {
                if (!String.IsNullOrEmpty(filterValue.Value))
                {
                    properties = properties.Where(x => (x.Values.ContainsKey(filterValue.Key) && x.Values[filterValue.Key].ToString().Contains(filterValue.Value)) || (x.TableGuid == filterValue.Key && x.Guid.ToString().Contains(filterValue.Value))).ToList();
                }
            }
            return View(properties);
        }

        public IActionResult CreateField(Guid tableGuid, Guid dbGuid)
        {
            var table = dataBaseSchemaService.GetTableByGuid(tableGuid, dbGuid);
            ViewBag.Table = table;
            var tableFieldModel = new TableFieldModel
            {
                Guid = Guid.NewGuid(),
                DbGuid = dbGuid,
                Columns = table.AttributeProperties,
                TableGuid = tableGuid,
                Values = new Dictionary<Guid, dynamic>()
            };
            foreach (var column in tableFieldModel.Columns)
            {
                tableFieldModel.Values.Add(column.Guid, "");
            }
            return View(tableFieldModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateField(IFormCollection tableFieldModelCollection)
        {
            var tableGuid = Guid.Parse(tableFieldModelCollection["TableGuid"]);
            var dbGuid = Guid.Parse(tableFieldModelCollection["DbGuid"]);
            var table = dataBaseSchemaService.GetTableByGuid(tableGuid, dbGuid);
            var dictValues = new Dictionary<Guid, dynamic>();
            var keys = tableFieldModelCollection.Keys.Where(x => table.AttributeProperties.Select(prop => prop.Guid.ToString()).Contains(x));
            ViewBag.Table = table;
            var tableFieldModel = new TableFieldModel
            {
                Guid = Guid.NewGuid(),
                TableGuid = tableGuid,
                DbGuid = dbGuid,
                Columns = table.AttributeProperties,
                Values = new Dictionary<Guid, dynamic>()
            };
            foreach (var key in keys)
            {
                tableFieldModel.Values.Add(Guid.Parse(key), tableFieldModelCollection[key].FirstOrDefault());
            }
            if (ModelState.IsValid)
            {
                var tableField = new TableField { Guid = tableFieldModel.Guid, TableGuid = tableFieldModel.TableGuid, Values = tableFieldModel.Values };
                var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
                var filePath = dataBaseSchemaService.GetDbFilePath();
                var dataBaseService = new DataBaseService(filePath, fileName);
                dataBaseService.AddField(tableField);
                dataBaseService.SaveChanges();
                return RedirectToAction("Fields", new { tableGuid, dbGuid });
            }
            return View(tableFieldModel);
        }

        public IActionResult DeleteField(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            dataBaseService.RemoveFieldByGuid(guid);
            dataBaseService.SaveChanges();
            return RedirectToAction("Fields", new { tableGuid, dbGuid });
        }

        public IActionResult EditField(Guid guid, Guid tableGuid, Guid dbGuid)
        {
            var table = dataBaseSchemaService.GetTableByGuid(tableGuid, dbGuid);
            var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
            var filePath = dataBaseSchemaService.GetDbFilePath();
            var dataBaseService = new DataBaseService(filePath, fileName);
            var tableField = dataBaseService.GetFieldByGuid(guid);
            ViewBag.Table = table;
            var tableFieldModel = new TableFieldModel
            {
                Guid = tableField.Guid,
                DbGuid = dbGuid,
                TableGuid = tableGuid,
                Columns = table.AttributeProperties,
                Values = tableField.Values
            };
            return View(tableFieldModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditField(IFormCollection tableFieldModelCollection)
        {
            var tableGuid = Guid.Parse(tableFieldModelCollection["TableGuid"]);
            var dbGuid = Guid.Parse(tableFieldModelCollection["DbGuid"]);
            var table = dataBaseSchemaService.GetTableByGuid(tableGuid, dbGuid);
            var dictValues = new Dictionary<Guid, dynamic>();
            var keys = tableFieldModelCollection.Keys.Where(x => table.AttributeProperties.Select(prop => prop.Guid.ToString()).Contains(x));
            ViewBag.Table = table;
            var tableFieldModel = new TableFieldModel
            {
                Guid = Guid.Parse(tableFieldModelCollection["Guid"]),
                TableGuid = tableGuid,
                DbGuid = dbGuid,
                Columns = table.AttributeProperties,
                Values = new Dictionary<Guid, dynamic>()
            };
            foreach (var key in keys)
            {
                tableFieldModel.Values.Add(Guid.Parse(key), tableFieldModelCollection[key].FirstOrDefault());
            }
            if (ModelState.IsValid)
            {
                var tableField = new TableField { Guid = tableFieldModel.Guid, TableGuid = tableFieldModel.TableGuid, Values = tableFieldModel.Values };
                var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
                var filePath = dataBaseSchemaService.GetDbFilePath();
                var dataBaseService = new DataBaseService(filePath, fileName);
                dataBaseService.UpdateField(tableField);
                dataBaseService.SaveChanges();
                return RedirectToAction("Fields", new { tableGuid, dbGuid });
            }
            return View(tableFieldModel);
        }
    }
}

