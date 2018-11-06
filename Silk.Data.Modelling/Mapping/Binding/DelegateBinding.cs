using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class DelegateBinding<TFrom, TTo> : MappingBinding
	{
		private readonly Func<TFrom, TTo> _delegate;

		public DelegateBinding(IFieldReference from, IFieldReference to, Func<TFrom, TTo> @delegate) : base(from, to)
		{
			_delegate = @delegate;
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var fromValue = from.ReadField<TFrom>(From);
			var toValue = _delegate(fromValue);
			to.WriteField(To, toValue);
		}
	}

	public class DelegateBinding : IMappingBindingFactory<Delegate>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, Delegate bindingOption)
		{
			var typedFunc = bindingOption as Func<TFrom, TTo>;
			if (typedFunc == null)
				throw new MappingRequirementException($"Delegate must be of type System.Func<{typeof(TFrom).FullName}, {typeof(TTo).FullName}>.");
			return new DelegateBinding<TFrom, TTo>(fromField, toField, typedFunc);
		}
	}
}
