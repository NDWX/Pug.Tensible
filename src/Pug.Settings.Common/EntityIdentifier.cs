namespace Settings
{
	/// <summary>
	/// Unique identifier of an entity
	/// </summary>
	public class EntityIdentifier
	{
		/// <summary>
		/// Type of entity. <seealso cref="Schema.IEntityType"/>
		/// </summary>
		public string Type { get; set; }
		
		/// <summary>
		/// Identifier of entity
		/// </summary>
		public string Identifier { get; set; }
	}
}