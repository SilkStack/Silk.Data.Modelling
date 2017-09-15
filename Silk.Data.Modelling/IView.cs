namespace Silk.Data.Modelling
{
	/// <summary>
	/// Represents a view of a data structure, derived from a Model.
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the model the view is based on.
		/// </summary>
		Model Model { get; }
	}

	/// <summary>
	/// Represents a view of a data structure, derived from a Model.
	/// </summary>
	public interface IView<TField> : IView
		where TField : IViewField
	{
		/// <summary>
		/// Gets an array of fields present on the view.
		/// </summary>
		TField[] Fields { get; }
	}

	public interface IView<TField, TSource> : IView<TField>
		where TField : IViewField
	{
		new TypedModel<TSource> Model { get; }
	}

	public interface IView<TField, TSource, TView> : IView<TField, TSource>
		where TField : IViewField
	{
	}
}
