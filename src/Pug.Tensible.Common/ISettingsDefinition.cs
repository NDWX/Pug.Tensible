namespace Pug.Tensible
{
	public interface ISettingsDefinition
	{
		string Name { get; }

		ISettingsResolver GetResolver(IEntityDefinition entityDefinition);
	}
}