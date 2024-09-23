using System;
using TableDataBase.Models;

namespace TableDataBaseMVC.Models
{
	public class AttributePropertyModel
	{
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid DbGuid { get; set; } = Guid.NewGuid();
        public Guid TableGuid { get; set; } = Guid.NewGuid();
        public AttributeType AttributeType { get; set; }
        public string? Name { get; set; }
    }
}

