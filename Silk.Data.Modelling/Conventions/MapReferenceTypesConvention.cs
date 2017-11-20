using Silk.Data.Modelling.Bindings;
using System.Reflection;

namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Maps reference types that have matching names, or flattened names.
	/// </summary>
	public class MapReferenceTypesConvention : ViewConvention<ViewBuilder>
	{
		public override ViewType SupportedViewTypes => ViewType.All;
		public override bool PerformMultiplePasses => false;
		public override bool SkipIfFieldDefined => true;

		public override void MakeModelField(ViewBuilder viewBuilder, ModelField field)
		{
			if (field.DataType.GetTypeInfo().IsValueType)
				return;

			var checkPaths = ConventionHelpers.GetPaths(field.Name);
			foreach (var path in checkPaths)
			{
				var bindField = viewBuilder.FindField(field, path);
				if (bindField == null || bindField.BindingDirection == BindingDirection.None ||
					bindField.Field.DataType.GetTypeInfo().IsValueType)
					continue;

				viewBuilder.DefineMappedViewField(field, bindField, path);
				break;
			}
		}
	}
}
