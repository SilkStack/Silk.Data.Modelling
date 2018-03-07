using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class InflateSameTypes : IMappingConvention
	{
		public InflateSameTypes(bool createInstancesAsNeeded = false)
		{
			CreateInstancesAsNeeded = createInstancesAsNeeded;
		}

		public bool CreateInstancesAsNeeded { get; }

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

				if (CreateInstancesAsNeeded)
				{
					var path = new List<string>();
					foreach (var pathSegment in toField.FieldPath.Take(toField.FieldPath.Length - 1))
					{
						path.Add(pathSegment);
						var pathField = toModel.GetField(path.ToArray());
						if (builder.IsBound(pathField))
							continue;

						var ctor = pathField.FieldType.GetTypeInfo().DeclaredConstructors
							.FirstOrDefault(q => q.GetParameters().Length == 0);
						if (ctor == null)
						{
							throw new MappingRequirementException($"A constructor with 0 parameters is required on type {pathField.FieldType}.");
						}

						builder
							.Bind(pathField)
							.AssignUsing<CreateInstanceIfNull, ConstructorInfo>(ctor);
					}
				}

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
