using System.Linq;
using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	public class ConvertToStringWithToString : IMappingConvention
	{
		public static ConvertToStringWithToString Instance { get; } = new ConvertToStringWithToString();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var (fromField, toField) in ConventionUtilities.GetBindCandidatePairs(fromModel, toModel, builder)
				.Where(q => q.targetField.FieldType == typeof(string)))
			{
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<ToStringBinding>();
			}
		}
	}
}