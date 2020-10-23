using System;
using Settings;

namespace Pug.Settings
{
	public class Setting<TValue> : ISetting
	{
		public TValue Value { get; }

		// ReSharper disable HeapView.PossibleBoxingAllocation
		object ISetting.ValueObject => Value;
		// ReSharper restore HeapView.PossibleBoxingAllocation
		
		public SettingValueSource ValueSource { get; }

		public Setting(TValue value, SettingValueSource valueSource)
		{
			Value = value;
			ValueSource = valueSource ?? throw new ArgumentNullException(nameof(valueSource));
		}
	}
	
}