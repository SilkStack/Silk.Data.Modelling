using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class CopySameTypes : IMappingConvention
	{
		public static CopySameTypes Instance { get; } = new CopySameTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var fromField in fromModel.Fields.Where(q => q.CanRead))
			{
				var toField = toModel.Fields.FirstOrDefault(field => field.CanWrite &&
					field.FieldName == fromField.FieldName &&
					field.FieldType == fromField.FieldType);
				if (toField == null)
					continue;
				builder
					.Bind(toField)
					.From(fromField)
					.Using<CopyBinding>();
			}
		}
	}
}
