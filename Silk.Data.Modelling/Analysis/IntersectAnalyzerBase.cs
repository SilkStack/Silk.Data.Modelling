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
		public ICollection<IIntersectCandidateSource> CandidateSources { get; }
			= new List<IIntersectCandidateSource>();
		public ICollection<ISpecializedIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>> SpecializedCandidateSources { get; }
			= new List<ISpecializedIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>>();

		public ICollection<IIntersectionRule<TLeftField, TRightField>> IntersectionRules { get; }
			= new List<IIntersectionRule<TLeftField, TRightField>>();
		public ICollection<ISpecializedIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>> SpecializedIntersectionRules { get; }
			= new List<ISpecializedIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>>();

		protected virtual IntersectCandidate[] GetIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
			=> CandidateSources.SelectMany(source => source.GetIntersectCandidates(leftModel, rightModel)).ToArray();

		protected virtual SpecializedIntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>[] GetSpecializedIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
			=> SpecializedCandidateSources.SelectMany(source => source.GetIntersectCandidates(leftModel, rightModel)).ToArray();

		protected virtual IntersectAnalysis CreateAnalysis(TLeftModel leftModel, TRightModel rightModel)
			=> new IntersectAnalysis(leftModel, rightModel);

		protected abstract IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> CreateIntersection(IntersectAnalysis analysis);

		protected virtual void ApplyValidSpecializedIntersectionCandidates(
			IntersectAnalysis analysis,
			SpecializedIntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>[] intersectCandidates
			)
		{
			foreach (var candidate in intersectCandidates)
			{
				if (analysis.IsLeftFieldAlreadyPaired(candidate))
					continue;

				foreach (var rule in SpecializedIntersectionRules)
				{
					if (rule.IsValidIntersection(candidate, out var intersectedFields))
					{
						analysis.AddIntersectedFields(intersectedFields);
						break;
					}
				}
			}

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

		protected virtual void ApplyValidIntersectionCandidates(IntersectAnalysis analysis, IntersectCandidate[] intersectCandidates)
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
			var specializedCandidates = GetSpecializedIntersectCandidates(leftModel, rightModel);
			var analysis = CreateAnalysis(leftModel, rightModel);

			ApplyValidSpecializedIntersectionCandidates(analysis, specializedCandidates);
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

			public bool IsLeftFieldAlreadyPaired(IntersectCandidate intersectCandidate)
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
