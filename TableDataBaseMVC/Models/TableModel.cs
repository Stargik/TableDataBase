using TableDataBase.Models;

namespace TableDataBaseMVC.Models
{
    public class TableModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid DbGuid { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public List<AttributeProperty> AttributeProperties { get; set; } = new();
    }
}

