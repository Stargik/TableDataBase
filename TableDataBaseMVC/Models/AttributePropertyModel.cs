using System;
using TableDataBase.Models;

namespace TableDataBaseMVC.Models
{
	public class AttributePropertyModel
	{
        public string DbName { get; set; }
        public string TableName { get; set; }
        public AttributeType AttributeType { get; set; }
        public string Name { get; set; }
    }
}

