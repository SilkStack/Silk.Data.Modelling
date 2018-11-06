using Silk.Data.Modelling.Mapping;
using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Base interface for all models.
	/// </summary>
	/// <remarks>Implementing this interface directly is not supported. Implement <see cref="IModel{TField}"/>.</remarks>
	public interface IModel
	{
		/// <summary>
		/// Gets an array of fields on the model.
		/// </summary>
		IField[] Fields { get; }

		/// <summary>
		/// Performs all relevant calls to the given <see cref="IModelTransformer"/> to transform the model type.
		/// </summary>
		/// <param name="transformer"></param>
		void Transform(IModelTransformer transformer);

		/// <summary>
		/// Resolves a path to the specified model node.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		ModelNode ResolveNode(Span<string> path);

		/// <summary>
		/// Gets a field reference for a source field.
		/// </summary>
		/// <param name="sourceField"></param>
		/// <returns></returns>
		IFieldReference GetFieldReference(ISourceField sourceField);

		/// <summary>
		/// Gets a field reference for a target field.
		/// </summary>
		/// <param name="targetField"></param>
		/// <returns></returns>
		IFieldReference GetFieldReference(ITargetField targetField);

		/// <summary>
		/// Creates a new field resolver.
		/// </summary>
		/// <returns></returns>
		IFieldResolver CreateFieldResolver();
	}

	/// <summary>
	/// A model with fields of type <see cref="TField"/>.
	/// </summary>
	/// <typeparam name="TField">The type of field the model uses.</typeparam>
	public interface IModel<TField> : IModel
		where TField : IField
	{
		/// <summary>
		/// Gets an array of fields on the model.
		/// </summary>
		new TField[] Fields { get; }
	}
}
