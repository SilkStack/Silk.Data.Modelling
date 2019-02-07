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

		/// <summary>
		/// Creates a container at the specified path.
		/// </summary>
		/// <param name="fieldPath"></param>
		void CreateContainer(IFieldPath<TModel, TField> fieldPath);

		/// <summary>
		/// Checks the provided path in the model graph for accessability.
		/// </summary>
		/// <param name="fieldPath"></param>
		/// <returns></returns>
		bool CheckPath(IFieldPath<TModel, TField> fieldPath);
	}
}
