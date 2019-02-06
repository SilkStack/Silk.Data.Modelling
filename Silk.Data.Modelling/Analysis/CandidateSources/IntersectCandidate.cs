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
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Gets the FieldPath for the left-side of the candidate.
		/// </summary>
		public FieldPath<TLeftModel, TLeftField> LeftPath { get; }

		/// <summary>
		/// Gets the FieldPath for the right-side of the candidate.
		/// </summary>
		public FieldPath<TRightModel, TRightField> RightPath { get; }

		/// <summary>
		/// Gets the LeftModel that LeftField is from.
		/// </summary>
		public TLeftModel LeftModel => LeftPath.Model;
		/// <summary>
		/// Gets the LeftField of the candidate.
		/// </summary>
		public TLeftField LeftField => LeftPath.FinalField;
		/// <summary>
		/// Gets the RightModel that RightField is from.
		/// </summary>
		public TRightModel RightModel => RightPath.Model;
		/// <summary>
		/// Gets the RightField of the candidate.
		/// </summary>
		public TRightField RightField => RightPath.FinalField;

		public IntersectCandidate(FieldPath<TLeftModel, TLeftField> leftPath, FieldPath<TRightModel, TRightField> rightPath)
		{
			LeftPath = leftPath;
			RightPath = rightPath;
		}
	}
}
