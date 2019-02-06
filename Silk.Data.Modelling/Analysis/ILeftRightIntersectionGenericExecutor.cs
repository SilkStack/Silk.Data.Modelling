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
		void Execute<TLeftField, TRightField, TLeftData, TRightData>(IntersectedFields<TLeftField, TRightField, TLeftData, TRightData> intersectedFields)
			where TLeftField : IField
			where TRightField : IField;
	}
}
