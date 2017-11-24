using Silk.Data.Modelling.Bindings;
using System;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Copies simple types.
	/// </summary>
	public class CopySimpleTypesConvention : ViewConvention<ViewBuilder>
	{
		public override ViewType SupportedViewTypes => ViewType.All;
		public override bool PerformMultiplePasses => false;
		public override bool SkipIfFieldDefined => true;

		public override void MakeModelField(ViewBuilder viewBuilder, ModelField field)
		{
			if (!IsSimpleType(field.DataType))
				return;

			var sourceField = viewBuilder.FindSourceField(field, field.Name,
				dataType: field.DataType);
			if (sourceField == null || sourceField.BindingDirection == BindingDirection.None)
				return;

			viewBuilder.DefineAssignedViewField(sourceField,
				metadata: sourceField.Field.Metadata);
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
				type == typeof(bool) ||
				type == typeof(DateTime) ||
				type == typeof(TimeSpan)
				;
		}
	}
}
