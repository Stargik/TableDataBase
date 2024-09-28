using System;
namespace TableDataBaseMVC.Models
{
	public class TableFieldModel
	{
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string TableName { get; set; }
        public string DbName { get; set; }
        public List<TableDataBase.Models.AttributeProperty> Columns { get; set; } = new();
        public Dictionary<string, dynamic> Values { get; set; } = new();
    }
}

