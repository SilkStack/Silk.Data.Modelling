using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Analysis.Rules;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ToStringBindingFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public void CreateBinding(MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext, IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				intersectedFields.LeftField.RemoveEnumerableType() != typeof(string) ||
				intersectedFields.LeftField.IsEnumerableType != intersectedFields.RightField.IsEnumerableType ||
				intersectedFields.IntersectionRuleType != typeof(ConvertableWithToStringRule<TFromModel, TFromField, TToModel, TToField>)
				)
			{
				return;
			}

			var builder = new BindingBuilder();
			intersectedFields.Dispatch(builder);

			mappingFactoryContext.Bindings.Add(builder.Binding);
		}

		public void PostBindings(MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext)
		{
		}

		private class BindingBuilder : IIntersectedFieldsGenericExecutor
		{
			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			void IIntersectedFieldsGenericExecutor.Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
				IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields
				)
			{
				Binding = new ToStringBinding<TFromModel, TFromField, TToModel, TToField, TLeftData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>
					);
			}
		}
	}

	public class ToStringBinding<TFromModel, TFromField, TToModel, TToField, TData> :
		BindingBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public ToStringBinding(IFieldPath<TFromModel, TFromField> fromPath, IFieldPath<TToModel, TToField> toPath) :
			base(fromPath.FinalField, fromPath, toPath.FinalField, toPath, fromPath.FinalField.IsEnumerableType ? fromPath : null)
		{
		}

		public override void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			if (!source.CheckPath(FromPath) || !destination.CheckPath(ToPath))
				return;

			destination.Write<string>(ToPath, source.Read<TData>(FromPath).ToString());
		}
	}
}
