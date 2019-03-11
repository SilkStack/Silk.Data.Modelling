using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Analysis
{
	public class DefaultIntersectionAnalyzer<TLeftModel, TLeftField, TRightModel, TRightField> :
		IntersectAnalyzerBase<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public DefaultIntersectionAnalyzer(
			IEnumerable<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>> intersectCandidateSources = null,
			IEnumerable<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>> intersectionRules = null
			)
		{
			AddCandidateSources(intersectCandidateSources);
			AddIntersectionRules(intersectionRules);
		}

		public void AddTypeConverters(IEnumerable<ITypeConverter> typeConverters)
		{
			IntersectionRules.Add(
				new TypeConverterRule<TLeftModel, TLeftField, TRightModel, TRightField>(typeConverters)
				);
		}

		protected override IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> CreateIntersection(
			IntersectAnalysis analysis
			)
			=> new Intersection(analysis.LeftModel, analysis.RightModel, analysis.IntersectedFields.ToArray());

		protected virtual IEnumerable<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>> GetDefaultRules()
		{
			yield return new SameDataTypeRule<TLeftModel, TLeftField, TRightModel, TRightField>();
			yield return new BothNumericTypesRule<TLeftModel, TLeftField, TRightModel, TRightField>();
			yield return new ConvertableWithToStringRule<TLeftModel, TLeftField, TRightModel, TRightField>();
			yield return new ExplicitCastRule<TLeftModel, TLeftField, TRightModel, TRightField>();
			yield return new ConvertableWithTryParse<TLeftModel, TLeftField, TRightModel, TRightField>();
		}

		protected virtual IEnumerable<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>> GetDefaultCandidateSources()
		{
			yield return new ExactPathMatchCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>();
			yield return new FlattenedNameMatchCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>();
		}

		private void AddCandidateSources(IEnumerable<IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>> intersectCandidateSources)
		{
			foreach (var item in intersectCandidateSources ?? GetDefaultCandidateSources())
				CandidateSources.Add(item);
		}

		private void AddIntersectionRules(IEnumerable<IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>> intersectionRules)
		{
			foreach (var item in intersectionRules ?? GetDefaultRules())
				IntersectionRules.Add(item);
		}

		public class Intersection :
			IntersectionBase<TLeftModel, TLeftField, TRightModel, TRightField>
		{
			public Intersection(TLeftModel leftModel, TRightModel rightModel,
				IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>[] intersectedFields) :
				base(leftModel, rightModel, intersectedFields)
			{
			}
		}
	}
}
