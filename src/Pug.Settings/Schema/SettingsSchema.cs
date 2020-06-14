using System;
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

		public IEnumerable<EntityTypeInfo> GetEntityTypes()
		{
			return _entityTypes.Values.Select(x => x.Info);
		}

		public IEnumerable<PurposeInfo> GetPurposes()
		{
			return _purposes.Values;
		}

		public IEntityTypeSchema GetEntityType(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			
			name = name.Trim();
			
			if(!_entityTypes.ContainsKey(name))
				return null;

			return _entityTypes[name];
		}

		public IResolver GetResolver(ISettingStore settingStore, IEntityRelationshipResolver entityRelationshipResolver)
		{
			return new Resolver(this, settingStore, entityRelationshipResolver);
		}

		#endregion
	}
}