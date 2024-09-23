using System;
namespace TableDataBase.Models
{
	public class TableField
	{
		public Guid Guid { get; set; } = Guid.NewGuid();
		public Guid TableGuid { get; set; }
		public Dictionary<Guid, dynamic> Values { get; set; } = new();
    }
}

