using Silk.Data.Modelling.Bindings;

namespace Silk.Data.Modelling.Conventions
{
	public class EnumerableConversionsConvention : ViewConvention
	{
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
				if (sourceField != null && sourceField.IsEnumerable &&
					targetField != null && targetField.IsEnumerable)
				{
					field.ModelBinding = new EnumerableBinding(field.ModelBinding,
						sourceField.EnumerableType, targetField.EnumerableType);
				}
			}
		}
	}
}
