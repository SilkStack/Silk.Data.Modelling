using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	/// <summary>
	/// Finds intersect candidates between two models.
	/// </summary>
	public interface IIntersectCandidateSource
	{
		IEnumerable<IntersectCandidate> GetIntersectCandidates(IModel leftModel, IModel rightModel);
	}

	/// <summary>
	/// Finds intersect candidates between two models.
	/// </summary>
	public interface ISpecializedIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		IEnumerable<SpecializedIntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> GetIntersectCandidates(
			TLeftModel leftModel, TRightModel rightModel
			);
	}
}
