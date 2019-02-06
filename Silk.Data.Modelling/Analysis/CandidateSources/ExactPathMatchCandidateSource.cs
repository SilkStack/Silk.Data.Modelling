using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	/// <summary>
	/// Finds intersect candidates with an exact path match.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class ExactPathMatchCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		public IEnumerable<IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> GetIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
		{
			//  todo: descend the field heirarchy to find exact path matches

			foreach (var leftField in leftModel.Fields)
			{
				foreach (var rightField in rightModel.Fields)
				{
					if (leftField.FieldName == rightField.FieldName)
						yield return new IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>(
							leftModel, leftField,
							rightModel, rightField
							);
				}
			}
		}
	}
}
