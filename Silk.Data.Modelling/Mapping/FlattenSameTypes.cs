using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class FlattenSameTypes : IMappingConvention
	{
		public static FlattenSameTypes Instance { get; } = new FlattenSameTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q)))
			{
				var potentialSourcePaths = ConventionUtilities.GetPaths(toField.FieldName).ToArray();

				ISourceField fromField = null;

				foreach (var sourcePath in potentialSourcePaths)
				{
					var testField = fromModel.GetField(sourcePath);
					if (testField == null || !testField.CanRead || testField.FieldType != toField.FieldType)
						continue;
					fromField = testField;
					break;
				}
				if (fromField == null)
					continue;

				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<CopyBinding>();
			}
		}
	}
}
