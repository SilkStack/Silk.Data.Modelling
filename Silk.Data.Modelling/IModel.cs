using Silk.Data.Modelling.GenericDispatch;
using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data structure model.
	/// </summary>
	public interface IModel
	{
		/// <summary>
		/// Gets a collection of fields present in the data structure.
		/// </summary>
		IReadOnlyList<IField> Fields { get; }

		/// <summary>
		/// Dispatch a genericly-typed method call using the models generic type parameters.
		/// </summary>
		/// <param name="executor"></param>
		void Dispatch(IModelGenericExecutor executor);
	}

	/// <summary>
	/// Data structure model.
	/// </summary>
	public interface IModel<TField> : IModel
		where TField : IField
	{
		/// <summary>
		/// Gets a collection of fields present in the data structure.
		/// </summary>
		new IReadOnlyList<TField> Fields { get; }

		/// <summary>
		/// Enumerates fields found at the given path relative to the model.
		/// </summary>
		/// <param name="fieldPath"></param>
		/// <returns></returns>
		IEnumerable<TField> GetPathFields(IFieldPath<TField> fieldPath);
	}
}
