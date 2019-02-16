using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Analysis.Rules;
using Silk.Data.Modelling.GenericDispatch;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class TryParseBindingFactory<TFromModel, TFromField, TToModel, TToField> :
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
				intersectedFields.IntersectionRuleType != typeof(ConvertableWithTryParse<TFromModel, TFromField, TToModel, TToField>)
				)
			{
				return;
			}

			var methodInfo = default(MethodInfo); // intersectedFields.IntersectionMetadata as MethodInfo;
			if (methodInfo == null)
				return;

			var builder = new BindingBuilder(methodInfo);
			intersectedFields.Dispatch(builder);
			mappingFactoryContext.Bindings.Add(builder.Binding);
		}

		public void PostBindings(MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext)
		{
		}

		private class BindingBuilder : IIntersectedFieldsGenericExecutor
		{
			private readonly MethodInfo _methodInfo;

			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			public BindingBuilder(MethodInfo methodInfo)
			{
				_methodInfo = methodInfo;
			}

			void IIntersectedFieldsGenericExecutor.Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
				IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields
				)
			{
				Binding = new TryParseBinding<TFromModel, TFromField, TToModel, TToField, TRightData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>,
					_methodInfo
					);
			}
		}
	}

	public class TryParseBinding<TFromModel, TFromField, TToModel, TToField, TData> :
		BindingBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private delegate bool TryParseMethod(string from, out TData to);

		private readonly TryParseMethod _tryParse;

		public TryParseBinding(IFieldPath<TFromModel, TFromField> fromPath, IFieldPath<TToModel, TToField> toPath,
			MethodInfo tryParseMethod) :
			base(fromPath.FinalField, fromPath, toPath.FinalField, toPath, fromPath.FinalField.IsEnumerableType ? fromPath : null)
		{
			_tryParse = CreateTryParseMethod(tryParseMethod);
		}

		private TryParseMethod CreateTryParseMethod(MethodInfo parseMethod)
		{
			var parameters = new List<ParameterExpression>();
			var fromParameter = Expression.Parameter(typeof(string), "from");
			parameters.Add(fromParameter);

			var toParameter = Expression.Parameter(typeof(TData).MakeByRefType(), "to");
			parameters.Add(toParameter);

			Expression body;
			if (parseMethod.DeclaringType == typeof(Enum))
				body = Expression.Call(parseMethod, fromParameter, Expression.Constant(false), toParameter);
			else
				body = Expression.Call(parseMethod, fromParameter, toParameter);

			return Expression.Lambda<TryParseMethod>(body, parameters).Compile();
		}

		public override void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			if (!source.CheckPath(FromPath) || !destination.CheckPath(ToPath))
				return;

			var strValue = source.Read<string>(FromPath);
			if (_tryParse(strValue, out var value))
				destination.Write<TData>(ToPath, value);
		}
	}
}
