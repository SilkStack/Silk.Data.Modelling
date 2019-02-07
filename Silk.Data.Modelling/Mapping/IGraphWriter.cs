namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Writes fields to a graph.
	/// </summary>
	public interface IGraphWriter<TModel, TField>
		where TField : class, IField
		where TModel : IModel<TField>
	{
		/// <summary>
		/// Write a value to the model graph.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldPath"></param>
		/// <param name="value"></param>
		void Write<T>(IFieldPath<TModel, TField> fieldPath, T value);
	}
}
