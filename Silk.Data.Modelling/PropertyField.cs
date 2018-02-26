using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A field that was derived from a property on a <see cref="Type"/>.
	/// </summary>
	public interface IPropertyField : IField
	{
	}

	public class PropertyField<T> : FieldBase<T>, IPropertyField, IField<T>
	{
		public PropertyField(string fieldName, bool canRead, bool canWrite, bool isEnumerable, Type elementType) :
			base(fieldName, canRead, canWrite, isEnumerable, elementType)
		{
		}
	}
}
