using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Intruder.Tools
{
	public static class TypeUtility
	{
		public static Type[] SubClasses( this Type type )
		{
			return type.Assembly.GetTypes().Where( x => x.IsSubclassOf( type ) && !x.IsAbstract ).ToArray();
		}

		public static bool HasParameterlessConstructor( this Type type )
		{
			return type.GetConstructor( Type.EmptyTypes ) != null;
		}

		// I hate this so much
		public static IEnumerable<Type> GetLoadableTypes( this Assembly assembly )
		{
			try
			{
				return assembly.GetTypes();
			}
			catch ( ReflectionTypeLoadException e )
			{
				return e.Types.Where( t => t != null );
			}
		}
	}
}
