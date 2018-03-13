using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	public class ConvertToStringWithToString : IMappingConvention
	{
		public static ConvertToStringWithToString Instance { get; } = new ConvertToStringWithToString();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var (fromField, toField) in ConventionUtilities.GetBindCandidatePairs(fromModel, toModel, builder))
			{
				var (fromType, toType) = ConventionUtilities.GetCompareTypes(fromField, toField);
				if (toType != typeof(string))
					continue;
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<ToStringBinding>();
			}
		}
	}
}