namespace Silk.Data.Modelling.Analysis
{
	public abstract class IntersectionBase<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersection<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public TLeftModel LeftModel { get; }

		public TRightModel RightModel { get; }

		IModel IIntersection.LeftModel => LeftModel;

		IModel IIntersection.RightModel => RightModel;

		public IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>[] IntersectedFields { get; }

		IntersectedFields[] IIntersection.IntersectedFields => IntersectedFields;

		protected IntersectionBase(TLeftModel leftModel, TRightModel rightModel,
			IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>[] intersectedFields)
		{
			LeftModel = leftModel;
			RightModel = rightModel;
			IntersectedFields = intersectedFields;
		}
	}
}
