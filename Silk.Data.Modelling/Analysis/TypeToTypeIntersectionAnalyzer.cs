using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Analyzes two TypeModels to generate a TypeToTypeIntersection.
	/// </summary>
	public class TypeToTypeIntersectionAnalyzer : IntersectAnalyzerBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeIntersectionAnalyzer(
			IEnumerable<IIntersectCandidateSource> intersectCandidateSources = null,
			IEnumerable<IIntersectionRule<PropertyInfoField, PropertyInfoField>> intersectionRules = null
			)
		{
			AddCandidateSources(intersectCandidateSources);
			AddIntersectionRules(intersectionRules);
		}

		private void AddCandidateSources(IEnumerable<IIntersectCandidateSource> intersectCandidateSources)
		{
			if (intersectCandidateSources == null)
			{
				CandidateSources.Add(new ExactNameMatchCandidateSource());
				CandidateSources.Add(new FlattenedNameMatchCandidateSource());

				return;
			}

			foreach (var item in intersectCandidateSources)
				CandidateSources.Add(item);
		}

		private void AddIntersectionRules(IEnumerable<IIntersectionRule<PropertyInfoField, PropertyInfoField>> intersectionRules)
		{
			if (intersectionRules == null)
			{
				IntersectionRules.Add(new SameDataTypeRule<PropertyInfoField, PropertyInfoField>());

				return;
			}

			foreach (var item in intersectionRules)
				IntersectionRules.Add(item);
		}

		protected override IIntersection<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> CreateIntersection(
			IntersectAnalysis analysis)
		{
			return new TypeToTypeIntersection(analysis.LeftModel, analysis.RightModel);
		}
	}
}
