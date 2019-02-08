using Silk.Data.Modelling.GenericDispatch;
using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data structure field.
	/// </summary>
	public interface IField
	{
		/// <summary>
		/// Gets the field's name.
		/// </summary>
		string FieldName { get; }

		/// <summary>
		/// Gets a value indicating if the field is readable.
		/// </summary>
		bool CanRead { get; }

		/// <summary>
		/// Gets a value indicating if the field is writable.
		/// </summary>
		bool CanWrite { get; }

		/// <summary>
		/// Gets a value indicating if the field's data type is an enumerable type.
		/// </summary>
		bool IsEnumerableType { get; }

		/// <summary>
		/// Gets the field's data type.
		/// </summary>
		Type FieldDataType { get; }

		/// <summary>
		/// Gets the field's element type if it's an enumerable type.
		/// </summary>
		Type FieldElementType { get; }

		/// <summary>
		/// Dispatch a genericly-typed method call using the fields generic type parameters.
		/// </summary>
		/// <param name="executor"></param>
		void Dispatch(IFieldGenericExecutor executor);
	}

	public static class FieldExtensions
	{
		/// <summary>
		/// Gets the Type of the field without the generic enumerable type if there is one.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Type RemoveEnumerableType(this IField field)
		{
			if (field.IsEnumerableType)
				return field.FieldElementType;
			return field.FieldDataType;
		}
	}
}
