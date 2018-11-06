using System;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CastExpressionBinding<TFrom, TTo> : MappingBinding
	{
		private readonly Func<TFrom, TTo> _cast;

		public CastExpressionBinding(IFieldReference from, IFieldReference to) : base(from, to)
		{
			_cast = CreateCastExpression();
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(To, _cast(from.ReadField<TFrom>(From)));
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
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField)
		{
			return new CastExpressionBinding<TFrom, TTo>(fromField, toField);
		}
	}
}
