using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	public static class TypeExtensions
	{
		private static Type _enumerableCompareType = typeof(IEnumerable<>);

		/// <summary>
		/// Gets the element <see cref="Type"/> and an enumerable type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>A <see cref="Type"/> if type is enumerable, otherwise null.</returns>
		public static Type GetEnumerableElementType(this Type type)
		{
			//  string is an enumerable of chars but we prefer to use it as a string
			if (type == typeof(string))
				return null;

			var enumerableInterfaceType = type.GetInterfaces()
				.FirstOrDefault(q => q.IsGenericType && q.GetGenericTypeDefinition() == _enumerableCompareType);
			if (enumerableInterfaceType == null && type.IsGenericType && type.GetGenericTypeDefinition() == _enumerableCompareType)
				enumerableInterfaceType = type;
			if (enumerableInterfaceType == null)
				return null;

			return enumerableInterfaceType.GetGenericArguments()[0];
		}
	}
}
