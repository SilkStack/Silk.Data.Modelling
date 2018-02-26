using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class CopySameTypes : IMappingConvention
	{
		public static CopySameTypes Instance { get; } = new CopySameTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q)))
			{
				var fromField = fromModel.Fields.FirstOrDefault(field => field.CanWrite &&
					field.FieldName == toField.FieldName &&
					field.FieldType == toField.FieldType);
				if (fromField == null)
					continue;
				builder
					.Bind(toField)
					.From(fromField)
					.Using<CopyBinding>();
			}
		}
	}
}
