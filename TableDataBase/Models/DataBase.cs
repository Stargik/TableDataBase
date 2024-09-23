namespace TableDataBase.Models
{
	public class DataBase
	{
		public Guid Guid { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
		public List<Table> Tables { get; set; } = new();
	}
}

