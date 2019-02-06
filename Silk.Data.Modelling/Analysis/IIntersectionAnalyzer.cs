namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Analyzes two models to generate an intersection.
	/// </summary>
	public interface IIntersectionAnalyzer
	{
	}

	/// <summary>
	/// Analyzes two models to generate an intersection.
	/// </summary>
	public interface IIntersectionAnalyzer<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		/// <summary>
		/// Create an intersection between two models.
		/// </summary>
		/// <param name="leftModel"></param>
		/// <param name="rightModel"></param>
		/// <returns></returns>
		IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> CreateIntersection(TLeftModel leftModel, TRightModel rightModel);
	}
}
