using System;
using System.Collections.Generic;
using Settings;

namespace Tensible
{
	public class EffectiveSettings<TPurpose> where TPurpose : class
	{
		public EffectiveSettings(TPurpose effectiveSettings, TPurpose storedSettings,
								IDictionary<string, SettingValueSource> settingsSource)
		{
			this.Settings = effectiveSettings ?? throw new ArgumentNullException(nameof(effectiveSettings));
			this.StoredSettings = storedSettings ?? throw new ArgumentNullException(nameof(storedSettings));
			this.SettingsSource = settingsSource ?? throw new ArgumentNullException(nameof(settingsSource));
		}
		
		public TPurpose Settings { get; }
		
		public TPurpose StoredSettings { get; }
		 
		public IDictionary<string, SettingValueSource> SettingsSource { get; }
	}
}