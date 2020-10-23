namespace Settings
{
	public static class Extensions
	{
		public static bool IsEmpty(this EntityIdentifier entityIdentifier)
		{
			return string.IsNullOrWhiteSpace(entityIdentifier.Type) &&
					string.IsNullOrWhiteSpace(entityIdentifier.Identifier);
		}
	}
}