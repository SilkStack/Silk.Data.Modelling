using System;

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
		/// Gets an enumerator for a field.
		/// </summary>
		/// <param name="fieldPath"></param>
		/// <returns></returns>
		IGraphReaderEnumerator<TModel, TField> GetEnumerator(IFieldPath<TModel, TField> fieldPath);

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

	/// <summary>
	/// Reads from an enumerable within a model graph.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	/// <typeparam name="TField"></typeparam>
	public interface IGraphReaderEnumerator<TModel, TField> : IDisposable
		where TField : class, IField
		where TModel : IModel<TField>
	{
		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns></returns>
		bool MoveNext();

		/// <summary>
		/// Gets the graph reader for the collection at the current position of the enumerator.
		/// </summary>
		IGraphReader<TModel, TField> Current { get; }
	}
}
