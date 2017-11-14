using Silk.Data.Modelling.Bindings;
using System;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Copies simple types.
	/// </summary>
	public class CopySimpleTypesConvention : ViewConvention
	{
		public override ViewType SupportedViewTypes => ViewType.All;

		public override void MakeModelFields(Model model, TypedModelField field, ViewDefinition viewDefinition)
		{
			if (!IsSimpleType(field.DataType) || viewDefinition.FieldDefinitions.Any(q => q.Name == field.Name))
				return;
			var bindField = model.Fields.FirstOrDefault(q => q.Name == field.Name && q.DataType == field.DataType);
			if (bindField == null)
				return;

			var bindingDirection = BindingDirection.None;
			if (field.CanWrite && bindField.CanRead)
				bindingDirection |= BindingDirection.ModelToView;
			if (field.CanRead && bindField.CanWrite)
				bindingDirection |= BindingDirection.ViewToModel;
			if (bindingDirection == BindingDirection.None)
				return;

			viewDefinition.FieldDefinitions.Add(
				new ViewFieldDefinition(field.Name,
					new AssignmentBinding(bindingDirection, new[] { bindField.Name }, new[] { field.Name }))
				{
					DataType = field.DataType
				});
		}

		private static bool IsSimpleType(Type type)
		{
			return
				type == typeof(sbyte) ||
				type == typeof(byte) ||
				type == typeof(short) ||
				type == typeof(ushort) ||
				type == typeof(int) ||
				type == typeof(uint) ||
				type == typeof(long) ||
				type == typeof(ulong) ||
				type == typeof(Single) ||
				type == typeof(Double) ||
				type == typeof(Decimal) ||
				type == typeof(string) ||
				type == typeof(Guid) ||
				type == typeof(char) ||
				type == typeof(bool)
				;
		}
	}
}
