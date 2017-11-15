using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.ResourceLoaders;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Maps reference types that have matching names, or flattened names.
	/// </summary>
	public class MapReferenceTypesConvention : ViewConvention<ViewBuilder>
	{
		public override ViewType SupportedViewTypes => ViewType.All;
		public override bool PerformMultiplePasses => false;
		public override bool SkipIfFieldDefined => true;

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
					var bindingDirection = BindingDirection.None;
					if (field.CanWrite && bindField.CanRead)
						bindingDirection |= BindingDirection.ModelToView;
					if (field.CanRead && bindField.CanWrite)
						bindingDirection |= BindingDirection.ViewToModel;
					if (bindingDirection == BindingDirection.None)
						continue;

					var subMapper = GetSubMapper(viewDefinition);
					var binding = new SubMappingBinding(
						bindingDirection,
						new[] { bindField.Name },
						new[] { field.Name },
						new[] { subMapper }
						);
					subMapper.AddField(field.Name, binding, bindField.DataType, field.DataType);
					viewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(field.Name, binding)
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
				subMapper = new SubMappingResourceLoader(
					viewDefinition.SourceModel,
					viewDefinition.ViewConventions
					);
				viewDefinition.ResourceLoaders.Add(subMapper);
			}
			return subMapper;
		}
	}
}
