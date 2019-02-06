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
		where TLeftField : IField
		where TRightField : IField
	{
		public ICollection<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>> CandidateSources { get; }
			= new List<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>>();

		public ICollection<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>> IntersectionRules { get; }
			= new List<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>>();

		protected virtual IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>[] GetIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
			=> CandidateSources.SelectMany(source => source.GetIntersectCandidates(leftModel, rightModel)).ToArray();

		protected virtual IntersectAnalysis CreateAnalysis(TLeftModel leftModel, TRightModel rightModel)
			=> new IntersectAnalysis(leftModel, rightModel);

		protected abstract IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> CreateIntersection(IntersectAnalysis analysis);

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

		protected class IntersectAnalysis
		{
			private List<IntersectedFields<TLeftField, TRightField>> _intersectedFields
				= new List<IntersectedFields<TLeftField, TRightField>>();

			public TLeftModel LeftModel { get; }
			public TRightModel RightModel { get; }
			public IReadOnlyList<IntersectedFields<TLeftField, TRightField>> IntersectedFields => _intersectedFields;

			public IntersectAnalysis(TLeftModel leftModel, TRightModel rightModel)
			{
				LeftModel = leftModel;
				RightModel = rightModel;
			}

			public bool IsLeftFieldAlreadyPaired(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate)
			{
				return false;
			}

			public void AddIntersectedFields(IntersectedFields<TLeftField, TRightField> intersectedFields)
			{
				_intersectedFields.Add(intersectedFields);
			}
		}
	}
}
