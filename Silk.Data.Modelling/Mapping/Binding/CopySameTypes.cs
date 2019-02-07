﻿using Silk.Data.Modelling.Analysis;
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
		public void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				intersectedFields.LeftField.FieldDataType != intersectedFields.RightField.FieldDataType ||
				mappingFactoryContext.IsToFieldBound(intersectedFields))
			{
				return;
			}

			var builder = new BindingBuilder();
			intersectedFields.Dispatch(builder);

			mappingFactoryContext.Bindings.Add(builder.Binding);
		}

		private class BindingBuilder : IIntersectedFieldsGenericExecutor
		{
			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			void IIntersectedFieldsGenericExecutor.Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
				IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields
				)
			{
				Binding = new CopySameTypesBinding<TFromModel, TFromField, TToModel, TToField, TLeftData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>
					);
			}
		}
	}

	public class CopySameTypesBinding<TFromModel, TFromField, TToModel, TToField, TData> :
		IBinding<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private readonly IFieldPath<TFromModel, TFromField> _fromPath;
		private readonly IFieldPath<TToModel, TToField> _toPath;

		public TToField ToField => _toPath.FinalField;

		public TFromField FromField => _fromPath.FinalField;

		public CopySameTypesBinding(IFieldPath<TFromModel, TFromField> fromPath, IFieldPath<TToModel, TToField> toPath)
		{
			_fromPath = fromPath;
			_toPath = toPath;
		}

		public void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
			=> destination.Write<TData>(_toPath, source.Read<TData>(_fromPath));
	}
}