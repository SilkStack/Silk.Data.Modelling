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
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Gets or sets a maximum depth to search for candidates.
		/// Defaults to 10.
		/// </summary>
		public int MaxDepth { get; set; } = 10;

		public IEnumerable<IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> GetIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
		{
			return SearchFields(
				leftModel.Fields,
				rightModel.Fields,
				new FieldPath<TLeftModel, TLeftField>(leftModel, default(TLeftField), new TLeftField[0]),
				new FieldPath<TRightModel, TRightField>(rightModel, default(TRightField), new TRightField[0]),
				0
				);

			IEnumerable<IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> SearchFields(
				IEnumerable<TLeftField> leftFields,
				IEnumerable<TRightField> rightFields,
				FieldPath<TLeftModel, TLeftField> leftPath,
				FieldPath<TRightModel, TRightField> rightPath,
				int depth
				)
			{
				if (depth == MaxDepth)
					yield break;

				foreach (var leftField in leftFields)
				{
					foreach (var rightField in rightFields)
					{
						if (leftField.FieldName == rightField.FieldName)
						{
							var newLeftPath = leftPath.Child(leftField);
							var newRightPath = rightPath.Child(rightField);
							yield return new IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>(
								newLeftPath, newRightPath,
								typeof(ExactPathMatchCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>)
								);

							foreach (var subCandidate in SearchFields(leftModel.GetPathFields(newLeftPath), rightModel.GetPathFields(newRightPath), newLeftPath, newRightPath, depth + 1))
								yield return subCandidate;
						}
					}
				}
			}
		}
	}
}
