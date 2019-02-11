using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.GenericDispatch
{
	/// <summary>
	/// Interface for executing generic typed code from an intersected field.
	/// </summary>
	public interface IIntersectedFieldsGenericExecutor
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
