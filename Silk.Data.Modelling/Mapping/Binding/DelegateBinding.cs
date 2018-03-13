using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class DelegateBinding<TFrom, TTo> : MappingBinding
	{
		private readonly Func<TFrom, TTo> _delegate;

		public DelegateBinding(string[] fromPath, string[] toPath, Func<TFrom, TTo> @delegate) : base(fromPath, toPath)
		{
			_delegate = @delegate;
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var fromValue = from.ReadField<TFrom>(FromPath, 0);
			var toValue = _delegate(fromValue);
			to.WriteField(ToPath, 0, toValue);
		}
	}

	public class DelegateBinding : IMappingBindingFactory<Delegate>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, Delegate bindingOption)
		{
			var typedFunc = bindingOption as Func<TFrom, TTo>;
			if (typedFunc == null)
				throw new MappingRequirementException($"Delegate must be of type System.Func<{typeof(TFrom).FullName}, {typeof(TTo).FullName}>.");
			return new DelegateBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath, typedFunc);
		}
	}
}
