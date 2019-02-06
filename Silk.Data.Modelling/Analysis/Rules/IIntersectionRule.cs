using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.Analysis.Rules
{
	public interface IIntersectionRule<TLeftField, TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		bool IsValidIntersection(IntersectCandidate intersectCandidate, out IntersectedFields<TLeftField, TRightField> intersectedFields);
	}

	public interface ISpecializedIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		bool IsValidIntersection(
			SpecializedIntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate,
			out IntersectedFields<TLeftField, TRightField> intersectedFields
			);
	}
}
