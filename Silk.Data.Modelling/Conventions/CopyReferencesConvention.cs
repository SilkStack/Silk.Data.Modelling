using Silk.Data.Modelling.Bindings;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Copies references to objects when types are compatible.
	/// </summary>
	public class CopyReferencesConvention : ViewConvention<ViewBuilder>
	{
		public override ViewType SupportedViewTypes => ViewType.All;
		public override bool PerformMultiplePasses => false;
		public override bool SkipIfFieldDefined => true;

		public override void MakeModelFields(Model model, ModelField field, ViewDefinition viewDefinition)
		{
			if (field.DataType.IsValueType || viewDefinition.FieldDefinitions.Any(q => q.Name == field.Name))
				return;
			var bindField = model.Fields.FirstOrDefault(q => q.Name == field.Name);
			if (bindField == null)
				return;

			var bindingDirection = BindingDirection.None;
			if (field.CanWrite && bindField.CanRead)
				bindingDirection |= BindingDirection.ModelToView;
			if (field.CanRead && bindField.CanWrite)
				bindingDirection |= BindingDirection.ViewToModel;
			if (bindingDirection == BindingDirection.None)
				return;

			if (bindField.DataType == field.DataType)
			{
				viewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(field.Name,
					new AssignmentBinding(bindingDirection, new[] { bindField.Name }, new[] { field.Name }))
				{
					DataType = field.DataType
				});
			}
		}
	}
}
