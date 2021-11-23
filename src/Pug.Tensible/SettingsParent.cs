using System;

namespace Pug.Tensible
{
	internal class SettingsParent<TEntity, TPurpose>
	{
		private readonly Func<TEntity, TPurpose, IServiceProvider, TPurpose> _mapper;
		public string ParentEntityType { get; }
		public string ParentSettingsPurpose { get; }

		public SettingsParent(string parentEntityType, string parentSettingsPurpose,
							Func<TEntity, TPurpose, IServiceProvider, TPurpose> mapper)
		{
			if(string.IsNullOrWhiteSpace(parentEntityType))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(parentEntityType));
			
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

			ParentEntityType = parentEntityType;
			ParentSettingsPurpose = parentSettingsPurpose ?? throw new ArgumentNullException(nameof(parentSettingsPurpose));
		}

		public TPurpose MapTo(TEntity entity, TPurpose settings, IServiceProvider serviceProvider)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			
			return _mapper(entity, settings, serviceProvider);
		}
	}
}