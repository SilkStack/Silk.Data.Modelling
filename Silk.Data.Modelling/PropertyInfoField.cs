using System;
using System.Reflection;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data structure field based on a CLR property.
	/// </summary>
	public class PropertyInfoField : IField
	{
		public string FieldName { get; private set; }

		public bool CanRead { get; private set; }

		public bool CanWrite { get; private set; }

		public bool IsEnumerableType => FieldElementType != null;

		public Type FieldDataType { get; private set; }

		public Type FieldElementType { get; private set; }

		private PropertyInfoField() { }

		public static PropertyInfoField CreateFromPropertyInfo(PropertyInfo propertyInfo)
		{
			return new PropertyInfoField
			{
				FieldName = propertyInfo.Name,
				CanRead = propertyInfo.CanRead,
				CanWrite = propertyInfo.CanWrite,
				FieldDataType = propertyInfo.PropertyType,
				FieldElementType = propertyInfo.PropertyType.GetEnumerableElementType()
			};
		}
	}
}
