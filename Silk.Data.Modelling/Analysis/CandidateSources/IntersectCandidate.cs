namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	public class IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		public TLeftModel LeftModel { get; }
		public TLeftField LeftField { get; }
		public TRightModel RightModel { get; }
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
