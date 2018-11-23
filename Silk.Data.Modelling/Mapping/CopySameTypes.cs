using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class CopySameTypes : IMappingConvention
	{
		public static CopySameTypes Instance { get; } = new CopySameTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var (fromField, toField) in builder.BindingCandidatesDelegate(fromModel, toModel, builder)
				.Where(q => AreTypesCompatible(q.sourceField, q.targetField)))
			{
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<CopyBinding>();
			}
		}

		private bool AreTypesCompatible(ISourceField sourceField, ITargetField targetField)
		{
			var compareTypes = ConventionUtilities.GetCompareTypes(sourceField, targetField);
			return compareTypes.sourceType == compareTypes.targetType;
		}
	}
}
