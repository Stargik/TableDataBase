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
    [Route("api/")]
    public class DataBasesController : Controller
    {
        private static IDataBaseSchemaService? dataBaseSchemaService;
        private static string? connectionString;
        private readonly IDataBaseSchemaServiceFactory dataBaseSchemaServiceFactory;
        private readonly IDataBaseServiceFactory dataBaseServiceFactory;

        public DataBasesController(IDataBaseSchemaServiceFactory dataBaseSchemaServiceFactory, IDataBaseServiceFactory dataBaseServiceFactory)
        {
            this.dataBaseSchemaServiceFactory = dataBaseSchemaServiceFactory;
            this.dataBaseServiceFactory = dataBaseServiceFactory;
        }

        // POST api/setconnection
        [HttpPost("setconnection")]
        public ActionResult SetConnection(string connection)
        {
            connectionString = connection;
            if (!String.IsNullOrEmpty(connectionString))
            {
                dataBaseSchemaService = dataBaseSchemaServiceFactory.Create(connectionString);
            }
            return Ok();
        }

        // GET: api/databases
        [HttpGet("databases")]
        public ActionResult<IEnumerable<DataBase>> Get()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                return BadRequest();
            }
            var dbs = dataBaseSchemaService.GetAllDbObjects();
            return Ok(dbs);
        }

        // GET api/databases/5
        [HttpGet("databases/{name}")]
        public ActionResult<DataBase> Get(string name)
        {
            var db = dataBaseSchemaService.GetDbObjectByName(name);
            return Ok(db);
        }

        // POST api/dataBases
        [HttpPost("databases")]
        public ActionResult Post([FromBody] DataBase dataBase)
        {
            if (dataBaseSchemaService.GetAllDbObjects().Select(x => x.Name).Contains(dataBase.Name))
            {
                return BadRequest();
            }
            dataBaseSchemaService.AddJsonDbObjectSchema(dataBase);
            dataBaseSchemaService.SaveChanges();
            return Ok();
        }

        // DELETE api/dataBases/5
        [HttpDelete("databases/{name}")]
        public ActionResult Delete(string name)
        {
            dataBaseSchemaService.RemoveJsonDbObjectSchemaByName(name);
            dataBaseSchemaService.SaveChanges();
            return Ok();
        }

        // GET api/databases/5/tables
        [HttpGet("databases/{dbname}/tables")]
        public ActionResult<IEnumerable<Table>> GetTables(string dbname)
        {
            var tables = dataBaseSchemaService.GetAllTablesByDbName(dbname);
            return Ok(tables);
        }

        // GET api/databases/5/tables/5
        [HttpGet("databases/{dbname}/tables/{name}")]
        public ActionResult<Table> GetTables(string name, string dbname)
        {
            var table = dataBaseSchemaService.GetTableByName(name, dbname);
            return Ok(table);
        }

        // POST api/databases/5/tables
        [HttpPost("databases/{dbname}/tables")]
        public ActionResult CreateTable([FromBody]Table table, string dbname)
        {
            if (dataBaseSchemaService.GetAllTablesByDbName(dbname).Select(x => x.Name).Contains(table.Name))
            {
                return BadRequest();
            }
            dataBaseSchemaService.AddTable(table, dbname);
            dataBaseSchemaService.SaveChanges();
            return Ok();
        }

        // DELETE api/databases/5/tables/5
        [HttpDelete("databases/{dbname}/tables/{name}")]
        public ActionResult DeleteTable(string name, string dbname)
        {
            dataBaseSchemaService.RemoveTableByName(name, dbname);
            dataBaseSchemaService.SaveChanges();
            return Ok();
        }

        // GET api/databases/5/tables/5/attributeproperties
        [HttpGet("databases/{dbname}/tables/{tablename}/attributeproperties")]
        public ActionResult<IEnumerable<AttributeProperty>> GetAttributeProperties(string tablename, string dbname)
        {
            var properties = dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(tablename, dbname);
            return Ok(properties);
        }

        // GET api/databases/5/tables/5/attributeproperties/5
        [HttpGet("databases/{dbname}/tables/{tablename}/attributeproperties/{name}")]
        public ActionResult<AttributeProperty> GetAttributeProperties(string name, string tablename, string dbname)
        {
            var propertiy = dataBaseSchemaService.GetAttributePropertyByName(name, tablename, dbname);
            return Ok(propertiy);
        }

        // POST api/databases/5/tables/5/attributeproperties
        [HttpPost("databases/{dbname}/tables/{tablename}/attributeproperties")]
        public ActionResult CreateAttributeProperty([FromBody] AttributeProperty attributeProperty, string tablename, string dbname)
        {
            if (dataBaseSchemaService.GetAllAttributePropertiesByDbTableName(tablename, dbname).Select(x => x.Name).Contains(attributeProperty.Name))
            {
                return BadRequest();
            }
            dataBaseSchemaService.AddAttributeProperty(attributeProperty, tablename, dbname);
            dataBaseSchemaService.SaveChanges();
            return Ok();
        }

        // DELETE api/databases/5/tables/5/attributeproperties/5
        [HttpDelete("databases/{dbname}/tables/{tablename}/attributeproperties/{name}")]
        public ActionResult DeleteAttributeProperty(string name, string tablename, string dbname)
        {
            dataBaseSchemaService.RemoveAttributePropertyByName(name, tablename, dbname);
            dataBaseSchemaService.SaveChanges();
            return Ok();
        }

        // GET api/databases/5/tables/5/fields
        [HttpGet("databases/{dbname}/tables/{tablename}/fields")]
        public ActionResult<IEnumerable<TableField>> GetFields(string tablename, string dbname)
        {
            var dataBaseService = GetDataBaseService(dbname);
            var fields = dataBaseService.GetAllFieldsByTableName(tablename);
            return fields;
        }

        // GET api/databases/5/tables/fields/5
        [HttpGet("databases/{dbname}/tables/fields/{guid}")]
        public ActionResult<TableField> GetField(Guid guid, string dbname)
        {
            var dataBaseService = GetDataBaseService(dbname);
            var field = dataBaseService.GetFieldByGuid(guid);
            return Ok(field);
        }

        // POST api/databases/5/tables/fields
        [HttpPost("databases/{dbname}/tables/fields")]
        public ActionResult CreateField([FromBody] TableField tableField, string dbname)
        {
            var dataBaseService = GetDataBaseService(dbname);
            dataBaseService.AddField(tableField);
            dataBaseService.SaveChanges();
            return Ok();
        }

        // PUT api/databases/5/tables/fields
        [HttpPut("databases/{dbname}/tables/fields")]
        public ActionResult PutField([FromBody] TableField tableField, string dbname)
        {
            var dataBaseService = GetDataBaseService(dbname);
            dataBaseService.UpdateField(tableField);
            dataBaseService.SaveChanges();
            return Ok();
        }

        // DELETE api/databases/5/tables/fields/5
        [HttpDelete("databases/{dbname}/tables/fields/{guid}")]
        public ActionResult DeleteField(Guid guid, string dbname)
        {
            var dataBaseService = GetDataBaseService(dbname);
            dataBaseService.RemoveFieldByGuid(guid);
            dataBaseService.SaveChanges();
            return Ok();
        }
        
        private IDataBaseService GetDataBaseService(string dbName)
        {
            var dataBaseService = dataBaseServiceFactory.Create(connectionString, dbName);
            return dataBaseService;
        }
    }
}

