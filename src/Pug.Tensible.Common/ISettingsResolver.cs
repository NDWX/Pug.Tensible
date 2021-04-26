using System;

namespace Pug.Tensible
{
	public interface ISettingsResolver
	{
		TSettings Get<TEntity, TSettings>(TEntity entityIdentifier, IServiceProvider serviceProvider)
			where TSettings : class;
		
		TSettings GetEffective<TEntity, TSettings>(TEntity entityIdentifier, IServiceProvider serviceProvider) where TSettings : class;
	}
}