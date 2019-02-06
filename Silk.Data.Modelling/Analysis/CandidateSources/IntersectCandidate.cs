namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	/// <summary>
	/// Intersect candidate between two models.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		/// <summary>
		/// Gets the LeftModel that LeftField is from.
		/// </summary>
		public TLeftModel LeftModel { get; }
		/// <summary>
		/// Gets the LeftField of the candidate.
		/// </summary>
		public TLeftField LeftField { get; }
		/// <summary>
		/// Gets the RightModel that RightField is from.
		/// </summary>
		public TRightModel RightModel { get; }
		/// <summary>
		/// Gets the RightField of the candidate.
		/// </summary>
		public TRightField RightField { get; }

		public IntersectCandidate(TLeftModel leftModel, TLeftField leftField,
			TRightModel rightModel, TRightField rightField)
		{
			LeftModel = leftModel;
			LeftField = leftField;
			RightModel = rightModel;
			RightField = rightField;
		}
	}
}
