using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule for matched data types.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class SameDataTypeRule<TLeftModel, TLeftField, TRightModel, TRightField> : IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public bool IsValidIntersection(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate,
			out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields
			)
		{
			if (intersectCandidate.LeftField.FieldDataType != intersectCandidate.RightField.FieldDataType)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>.Create(
				intersectCandidate.LeftField, intersectCandidate.RightField,
				intersectCandidate.LeftPath, intersectCandidate.RightPath
				);
			return true;
		}
	}
}
