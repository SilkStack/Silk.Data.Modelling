namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Interface for executing generic typed code.
	/// </summary>
	public interface ILeftRightIntersectionGenericExecutor
	{
		/// <summary>
		/// Execute generic typed code.
		/// </summary>
		/// <typeparam name="TLeftField"></typeparam>
		/// <typeparam name="TRightField"></typeparam>
		/// <typeparam name="TLeftData"></typeparam>
		/// <typeparam name="TRightData"></typeparam>
		/// <param name="intersectedFields"></param>
		void Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields)
			where TLeftField : class, IField
			where TRightField : class, IField
			where TLeftModel : IModel<TLeftField>
			where TRightModel : IModel<TRightField>;
	}
}
