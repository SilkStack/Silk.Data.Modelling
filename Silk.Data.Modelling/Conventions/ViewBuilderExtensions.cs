using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.ResourceLoaders;
using System;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	public static class ViewBuilderExtensions
	{
		public static void DefineAssignedViewField(this ViewBuilder viewBuilder, ViewBuilder.FieldInfo fieldInfo,
			string[] modelBindingPath = null, string viewFieldName = null,
			params object[] metadata)
		{
			if (viewFieldName == null)
				viewFieldName = fieldInfo.Field.Name;
			viewBuilder.DefineAssignedViewField(viewFieldName, fieldInfo.Field.DataType,
				fieldInfo.BindingDirection, modelBindingPath, metadata);
		}

		public static void DefineAssignedViewField(this ViewBuilder viewBuilder, string viewFieldName, Type fieldDataType,
			BindingDirection bindingDirection, string[] modelBindingPath = null,
			params object[] metadata)
		{
			if (modelBindingPath == null)
				modelBindingPath = new[] { viewFieldName };

			viewBuilder.DefineField(viewFieldName,
				new AssignmentBinding(bindingDirection, modelBindingPath, new[] { viewFieldName }),
				fieldDataType, metadata);
		}

		public static void DefineMappedViewField(this ViewBuilder viewBuilder,
			ModelField modelField, ViewBuilder.FieldInfo fieldInfo,
			string[] modelBindingPath, params object[] metadata)
		{
			var subMapper = GetSubMapper(viewBuilder.ViewDefinition);
			var binding = new SubMappingBinding(
				fieldInfo.BindingDirection,
				modelBindingPath,
				new[] { fieldInfo.Field.Name },
				new[] { subMapper }
				);

			subMapper.AddField(fieldInfo.Field.Name, binding, fieldInfo.Field.DataType, modelField.DataType);

			viewBuilder.DefineField(fieldInfo.Field.Name, binding, modelField.DataType, metadata);
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
