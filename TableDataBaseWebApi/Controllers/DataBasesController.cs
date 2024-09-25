using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TableDataBase.Interfaces;
using TableDataBase.Models;
using TableDataBase.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TableDataBaseWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataBasesController : Controller
    {
        private static IDataBaseSchemaService? dataBaseSchemaService;
        private static string? connectionString;
        private readonly bool isLocal;

        public DataBasesController(IConfiguration configuration)
        {
            isLocal = configuration.GetValue<bool>("IsLocal");
        }

        // POST api/DataBases/SetConnection
        [HttpPost("SetConnection")]
        public void SetConnection(string connection)
        {
            connectionString = connection;
            if (!String.IsNullOrEmpty(connectionString))
            {
                dataBaseSchemaService = new DataBaseSchemaClientService(connectionString);
            }
        }

        // GET: api/DataBases
        [HttpGet]
        public ActionResult<IEnumerable<DataBase>> Get()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                return BadRequest();
            }
            var dbs = dataBaseSchemaService.GetAllDbObjects();
            return dbs;
        }

        // GET api/DataBases/5
        [HttpGet("{guid}")]
        public ActionResult<DataBase> Get(Guid guid)
        {
            var db = dataBaseSchemaService.GetDbObjectByGuid(guid);
            return db;
        }

        // POST api/DataBases
        [HttpPost]
        public void Post([FromBody] DataBase dataBase)
        {
            dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
            dataBaseSchemaService.SaveChanges();
        }

        // DELETE api/DataBases/5
        [HttpDelete("{guid}")]
        public void Delete(Guid guid)
        {
            dataBaseSchemaService.RemoveJsonDbObjectSchemaByGuid(guid);
            dataBaseSchemaService.SaveChanges();
        }

        // PUT api/DataBases/
        [HttpPut("{guid}")]
        public void Put([FromBody] DataBase dataBase)
        {
            dataBaseSchemaService.UpdateJsonDbObjectSchema(dataBase);
            dataBaseSchemaService.SaveChanges();
        }
        
        // GET api/DataBases/Tables
        [HttpGet("Tables")]
        public ActionResult<IEnumerable<Table>> GetTables(Guid dbguid)
        {
            var tables = dataBaseSchemaService.GetAllTablesByDbGuid(dbguid);
            return tables;
        }

        // POST api/DataBases
        [HttpPost("Tables")]
        public void CreateTable([FromBody]Table table, Guid dbguid)
        {
            dataBaseSchemaService.AddTable(table, dbguid);
            dataBaseSchemaService.SaveChanges();
        }

        // DELETE api/DataBases
        [HttpDelete("Tables")]
        public void DeleteTable(Guid guid, Guid dbguid)
        {
            dataBaseSchemaService.RemoveTableByGuid(guid, dbguid);
            dataBaseSchemaService.SaveChanges();
        }

        // PUT api/DataBases/Tables
        [HttpPut("Tables")]
        public void PutTable([FromBody] Table table, Guid dbguid)
        {
            dataBaseSchemaService.UpdateTable(table, dbguid);
        }
        
        // GET api/DataBases/AttributeProperties
        [HttpGet("AttributeProperties")]
        public ActionResult<IEnumerable<AttributeProperty>> GetAttributeProperties(Guid tableguid, Guid dbguid)
        {
            var properties = dataBaseSchemaService.GetAllAttributePropertiesByDbTableGuid(tableguid, dbguid);
            return properties;
        }

        // POST api/DataBases/AttributeProperties
        [HttpPost("AttributeProperties")]
        public void CreateAttributeProperty([FromBody] AttributeProperty attributeProperty, Guid tableguid, Guid dbguid)
        {
            dataBaseSchemaService.AddAttributeProperty(attributeProperty, tableguid, dbguid);
            dataBaseSchemaService.SaveChanges();
        }

        // DELETE api/DataBases/AttributeProperties
        [HttpDelete("AttributeProperties")]
        public void DeleteAttributeProperty(Guid guid, Guid tableguid, Guid dbguid)
        {
            dataBaseSchemaService.RemoveAttributePropertyByGuid(guid, tableguid, dbguid);
            dataBaseSchemaService.SaveChanges();
        }

        // PUT api/DataBases/AttributeProperties
        [HttpPut("AttributeProperties")]
        public void PutAttributeProperty([FromBody] AttributeProperty attributeProperty, Guid tableguid, Guid dbguid)
        {
            dataBaseSchemaService.UpdateAttributeProperty(attributeProperty, tableguid, dbguid);
            dataBaseSchemaService.SaveChanges();
        }
        
        // POST api/DataBases/AttributeProperties
        [HttpPost("Relations")]
        public void CreateRelation([FromBody] AttributeProperty attributeProperty, Guid tableguid, Guid targettableguid, Guid dbguid)
        {
            dataBaseSchemaService.AddRelation(attributeProperty, tableguid, targettableguid, dbguid);
            dataBaseSchemaService.SaveChanges();
        }

        // GET api/DataBases/Fields
        [HttpGet("Fields")]
        public ActionResult<IEnumerable<TableField>> GetFields(Guid tableguid, Guid dbguid)
        {
            var dataBaseService = GetDataBaseService(dbguid);
            var fields = dataBaseService.GetAllFieldsByTableGuid(tableguid);
            return fields;
        }
        
        // GET api/DataBases/Fields
        [HttpGet("Fields/{guid}")]
        public ActionResult<TableField> GetField(Guid guid, Guid dbguid)
        {
            var dataBaseService = GetDataBaseService(dbguid);
            var field = dataBaseService.GetFieldByGuid(guid);
            return field;
        }
        
        // POST api/DataBases/Fields
        [HttpPost("Fields")]
        public void CreateField([FromBody] TableField tableField, Guid dbguid)
        {
            var dataBaseService = GetDataBaseService(dbguid);
            dataBaseService.AddField(tableField);
            dataBaseService.SaveChanges();
        }

        // PUT api/DataBases/Fields
        [HttpPut("Fields")]
        public void PutField([FromBody] TableField tableField, Guid dbguid)
        {
            var dataBaseService = GetDataBaseService(dbguid);
            dataBaseService.UpdateField(tableField);
            dataBaseService.SaveChanges();
        }

        // DELETE api/DataBases/Fields
        [HttpDelete("Fields")]
        public void DeleteField(Guid guid, Guid dbguid)
        {
            var dataBaseService = GetDataBaseService(dbguid);
            dataBaseService.RemoveFieldByGuid(guid);
            dataBaseService.SaveChanges();
        }
        
        private IDataBaseService GetDataBaseService(Guid dbGuid)
        {
            if (isLocal)
            {
                var fileName = dataBaseSchemaService.GetDbFileNameByGuid(dbGuid);
                var filePath = dataBaseSchemaService.GetDbFilePath();
                var dataBaseService = new DataBaseService(filePath, fileName);
                return dataBaseService;
            }
            else
            {
                var dataBaseService = new DataBaseClientService(connectionString, dbGuid.ToString());
                return dataBaseService;
            }
        }

    }
}

