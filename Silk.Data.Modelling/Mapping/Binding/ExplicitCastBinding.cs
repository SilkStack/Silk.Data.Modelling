using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ExplicitCastBinding<TFrom, TTo> : MappingBinding
	{
		private Func<TFrom, TTo> _cast;

		public ExplicitCastBinding(IFieldReference from, IFieldReference to, MethodInfo castMethod) : base(from, to)
		{
			_cast = CreateCastDelegate(castMethod);
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(To, _cast(from.ReadField<TFrom>(From)));
		}

		private static Func<TFrom, TTo> CreateCastDelegate(MethodInfo castMethod)
		{
			var parameter = Expression.Parameter(typeof(TFrom));
			var lambda = Expression.Lambda<Func<TFrom, TTo>>(
				Expression.Call(castMethod, parameter),
				parameter
				);
			return lambda.Compile();
		}
	}

	public class ExplicitCastBinding : IMappingBindingFactory<MethodInfo>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, MethodInfo castMethod)
		{
			return new ExplicitCastBinding<TFrom, TTo>(fromField, toField, castMethod);
		}
	}
}
