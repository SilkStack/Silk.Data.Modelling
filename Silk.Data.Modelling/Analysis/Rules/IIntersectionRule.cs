using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.Analysis.Rules
{
	public interface IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		bool IsValidIntersection(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate,
			out IntersectedFields<TLeftField, TRightField> intersectedFields
			);
	}
}
