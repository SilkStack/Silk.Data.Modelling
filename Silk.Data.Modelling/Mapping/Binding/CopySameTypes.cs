using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CopySameTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public bool GetBinding(
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields,
			out IBinding<TFromModel, TFromField, TToModel, TToField> binding)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				intersectedFields.LeftField.FieldDataType != intersectedFields.RightField.FieldDataType)
			{
				binding = null;
				return false;
			}

			var builder = new BindingBuilder();
			intersectedFields.Dispatch(builder);

			binding = builder.Binding;
			return true;
		}

		private class BindingBuilder : IIntersectedFieldsGenericExecutor
		{
			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			void IIntersectedFieldsGenericExecutor.Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields)
			{
				Binding = new CopySameTypesBinding<TFromModel, TFromField, TToModel, TToField, TLeftData, TRightData>();
			}
		}
	}

	public class CopySameTypesBinding<TFromModel, TFromField, TToModel, TToField, TFromData, TToData> :
		IBinding<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{

	}
}
