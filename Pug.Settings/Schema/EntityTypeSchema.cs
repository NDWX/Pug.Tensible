using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	internal class EntityTypeSchema : IEntityType
	{
		private readonly IDictionary<string, PurposeInfo> _purposeInfos;

		public EntityTypeSchema(EntityTypeInfo info, IDictionary<string, PurposeInfo> purposeInfos,
								IDictionary<string, IEnumerable<SettingDefinition>> purposes)
		{
			_purposeInfos = purposeInfos;
			Info = info;
			Purposes = purposes;
		}

		public EntityTypeInfo Info { get; }

		public IDictionary<string, IEnumerable<SettingDefinition>> Purposes { get; }

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

		public IEnumerable<SettingDefinition> GetSettings(string purpose = null, string name = null)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}