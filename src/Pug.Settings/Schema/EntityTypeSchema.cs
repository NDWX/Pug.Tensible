using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Settings.Schema
{
	internal class EntityTypeSchema : IEntityType
	{
		private readonly IDictionary<string, PurposeInfo> _purposeInfos;
		private readonly IDictionary<string, EntityTypePurpose> purposes;

		public EntityTypeSchema(EntityTypeInfo info, IDictionary<string, PurposeInfo> purposeInfos,
								IDictionary<string, EntityTypePurpose> purposes)
		{
			_purposeInfos = purposeInfos;
			Info = info;
			this.purposes = purposes;
		}

		public EntityTypeInfo Info { get; }

		public IDictionary<string, EntityTypePurpose> Purposes =>
			new ReadOnlyDictionary<string, EntityTypePurpose>(purposes);

		#region IEntityType Members

		public EntityTypeInfo GetInfo()
		{
			return Info;
		}

		public IEnumerable<PurposeInfo> GetPurposes()
		{
			return from purposeInfo in _purposeInfos
					where Purposes.ContainsKey(purposeInfo.Key)
					select purposeInfo.Value;
		}

		public IEnumerable<SettingDefinition> GetSettings(string purpose = "", string name = null)
		{
			if(purpose == null)
				purpose = string.Empty;
			else
				purpose = purpose.Trim();

			if(name != null)
				name = name.Trim();
			
			if(purposes.ContainsKey(purpose))
			{
				if(!string.IsNullOrWhiteSpace(name))
					return new[] {purposes[purpose].GetSetting(name.Trim())};

				return purposes[purpose].GetSettings();
			}

			return new SettingDefinition[0];
		}

		#endregion
	}
}