using Silk.Data.Modelling.Bindings;
using System;

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
	}
}
