namespace TableDataBase.Models
{
	public class Table
	{
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public List<AttributeProperty> AttributeProperties { get; set; } = new();
	}
}