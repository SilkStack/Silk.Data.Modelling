namespace Silk.Data.Modelling.GenericDispatch
{
	/// <summary>
	/// Interface for executing generic typed code from a field.
	/// </summary>
	public interface IFieldGenericExecutor
	{
		/// <summary>
		/// Execute generic typed code.
		/// </summary>
		/// <typeparam name="TField"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="field"></param>
		void Execute<TField, TData>(IField field)
			where TField : class, IField;
	}
}
