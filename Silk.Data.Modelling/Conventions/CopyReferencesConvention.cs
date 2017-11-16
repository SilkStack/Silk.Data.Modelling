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

		public override void MakeModelField(ViewBuilder viewBuilder, ModelField field)
		{
			if (field.DataType.IsValueType) return;

			var sourceField = viewBuilder.FindField(field, field.Name,
				dataType: field.DataType);
			if (sourceField == null || sourceField.BindingDirection == BindingDirection.None)
				return;

			viewBuilder.DefineAssignedViewField(sourceField);
		}
	}
}
