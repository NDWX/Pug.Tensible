namespace Settings.Schema
{
	public class ElementInfo
	{
		public ElementInfo(string name, string description)
		{
			Name = name;
			Description = description;
		}

		public string Name { get; protected set; }

		public string Description { get; protected set; }
	}
}