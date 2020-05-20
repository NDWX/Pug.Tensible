namespace Settings.Schema
{
	public class PurposeInfo : ElementInfo
	{
		public PurposeInfo(string name, string description, Inheritability inheritability,
							bool allowInheritabilityOverride)
			: base(name, description)
		{
			Inheritability = inheritability;
			AllowInheritabilityOverride = allowInheritabilityOverride;
		}

		public Inheritability Inheritability { get; protected set; }

		public bool AllowInheritabilityOverride { get; protected set; }
	}
}