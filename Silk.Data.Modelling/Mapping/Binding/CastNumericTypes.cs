using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CastNumericTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private static Type[] _numericTypes = new[]
		{
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(decimal),
			typeof(float),
			typeof(double)
		};

		public void CreateBinding(MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext, IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				!_numericTypes.Contains(intersectedFields.LeftField.RemoveEnumerableType()) ||
				!_numericTypes.Contains(intersectedFields.RightField.RemoveEnumerableType()) ||
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
				Binding = new CastExpressionBinding<TFromModel, TFromField, TToModel, TToField, TLeftData, TRightData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>
					);
			}
		}
	}

	public class CastExpressionBinding<TFromModel, TFromField, TToModel, TToField, TFromData, TToData> :
		BindingBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private readonly Func<TFromData, TToData> _func;

		public CastExpressionBinding(IFieldPath<TFromModel, TFromField> fromPath, IFieldPath<TToModel, TToField> toPath) :
			base(fromPath.FinalField, fromPath, toPath.FinalField, toPath, fromPath.FinalField.IsEnumerableType ? fromPath : null)
		{
			_func = CreateCastExpression();
		}

		private Func<TFromData, TToData> CreateCastExpression()
		{
			var fromParameter = Expression.Parameter(typeof(TFromData), "from");
			var castExpression = Expression.Convert(fromParameter, typeof(TToData));
			return Expression.Lambda<Func<TFromData, TToData>>(castExpression, fromParameter).Compile();
		}

		public override void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			if (!source.CheckPath(FromPath) || !destination.CheckPath(ToPath))
				return;

			destination.Write<TToData>(ToPath, _func(source.Read<TFromData>(FromPath)));
		}
	}
}
