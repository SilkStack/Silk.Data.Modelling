namespace Silk.Data.Modelling
{
	/// <summary>
	/// Represents a view of a data structure, derived from a Model.
	/// </summary>
	public interface IView { }

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
	}

	public interface IView<TField, TSource, TView> : IView<TField, TSource>
		where TField : IViewField
	{
	}
}
