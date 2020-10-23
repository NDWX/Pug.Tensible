using System;

namespace Pug.Settings
{
	public static class TypeExtensions
	{
		public static bool Inherits(this Type type, Type baseType)
		{
			if(type == null) throw new ArgumentNullException(nameof(type));
			if(baseType == null) throw new ArgumentNullException(nameof(baseType));

			if(baseType.IsGenericTypeDefinition)
			{
				if(type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
					return true;
			}

			if(type.BaseType == null)
				return false;
			
			if(type.BaseType == baseType)
				return true;

			return type.BaseType.Inherits(baseType);
		}
		
		public static bool InheritsOrEqualsTo(this Type type, Type baseType)
		{
			if(type == baseType)
				return true;

			return type.Inherits(baseType);
		}
	}
}