namespace Settings.Schema
{
	/// <summary>
	/// Base class for entity type or purpose within schema
	/// </summary>
	public class ElementInfo
	{
		public ElementInfo(string name, string description)
		{
			Name = name;
			Description = description;
		}

		/// <summary>
		/// Name of entity type or purpose within schema
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// Description of entity type or purpose within schema
		/// </summary>
		public string Description { get; protected set; }
	}
}