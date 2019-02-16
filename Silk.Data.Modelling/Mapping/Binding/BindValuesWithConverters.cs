using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class BindValuesWithConvertersFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields
			)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				intersectedFields.LeftField.IsEnumerableType != intersectedFields.RightField.IsEnumerableType
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
				Binding = new BindValuesWithConvertersBinding<TFromModel, TFromField, TToModel, TToField, TLeftData, TRightData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>,
					intersectedFields.GetConvertDelegate()
					);
			}
		}
	}

	public class BindValuesWithConvertersBinding<TFromModel, TFromField, TToModel, TToField, TFromData, TToData> :
		BindingBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private readonly TryConvertDelegate<TFromData, TToData> _tryConvertDelegate;

		public BindValuesWithConvertersBinding(
			IFieldPath<TFromModel, TFromField> fromPath,
			IFieldPath<TToModel, TToField> toPath,
			TryConvertDelegate<TFromData, TToData> tryConvertDelegate
			) :
			base(fromPath.FinalField, fromPath, toPath.FinalField, toPath, fromPath.FinalField.IsEnumerableType ? fromPath : null)
		{
			_tryConvertDelegate = tryConvertDelegate;
		}

		public override void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			if (!source.CheckPath(FromPath) || !destination.CheckPath(ToPath))
				return;

			var fromData = source.Read<TFromData>(FromPath);
			if (_tryConvertDelegate(fromData, out var toData))
				destination.Write<TToData>(ToPath, toData);
		}
	}
}
