using Silk.Data.Modelling.Bindings;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	public class EnumerableConversionsConvention : ViewConvention
	{
		public override ViewType SupportedViewTypes => ViewType.All;

		public override void FinalizeModel(ViewDefinition viewDefinition)
		{
			foreach (var field in viewDefinition.FieldDefinitions)
			{
				var sourceField = viewDefinition.SourceModel.GetField(
					field.ModelBinding.ModelFieldPath
					);
				var targetField = viewDefinition.TargetModel.GetField(
					field.ModelBinding.ViewFieldPath
					);
				if (sourceField != null && (sourceField.IsEnumerable || IsEnumerablePath(viewDefinition.SourceModel, field.ModelBinding.ModelFieldPath)) &&
					targetField != null && (targetField.IsEnumerable || IsEnumerablePath(viewDefinition.TargetModel, field.ModelBinding.ViewFieldPath)))
				{
					var sourceEnumerableType = sourceField.EnumerableType;
					if (sourceEnumerableType == null)
						sourceEnumerableType = typeof(IEnumerable<>).MakeGenericType(sourceField.DataType);
					var targetEnumerableType = targetField.EnumerableType;
					if (targetEnumerableType == null)
						targetEnumerableType = typeof(IEnumerable<>).MakeGenericType(targetField.DataType);
					field.ModelBinding = new EnumerableBinding(field.ModelBinding,
						sourceEnumerableType, targetEnumerableType);
				}
			}
		}

		private bool IsEnumerablePath(Model model, string[] path)
		{
			var currentPath = new string[0];
			foreach (var pathComponent in path)
			{
				currentPath = currentPath.Concat(new[] { pathComponent }).ToArray();
				var field = model.GetField(currentPath);
				if (field.IsEnumerable)
					return true;
			}
			return false;
		}
	}
}
