namespace Silk.Data.Modelling.GenericDispatch
{
	/// <summary>
	/// Interface for executing generic typed code from a model.
	/// </summary>
	public interface IModelGenericExecutor
	{
		/// <summary>
		/// Execute generic typed code.
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TField"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="model"></param>
		void Execute<TModel, TField, TData>(TModel model)
			where TField : class, IField
			where TModel : IModel<TField>;
	}
}
