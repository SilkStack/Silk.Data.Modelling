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
				var fromField = fromModel.Fields.FirstOrDefault(field => field.CanRead &&
					field.FieldName == toField.FieldName &&
					AreTypesCompatible(field, toField));
				if (fromField == null)
					continue;
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
