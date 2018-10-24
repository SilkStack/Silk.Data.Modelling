using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ExplicitCastBinding<TFrom, TTo> : MappingBinding
	{
		private Func<TFrom, TTo> _cast;

		public ExplicitCastBinding(string[] fromPath, string[] toPath, MethodInfo castMethod) : base(fromPath, toPath)
		{
			_cast = CreateCastDelegate(castMethod);
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(ToPath, _cast(from.ReadField<TFrom>(FromPath)));
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
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, MethodInfo castMethod)
		{
			return new ExplicitCastBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath, castMethod);
		}
	}
}
