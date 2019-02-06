namespace Silk.Data.Modelling.Analysis
{
	public abstract class IntersectionBase<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersection<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		public TLeftModel LeftModel { get; }

		public TRightModel RightModel { get; }

		IModel IIntersection.LeftModel => LeftModel;

		IModel IIntersection.RightModel => RightModel;

		public IntersectedFields<TLeftField, TRightField>[] IntersectedFields { get; }

		IntersectedFields[] IIntersection.IntersectedFields => IntersectedFields;

		protected IntersectionBase(TLeftModel leftModel, TRightModel rightModel,
			IntersectedFields<TLeftField, TRightField>[] intersectedFields)
		{
			LeftModel = leftModel;
			RightModel = rightModel;
			IntersectedFields = intersectedFields;
		}
	}
}
