using System;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CastExpressionBinding<TFrom, TTo> : MappingBinding
	{
		private readonly Func<TFrom, TTo> _cast;

		public CastExpressionBinding(string[] fromPath, string[] toPath) : base(fromPath, toPath)
		{
			_cast = CreateCastExpression();
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(ToPath, 0, _cast(from.ReadField<TFrom>(FromPath, 0)));
		}

		private Func<TFrom, TTo> CreateCastExpression()
		{
			var fromParameter = Expression.Parameter(typeof(TFrom), "from");
			var castExpression = Expression.Convert(fromParameter, typeof(TTo));
			return Expression.Lambda<Func<TFrom, TTo>>(castExpression, fromParameter).Compile();
		}
	}

	public class CastExpressionBinding : IMappingBindingFactory
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField)
		{
			return new CastExpressionBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath);
		}
	}
}
