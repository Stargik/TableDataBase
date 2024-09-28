using TableDataBase.Models;

namespace TableDataBaseMVC.Models
{
    public class TableModel
    {
        public string DbName { get; set; }
        public string Name { get; set; }
        public List<AttributeProperty> AttributeProperties { get; set; } = new();
    }
}

