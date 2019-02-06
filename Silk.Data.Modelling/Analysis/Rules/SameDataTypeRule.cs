using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.Analysis.Rules
{
	public class SameDataTypeRule<TLeftField, TRightField> : IIntersectionRule<TLeftField, TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		public bool IsValidIntersection(
			IntersectCandidate intersectCandidate,
			out IntersectedFields<TLeftField, TRightField> intersectedFields
			)
		{
			if (intersectCandidate.LeftField.FieldDataType != intersectCandidate.RightField.FieldDataType)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = new IntersectedFields<TLeftField, TRightField>();
			return true;
		}
	}
}
