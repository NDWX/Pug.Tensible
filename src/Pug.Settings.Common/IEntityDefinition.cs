using System.Collections.Generic;

namespace Tensible
{
	public interface IEntityDefinition
	{
		string Name { get; }
		
		IDictionary<string, ISettingsDefinition> Settings { get; }
	}
}