using System.Collections.Generic;

namespace Tensible
{
	internal interface IEntityDefinition
	{
		string Name { get; }
		
		IDictionary<string, ISettingsDefinition> Settings { get; }
	}
}