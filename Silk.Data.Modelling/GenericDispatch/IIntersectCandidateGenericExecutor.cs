using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.GenericDispatch
{
	public interface IIntersectCandidateGenericExecutor
	{
		void Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectCandidate
			)
			where TLeftModel : IModel<TLeftField>
			where TRightModel : IModel<TRightField>
			where TLeftField : class, IField
			where TRightField : class, IField;
	}
}
