using System.Collections.Generic;

namespace Pug.Tensible
{
	public interface IEntityDefinition
	{
		string Name { get; }
		
		IDictionary<string, ISettingsDefinition> Settings { get; }
	}
}