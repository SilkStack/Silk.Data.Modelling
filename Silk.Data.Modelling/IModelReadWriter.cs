using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Reads and writes model fields to a store instance.
	/// </summary>
	public interface IModelReadWriter
	{
		/// <summary>
		/// Gets the model.
		/// </summary>
		IModel Model { get; }

		/// <summary>
		/// Gets the field resolver used to resolve field nodes on Model.
		/// </summary>
		IFieldResolver FieldResolver { get; }

		/// <summary>
		/// Reads a given field's value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <returns></returns>
		T ReadField<T>(IFieldReference field);

		/// <summary>
		/// Writes a value to a given field.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <param name="value"></param>
		void WriteField<T>(IFieldReference field, T value);
	}
}
