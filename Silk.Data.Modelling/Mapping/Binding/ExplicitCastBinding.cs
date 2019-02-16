using System;
using System.Linq.Expressions;
using System.Reflection;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Analysis.Rules;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ExplicitCastBindingFactory<TFromModel, TFromField, TToModel, TToField> :
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
				intersectedFields.LeftField.IsEnumerableType != intersectedFields.RightField.IsEnumerableType ||
				intersectedFields.IntersectionRuleType != typeof(ExplicitCastRule<TFromModel, TFromField, TToModel, TToField>)
				)
			{
				return;
			}

			var methodInfo = default(MethodInfo);  //intersectedFields.IntersectionMetadata as MethodInfo;
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
				Binding = new ExplicitCastBinding<TFromModel, TFromField, TToModel, TToField, TLeftData, TRightData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>,
					_methodInfo
					);
			}
		}
	}

	public class ExplicitCastBinding<TFromModel, TFromField, TToModel, TToField, TFromData, TToData> :
		BindingBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private readonly Func<TFromData, TToData> _cast;

		public ExplicitCastBinding(IFieldPath<TFromModel, TFromField> fromPath, IFieldPath<TToModel, TToField> toPath,
			MethodInfo castMethod) :
			base(fromPath.FinalField, fromPath, toPath.FinalField, toPath, fromPath.FinalField.IsEnumerableType ? fromPath : null)
		{
			_cast = CreateCastDelegate(castMethod);
		}

		private static Func<TFromData, TToData> CreateCastDelegate(MethodInfo castMethod)
		{
			var parameter = Expression.Parameter(typeof(TFromData));
			var lambda = Expression.Lambda<Func<TFromData, TToData>>(
				Expression.Call(castMethod, parameter),
				parameter
				);
			return lambda.Compile();
		}

		public override void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			destination.Write<TToData>(ToPath, _cast(source.Read<TFromData>(FromPath)));
		}
	}
}
