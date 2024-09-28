namespace TableDataBase.Models
{
	public class Table
	{
        public string Name { get; set; }
        public List<AttributeProperty> AttributeProperties { get; set; } = new();
	}
}