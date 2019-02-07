using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CopySameTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : IField
		where TToField : IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public bool GetBinding(IntersectedFields intersectedFields, out IBinding binding)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				intersectedFields.LeftField.FieldDataType != intersectedFields.RightField.FieldDataType)
			{
				binding = null;
				return false;
			}

			var builder = new BindingBuilder();
			intersectedFields.ExecuteGenericEntryPoint(builder);

			binding = builder.Binding;
			return true;
		}

		private class BindingBuilder : ILeftRightIntersectionGenericExecutor
		{
			public IBinding Binding { get; private set; }

			void ILeftRightIntersectionGenericExecutor.Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields)
			{
				Binding = new CopySameTypesBinding<TLeftData, TRightData>();
			}
		}
	}

	public class CopySameTypesBinding<TFrom, TTo> : IBinding
	{

	}
}
