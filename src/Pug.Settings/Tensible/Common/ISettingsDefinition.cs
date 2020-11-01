namespace Tensible
{
	public interface ISettingsDefinition
	{
		string Name { get; }

		ISettingsResolver GetResolver();
	}
}