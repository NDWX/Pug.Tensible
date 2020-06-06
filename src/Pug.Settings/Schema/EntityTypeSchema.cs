using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Settings.Schema
{
	internal class EntityTypeSchema : IEntityType
	{
		private readonly IDictionary<string, PurposeInfo> _purposeInfos;
		private readonly IDictionary<string, EntityPurposeSchema> purposes;

		public EntityTypeSchema(EntityTypeInfo info, IDictionary<string, PurposeInfo> purposeInfos,
								IDictionary<string, EntityPurposeSchema> purposes)
		{
			_purposeInfos = purposeInfos;
			Info = info;
			this.purposes = purposes;
		}

		public EntityTypeInfo Info { get; }

		public IDictionary<string, EntityPurposeSchema> Purposes =>
			new ReadOnlyDictionary<string, EntityPurposeSchema>(purposes);

		#region IEntityType Members

		public EntityTypeInfo GetInfo()
		{
			return Info;
		}

		public IEnumerable<PurposeInfo> GetPurposes()
		{
			return from purposeInfo in _purposeInfos
					// ReSharper disable once HeapView.DelegateAllocation
					where Purposes.ContainsKey(purposeInfo.Key)
					select purposeInfo.Value;
		}

		public IEntityPurposeSchema GetPurpose(string name)
		{
			name = name?.Trim() ?? string.Empty;

			if(!purposes.ContainsKey(name))
				return null;
			
			return purposes[name];
		}

		public IEnumerable<ISettingSchema> GetSettings(string purpose = "", string name = null)
		{
			purpose = purpose?.Trim() ?? string.Empty;

			name = name?.Trim();
			
			if(purposes.ContainsKey(purpose))
			{
				if(!string.IsNullOrWhiteSpace(name))
				{
					if( purposes[purpose].Settings.ContainsKey(name))
						return new[] {purposes[purpose].Settings[name]};
					
					return new ISettingSchema[] { };
				}

				return purposes[purpose].Settings.Values;
			}

			return new SettingSchema[0];
		}

		#endregion
	}
}