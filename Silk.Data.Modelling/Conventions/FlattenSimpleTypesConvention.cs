using Silk.Data.Modelling.Bindings;
using System;

namespace Silk.Data.Modelling.Conventions
{
	public class FlattenSimpleTypesConvention : ViewConvention<ViewBuilder>
	{
		public override ViewType SupportedViewTypes => ViewType.ModelDriven;
		public override bool PerformMultiplePasses => false;
		public override bool SkipIfFieldDefined => true;

		public override void MakeModelField(ViewBuilder viewBuilder, ModelField field)
		{
			if (!IsSimpleType(field.DataType))
				return;

			var checkPaths = ConventionHelpers.GetPaths(field.Name);
			foreach (var path in checkPaths)
			{
				var sourceField = viewBuilder.FindField(field, path,
					dataType: field.DataType);
				if (sourceField == null ||
					sourceField.Field.DataType != field.DataType ||
					sourceField.BindingDirection == BindingDirection.None)
					continue;

				viewBuilder.DefineAssignedViewField(sourceField, path);
				return;
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
				type == typeof(char) ||
				type == typeof(bool)
				;
		}
	}
}
