using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class CopySameTypes : IMappingConvention
	{
		public void CreateMappings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var fromField in fromModel.Fields)
			{
				var toField = toModel.Fields.FirstOrDefault(field => field.FieldName == fromField.FieldName &&
					field.FieldType == fromField.FieldType);
				if (toField == null)
					continue;
				builder
					.For(toField)
					.MapFrom(fromField)
					.WithCopyBinding();
			}
		}
	}
}
