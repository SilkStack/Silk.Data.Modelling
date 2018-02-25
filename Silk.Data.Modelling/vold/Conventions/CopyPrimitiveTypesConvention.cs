using Silk.Data.Modelling.Bindings;
using System;
using System.Linq;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Copies primitive types.
	/// </summary>
	public class CopyPrimitiveTypesConvention : ViewConvention<ViewBuilder>
	{
		private static Type[] _defaultPrimitiveTypes = new[]
		{
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(Single),
			typeof(Double),
			typeof(Decimal),
			typeof(string),
			typeof(Guid),
			typeof(char),
			typeof(bool),
			typeof(DateTime),
			typeof(TimeSpan)
		};

		private Type[] _primitiveTypes;

		public override ViewType SupportedViewTypes => ViewType.All;
		public override bool PerformMultiplePasses => false;
		public override bool SkipIfFieldDefined => true;

		public CopyPrimitiveTypesConvention(Type[] primitiveTypes)
		{
			_primitiveTypes = primitiveTypes;
		}

		public CopyPrimitiveTypesConvention() : this(_defaultPrimitiveTypes)
		{
		}

		public override void MakeModelField(ViewBuilder viewBuilder, ModelField field)
		{
			if (!_primitiveTypes.Contains(field.DataType))
				return;

			var sourceField = viewBuilder.FindSourceField(field, field.Name,
				dataType: field.DataType);
			if (sourceField == null || sourceField.BindingDirection == BindingDirection.None)
				return;

			viewBuilder.DefineAssignedViewField(sourceField,
				metadata: sourceField.Field.Metadata);
		}
	}
}
