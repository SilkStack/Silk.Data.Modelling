namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Reads fields from a graph.
	/// </summary>
	public interface IGraphReader<TModel, TField>
		where TField : class, IField
		where TModel : IModel<TField>
	{
		/// <summary>
		/// Reads a field from the model graph.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldPath"></param>
		/// <returns></returns>
		T Read<T>(IFieldPath<TModel, TField> fieldPath);

		/// <summary>
		/// Checks for a valid container at the provided path on the model graph.
		/// </summary>
		/// <param name="fieldPath"></param>
		/// <returns></returns>
		bool CheckContainer(IFieldPath<TModel, TField> fieldPath);

		/// <summary>
		/// Checks the provided path in the model graph for accessability.
		/// </summary>
		/// <param name="fieldPath"></param>
		/// <returns></returns>
		bool CheckPath(IFieldPath<TModel, TField> fieldPath);
	}
}
