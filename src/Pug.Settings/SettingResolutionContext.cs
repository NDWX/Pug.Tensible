using System;
using Pug.Settings.Schema;
using Settings;

namespace Pug.Settings
{
	class SettingResolutionContext
	{
		public SettingResolutionContext(IEntityTypeSettingsSchema entityTypeSettingsSchema, IPurposeDefinition purposeDefinition)
		{
			EntityTypeSettingsSchema = entityTypeSettingsSchema ?? throw new ArgumentNullException(nameof(entityTypeSettingsSchema));
			PurposeDefinition = purposeDefinition ?? throw new ArgumentNullException(nameof(purposeDefinition));
		}
		
		public SettingResolutionContext(IEntityTypeSettingsSchema entityTypeSettingsSchema, IPurposeDefinition purposeDefinition, IEntityTypeSettingsSchema parentEntityTypeSettingsSchema, EntityIdentifier parentEntity, object parentEntityPurposeSettings) : this(entityTypeSettingsSchema, purposeDefinition)
		{
			ParentEntityTypeSettingsSchema = parentEntityTypeSettingsSchema ?? throw new ArgumentNullException(nameof(parentEntityTypeSettingsSchema));
			ParentEntity = parentEntity ?? throw new ArgumentNullException(nameof(parentEntity));
			ParentEntityPurposeSettings = parentEntityPurposeSettings ?? throw new ArgumentNullException(nameof(parentEntityPurposeSettings));
		}
		
		public IEntityTypeSettingsSchema EntityTypeSettingsSchema { get;  }
		
		public IPurposeDefinition PurposeDefinition { get;  }
		
		public IEntityTypeSettingsSchema ParentEntityTypeSettingsSchema { get;  }

		public EntityIdentifier ParentEntity { get;  }

		public object ParentEntityPurposeSettings { get;  }
	}
}