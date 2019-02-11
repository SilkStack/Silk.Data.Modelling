using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	/// <summary>
	/// Finds intersect candidates between two models.
	/// </summary>
	public interface IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Get an enumerable of intersect candidates between the two provided models.
		/// </summary>
		/// <param name="leftModel"></param>
		/// <param name="rightModel"></param>
		/// <returns></returns>
		IEnumerable<IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> GetIntersectCandidates(
			TLeftModel leftModel, TRightModel rightModel
			);
	}
}
