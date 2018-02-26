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
