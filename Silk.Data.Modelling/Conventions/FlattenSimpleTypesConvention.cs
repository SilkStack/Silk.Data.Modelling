using Silk.Data.Modelling.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silk.Data.Modelling.Conventions
{
	public class FlattenSimpleTypesConvention : ViewConvention
	{
		public override ViewType SupportedViewTypes => ViewType.ModelDriven;

		public override void MakeModelFields(Model model, TypedModelField field, ViewDefinition viewDefinition)
		{
			if (!IsSimpleType(field.DataType) || viewDefinition.FieldDefinitions.Any(q => q.Name == field.Name))
				return;
			var checkPaths = ConventionHelpers.GetPaths(field.Name);
			foreach (var path in checkPaths)
			{
				var sourceField = ConventionHelpers.GetField(path, model);
				if (sourceField != null && sourceField.DataType == field.DataType)
				{
					var bindingDirection = BindingDirection.None;
					if (field.CanWrite && sourceField.CanRead)
						bindingDirection |= BindingDirection.ModelToView;
					if (bindingDirection == BindingDirection.None)
						continue;

					viewDefinition.FieldDefinitions.Add(
						new ViewFieldDefinition(field.Name,
						new AssignmentBinding(bindingDirection, path, new[] { field.Name }))
						{
							DataType = field.DataType
						});
					break;
				}
			}
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
				type == typeof(char)
				;
		}
	}
}
