using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Analyzes two models to generate an intersection.
	/// </summary>
	public abstract class IntersectAnalyzerBase<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersectionAnalyzer<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Gets a writable collection of intersect candidate sources.
		/// </summary>
		public ICollection<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>> CandidateSources { get; }
			= new List<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>>();

		/// <summary>
		/// Gets a wrtiable collection of intersection rules.
		/// </summary>
		public ICollection<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>> IntersectionRules { get; }
			= new List<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>>();

		/// <summary>
		/// Get all intersection candidates from candidate sources.
		/// </summary>
		/// <param name="leftModel"></param>
		/// <param name="rightModel"></param>
		/// <returns></returns>
		protected virtual IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>[] GetIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
			=> CandidateSources.SelectMany(source => source.GetIntersectCandidates(leftModel, rightModel)).ToArray();

		/// <summary>
		/// Create an IntersectAnalysis object to hold the analysis state.
		/// </summary>
		/// <param name="leftModel"></param>
		/// <param name="rightModel"></param>
		/// <returns></returns>
		protected virtual IntersectAnalysis CreateAnalysis(TLeftModel leftModel, TRightModel rightModel)
			=> new IntersectAnalysis(leftModel, rightModel);

		/// <summary>
		/// Factory to produce the final intersection instance.
		/// </summary>
		/// <param name="analysis"></param>
		/// <returns></returns>
		protected abstract IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> CreateIntersection(IntersectAnalysis analysis);

		/// <summary>
		/// Populate the IntersectAnalysis with IntersectFields from valid rules matches from the provided candidates.
		/// </summary>
		/// <param name="analysis"></param>
		/// <param name="intersectCandidates"></param>
		protected virtual void ApplyValidIntersectionCandidates(IntersectAnalysis analysis, IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>[] intersectCandidates)
		{
			foreach (var candidate in intersectCandidates)
			{
				if (analysis.IsLeftFieldAlreadyPaired(candidate))
					continue;

				foreach (var rule in IntersectionRules)
				{
					if (rule.IsValidIntersection(candidate, out var intersectedFields))
					{
						analysis.AddIntersectedFields(intersectedFields);
						break;
					}
				}
			}
		}

		public IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> CreateIntersection(
			TLeftModel leftModel, TRightModel rightModel
			)
		{
			var intersectCandidates = GetIntersectCandidates(leftModel, rightModel);
			var analysis = CreateAnalysis(leftModel, rightModel);

			ApplyValidIntersectionCandidates(analysis, intersectCandidates);

			return CreateIntersection(analysis);
		}

		/// <summary>
		/// Analysis state object.
		/// </summary>
		protected class IntersectAnalysis
		{
			private List<IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>> _intersectedFields
				= new List<IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>>();

			public TLeftModel LeftModel { get; }
			public TRightModel RightModel { get; }
			public IReadOnlyList<IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>> IntersectedFields => _intersectedFields;

			public IntersectAnalysis(TLeftModel leftModel, TRightModel rightModel)
			{
				LeftModel = leftModel;
				RightModel = rightModel;
			}

			public virtual bool IsLeftFieldAlreadyPaired(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate)
				=> _intersectedFields.Any(intersectedFields => ReferenceEquals(intersectedFields.LeftField, intersectCandidate.LeftField));

			public virtual void AddIntersectedFields(IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
				=>_intersectedFields.Add(intersectedFields);
		}
	}
}
