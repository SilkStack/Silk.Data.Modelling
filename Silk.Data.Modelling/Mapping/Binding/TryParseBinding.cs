using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class TryParseBinding<TFrom, TTo> : MappingBinding
	{
		private delegate bool TryParseMethod(TFrom from, out TTo to);

		private readonly TryParseMethod _tryParse;

		public TryParseBinding(string[] fromPath, string[] toPath, MethodInfo parseMethod) : base(fromPath, toPath)
		{
			_tryParse = CreateTryParseMethod(parseMethod);
		}

		private TryParseMethod CreateTryParseMethod(MethodInfo parseMethod)
		{
			var parameters = new List<ParameterExpression>();
			var fromParameter = Expression.Parameter(typeof(TFrom), "from");
			parameters.Add(fromParameter);

			var toParameter = Expression.Parameter(typeof(TTo).MakeByRefType(), "to");
			parameters.Add(toParameter);

			Expression body;
			if (parseMethod.DeclaringType == typeof(Enum))
				body = Expression.Call(parseMethod, fromParameter, Expression.Constant(false), toParameter);
			else
				body = Expression.Call(parseMethod, fromParameter, toParameter);

			return Expression.Lambda<TryParseMethod>(body, parameters).Compile();
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var fromItem = from.ReadField<TFrom>(FromPath, 0);
			if (_tryParse(fromItem, out var toItem))
				to.WriteField(ToPath, 0, toItem);
		}
	}

	public class TryParseBinding : IMappingBindingFactory<MethodInfo>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, MethodInfo bindingOption)
		{
			return new TryParseBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath, bindingOption);
		}
	}
}