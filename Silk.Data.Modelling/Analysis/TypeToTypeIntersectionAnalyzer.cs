using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;

namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Analyzes two TypeModels to generate a TypeToTypeIntersection.
	/// </summary>
	public class TypeToTypeIntersectionAnalyzer : IntersectAnalyzerBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeIntersectionAnalyzer()
		{
			CandidateSources.Add(new ExactNameMatchCandidateSource());
			CandidateSources.Add(new FlattenedNameMatchCandidateSource());

			IntersectionRules.Add(new SameDataTypeRule<PropertyInfoField, PropertyInfoField>());
		}

		protected override IIntersection<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> CreateIntersection(
			IntersectAnalysis analysis)
		{
			return new TypeToTypeIntersection(analysis.LeftModel, analysis.RightModel);
		}
	}
}
