namespace Silk.Data.Modelling.Analysis
{
	public abstract class IntersectedFields
	{
		public IField LeftField { get; }
		public IField RightField { get; }

		protected IntersectedFields(IField leftField, IField rightField)
		{
			LeftField = leftField;
			RightField = rightField;
		}
	}

	/// <summary>
	/// Intersection of two fields.
	/// </summary>
	public class IntersectedFields<TLeftField, TRightField> : IntersectedFields
		where TLeftField : IField
		where TRightField : IField
	{
		public new TLeftField LeftField { get; }
		public new TRightField RightField { get; }

		public IntersectedFields(TLeftField leftField, TRightField rightField)
			: base(leftField, rightField)
		{
			LeftField = leftField;
			RightField = rightField;
		}
	}
}
