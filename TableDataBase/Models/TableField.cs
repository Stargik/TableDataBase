using System;
namespace TableDataBase.Models
{
	public class TableField
	{
		public Guid Guid { get; set; } = Guid.NewGuid();
		public string TableName { get; set; }
		public Dictionary<string, dynamic> Values { get; set; } = new();
    }
}

