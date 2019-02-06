namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	public class IntersectCandidate
	{
		public IField LeftField { get; }
		public IField RightField { get; }

		public IntersectCandidate(IField leftField, IField rightField)
		{
			LeftField = leftField;
			RightField = rightField;
		}
	}

	public class SpecializedIntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> : IntersectCandidate
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		public new TLeftField LeftField { get; }
		public new TRightField RightField { get; }

		public SpecializedIntersectCandidate(TLeftField leftField, TRightField rightField) :
			base(leftField, rightField)
		{
			LeftField = leftField;
			RightField = rightField;
		}
	}
}
