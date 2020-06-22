using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Settings.Schema
{
	/// <summary>
	/// Definition of a prupose within an entity type
	/// </summary>
	public class EntityPurposeDefinition
	{
		/// <summary>
		/// Name of purpose
		/// </summary>
		public string Name { get; }
		
		/// <summary>
		/// Name of parent entity type from which settings should be inherited
		/// </summary>
		public string ParentEntityType { get; }
		
		/// <summary>
		/// A child entity may choose to only inherit or block inheritance of specific settings from parent entity type
		/// </summary>
		public PurposeSettingsInheritance Inheritance { get; }
		
		/// <summary>
		/// Definition of settings within a purpose
		/// </summary>
		public IEnumerable<SettingDefinition> Settings { get; }
		
		/// <summary>
		/// An entity may choose to only allow or block inheritance of certain settings by child entity
		/// </summary>
		public PurposeSettingsInheritance Inheritability { get; }

		/// <summary>
		/// Define setting purpose with settings inheritance from another entity type
		/// </summary>
		/// <param name="name">Name of purpose</param>
		/// <param name="parentEntityType">Name of parent entity type from which settings should be inherited</param>
		/// <param name="inheritance">A child entity may choose to only inherit or block inheritance of specific settings from parent entity type</param>
		/// <param name="settings">Definition of settings within a purpose</param>
		/// <param name="inheritability">An entity may choose to only allow or block inheritance of certain settings by child entity</param>
		public EntityPurposeDefinition(string name, string parentEntityType, 
										PurposeSettingsInheritance inheritance = null, 
										IEnumerable<SettingDefinition> settings = null, 
										PurposeSettingsInheritance inheritability = null)
		{
			Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
			ParentEntityType = parentEntityType?.Trim();

			if(string.IsNullOrEmpty(ParentEntityType))
				Inheritance = null;
			else
			{
				Inheritance = inheritance?? new PurposeSettingsInheritance(PurposeSettingsInheritanceType.Inherit);
			}

			Settings = settings ?? new SettingDefinition[0];
			
			if( inheritability != null )
				Inheritability = inheritability;
			else
				Inheritability = new PurposeSettingsInheritance(PurposeSettingsInheritanceType.DoNotInherit);
		}


		/// <summary>
		/// Define setting purpose without inheritance from another entity type
		/// </summary>
		/// <param name="name">Name of purpose</param>
		/// <param name="settings">Definition of settings within a purpose</param>
		/// <param name="inheritability">An entity may choose to only allow or block inheritance of certain settings by child entity</param>
		public EntityPurposeDefinition(string name,
										IEnumerable<SettingDefinition> settings,
										PurposeSettingsInheritance inheritability = null) 
			: this(name, null, null, settings, inheritability)
		{
		}
	}
}