using System;
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
			to.WriteField<TTo>(ToPath, 0, _cast(from.ReadField<TFrom>(FromPath, 0)));
		}

		private static Func<TFrom, TTo> CreateCastDelegate(MethodInfo castMethod)
		{
			var method = new DynamicMethod("Cast", typeof(TTo), new[] { typeof(TFrom) }, true);
			var ilgen = method.GetILGenerator();
			ilgen.Emit(OpCodes.Ldarg_0);
			ilgen.Emit(OpCodes.Call, castMethod);
			ilgen.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(Func<TFrom,TTo>)) as Func<TFrom,TTo>;
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
