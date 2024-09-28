namespace TableDataBase.Models
{
	public class DataBase
	{
        public string Name { get; set; }
		public List<Table> Tables { get; set; } = new();
	}
}

