namespace TableDataBase.Models
{
	public class AttributeProperty
	{
		public Guid Guid { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public AttributeType AttributeType { get; set; }
        public Guid? RelationTableGuid { get; set; } = null;
    }
}

