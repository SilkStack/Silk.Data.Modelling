using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.ResourceLoaders;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Maps reference types that have matching names, or flattened names.
	/// </summary>
	public class MapReferenceTypesConvention : ViewConvention
	{
		public override void MakeModelFields(Model model, TypedModelField field, ViewDefinition viewDefinition)
		{
			if (field.DataType.IsValueType || viewDefinition.FieldDefinitions.Any(q => q.Name == field.Name) ||
				!field.DataType.GetConstructors().Any(q => q.GetParameters().Length == 0))
				return;
			var checkPaths = ConventionHelpers.GetPaths(field.Name);
			foreach (var path in checkPaths)
			{
				var bindField = ConventionHelpers.GetField(path, model);
				if (bindField != null && !bindField.DataType.IsValueType)
				{
					var subMapper = GetSubMapper(viewDefinition);
					if (!subMapper.HasMapping(bindField.DataType, field.DataType))
					{
						//  todo: use the same view conventions as the the caller to build sub-views
						subMapper.AddMapping(bindField.DataTypeModel, field.DataType);
					}
					subMapper.AddMappedField(bindField.Name, field.Name, bindField.DataType, field.DataType);
					viewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(field.Name,
						new SubMappingBinding(new[] { bindField.Name }, new[] { field.Name }))
					{
						DataType = field.DataType
					});
					break;
				}
			}
		}

		private SubMappingResourceLoader GetSubMapper(ViewDefinition viewDefinition)
		{
			var subMapper = viewDefinition.ResourceLoaders
				.OfType<SubMappingResourceLoader>()
				.FirstOrDefault();
			if (subMapper == null)
			{
				subMapper = new SubMappingResourceLoader(viewDefinition.Model);
				viewDefinition.ResourceLoaders.Add(subMapper);
			}
			return subMapper;
		}
	}
}
