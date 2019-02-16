using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Analyzes two TypeModels to generate a TypeToTypeIntersection.
	/// </summary>
	public class TypeToTypeIntersectionAnalyzer : DefaultIntersectionAnalyzer<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeIntersectionAnalyzer(
			IEnumerable<IIntersectCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> intersectCandidateSources = null,
			IEnumerable<IIntersectionRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> intersectionRules = null
			) : base(intersectCandidateSources, intersectionRules)
		{
		}
	}
}
