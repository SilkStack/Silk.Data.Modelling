using System;
using System.Reflection;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data structure field based on a CLR property.
	/// </summary>
	public abstract class PropertyInfoField : IField
	{
		public string FieldName { get; private set; }

		public bool CanRead { get; private set; }

		public bool CanWrite { get; private set; }

		public bool IsEnumerableType => FieldElementType != null;

		public Type FieldDataType { get; private set; }

		public Type FieldElementType { get; private set; }

		public PropertyInfo Property { get; private set; }

		protected PropertyInfoField() { }

		/// <summary>
		/// Create a PropertyInfoField from a PropertyInfo instance.
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <returns></returns>
		public static PropertyInfoField CreateFromPropertyInfo(PropertyInfo propertyInfo)
		{
			var enumerableElementType = propertyInfo.PropertyType.GetEnumerableElementType();
			var ret = Activator.CreateInstance(typeof(PropertyInfoField<>).MakeGenericType(
				enumerableElementType ?? propertyInfo.PropertyType
				)) as PropertyInfoField;

			ret.FieldName = propertyInfo.Name;
			ret.CanRead = propertyInfo.CanRead;
			ret.CanWrite = propertyInfo.CanWrite;
			ret.FieldDataType = propertyInfo.PropertyType;
			ret.FieldElementType = propertyInfo.PropertyType.GetEnumerableElementType();
			ret.Property = propertyInfo;

			return ret;
		}

		public abstract void Dispatch(IFieldGenericExecutor executor);
	}

	public class PropertyInfoField<T> : PropertyInfoField
	{
		public override void Dispatch(IFieldGenericExecutor executor)
			=> executor.Execute<PropertyInfoField, T>(this);
	}
}
