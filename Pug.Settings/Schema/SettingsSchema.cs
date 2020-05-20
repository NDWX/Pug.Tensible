using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	internal class SettingsSchema : ISettingsSchema
	{
		private readonly IDictionary<string, EntityTypeSchema> _entityTypes;
		private readonly IDictionary<string, PurposeInfo> _purposes;

		public SettingsSchema(IDictionary<string, PurposeInfo> purposes,
							IDictionary<string, EntityTypeSchema> entityTypes)
		{
			_purposes = purposes;
			_entityTypes = entityTypes;
		}

		#region ISettingsSchema Members

		public IEnumerable<EntityTypeInfo> GetEntityTypes(string parent)
		{
			return _entityTypes.Values.Select(x => x.Info);
		}

		public IEnumerable<PurposeInfo> GetPurposes()
		{
			return _purposes.Values;
		}

		public IEntityType GetEntityType(string name)
		{
			if(!_entityTypes.ContainsKey(name))
				return null;

			return _entityTypes[name];
		}

		#endregion
	}
}