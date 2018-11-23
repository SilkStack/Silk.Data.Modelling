using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A field on a model.
	/// </summary>
	/// <remarks>Implementing this field directly is not supported, implement <see cref="IField{T}"/>.</remarks>
	public interface IField
	{
		/// <summary>
		/// Gets the field's name.
		/// </summary>
		string FieldName { get; }

		/// <summary>
		/// Gets the <see cref="Type"/> of the data stored in the field.
		/// </summary>
		Type FieldType { get; }

		/// <summary>
		/// Gets a value indicating if the field can be read from.
		/// </summary>
		bool CanRead { get; }

		/// <summary>
		/// Gets a value indicating if the field can be written to.
		/// </summary>
		bool CanWrite { get; }

		/// <summary>
		/// Gets a value indicating if <see cref="FieldType"/> is an enumerable type.
		/// </summary>
		bool IsEnumerable { get; }

		/// <summary>
		/// When <see cref="IsEnumerable"/> is true returns the <see cref="Type"/> contained in the enumerable type.
		/// </summary>
		/// <remarks>Returns null when <see cref="IsEnumerable"/> is false.</remarks>
		Type ElementType { get; }

		/// <summary>
		/// Gets the <see cref="TypeModel"/> of <see cref="FieldType"/>.
		/// </summary>
		TypeModel FieldTypeModel { get; }

		/// <summary>
		/// Gets a <see cref="IModel"/> that is suitable for modelling operations for the field.
		/// </summary>
		IModel FieldModel { get; }

		/// <summary>
		/// Performs all relevant calls to the given <see cref="IModelTransformer"/> as part of the model transformation process.
		/// </summary>
		/// <param name="transformer"></param>
		void Transform(IModelTransformer transformer);
	}

	/// <summary>
	/// A field on a model with a <see cref="IField.FieldType"/> of <see cref="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of data stored in the field.</typeparam>
	public interface IField<T> : IField
	{
		/// <summary>
		/// Gets the <see cref="TypeModel"/> of <see cref="FieldType"/>.
		/// </summary>
		new TypeModel<T> FieldTypeModel { get; }
	}
}
