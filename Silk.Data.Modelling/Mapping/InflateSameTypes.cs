using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class InflateSameTypes : IMappingConvention
	{
		public static InflateSameTypes Instance { get; } = new InflateSameTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var fromField in fromModel.Fields.Where(q => q.CanRead))
			{
				var potentialTargetPaths = ConventionUtilities.GetPaths(fromField.FieldName).ToArray();

				ITargetField toField = null;

				foreach (var targetPath in potentialTargetPaths)
				{
					var testField = toModel.GetField(targetPath);
					if (testField == null || !testField.CanWrite || testField.FieldType != fromField.FieldType ||
						builder.IsBound(testField))
						continue;
					toField = testField;
					break;
				}
				if (toField == null)
					continue;

				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<CopyBinding>();
			}

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
