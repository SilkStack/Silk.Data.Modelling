using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.ResourceLoaders;
using System;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	public static class ViewBuilderExtensions
	{
		public static void DefineAssignedViewField(this ViewBuilder viewBuilder, ViewBuilder.FieldInfo fieldInfo,
			string[] modelBindingPath = null)
		{
			viewBuilder.DefineAssignedViewField(fieldInfo.Field.Name, fieldInfo.Field.DataType,
				fieldInfo.BindingDirection, modelBindingPath);
		}

		public static void DefineAssignedViewField(this ViewBuilder viewBuilder, string viewFieldName, Type fieldDataType,
			BindingDirection bindingDirection, string[] modelBindingPath = null)
		{
			if (modelBindingPath == null)
				modelBindingPath = new[] { viewFieldName };

			viewBuilder.ViewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(viewFieldName,
					new AssignmentBinding(bindingDirection, modelBindingPath, new[] { viewFieldName }))
			{
				DataType = fieldDataType
			});
		}

		public static void DefineMappedViewField(this ViewBuilder viewBuilder,
			ModelField modelField, ViewBuilder.FieldInfo fieldInfo,
			string[] modelBindingPath)
		{
			var subMapper = GetSubMapper(viewBuilder.ViewDefinition);
			var binding = new SubMappingBinding(
				fieldInfo.BindingDirection,
				modelBindingPath,
				new[] { fieldInfo.Field.Name },
				new[] { subMapper }
				);

			subMapper.AddField(fieldInfo.Field.Name, binding, fieldInfo.Field.DataType, modelField.DataType);
			viewBuilder.ViewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(fieldInfo.Field.Name, binding)
			{
				DataType = modelField.DataType
			});
		}

		private static SubMappingResourceLoader GetSubMapper(ViewDefinition viewDefinition)
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
