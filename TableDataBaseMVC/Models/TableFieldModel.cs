using System;
namespace TableDataBaseMVC.Models
{
	public class TableFieldModel
	{
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid TableGuid { get; set; }
        public Guid DbGuid { get; set; }
        public List<TableDataBase.Models.AttributeProperty> Columns { get; set; } = new();
        public Dictionary<Guid, dynamic> Values { get; set; } = new();
    }
}

