using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Analyzes two TypeModels to generate a TypeToTypeIntersection.
	/// </summary>
	public class TypeToTypeIntersectionAnalyzer : IntersectAnalyzerBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeIntersectionAnalyzer(
			IEnumerable<IIntersectCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> intersectCandidateSources = null,
			IEnumerable<IIntersectionRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> intersectionRules = null
			)
		{
			AddCandidateSources(intersectCandidateSources);
			AddIntersectionRules(intersectionRules);
		}

		private void AddCandidateSources(IEnumerable<IIntersectCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> intersectCandidateSources)
		{
			if (intersectCandidateSources == null)
			{
				CandidateSources.Add(new ExactPathMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
				CandidateSources.Add(new FlattenedNameMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());

				return;
			}

			foreach (var item in intersectCandidateSources)
				CandidateSources.Add(item);
		}

		private void AddIntersectionRules(IEnumerable<IIntersectionRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> intersectionRules)
		{
			if (intersectionRules == null)
			{
				IntersectionRules.Add(new SameDataTypeRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
				IntersectionRules.Add(new BothNumericTypesRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
				IntersectionRules.Add(new ConvertableWithToStringRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
				IntersectionRules.Add(new ExplicitCastRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
				IntersectionRules.Add(new ConvertableWithTryParse<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());

				return;
			}

			foreach (var item in intersectionRules)
				IntersectionRules.Add(item);
		}

		protected override IIntersection<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> CreateIntersection(
			IntersectAnalysis analysis)
		{
			return new TypeToTypeIntersection(analysis.LeftModel, analysis.RightModel, analysis.IntersectedFields.ToArray());
		}
	}
}
