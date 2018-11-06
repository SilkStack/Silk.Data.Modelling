using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CopyBinding : IMappingBindingFactory
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField)
		{
			if (typeof(TFrom) != typeof(TTo))
				throw new InvalidOperationException("TFrom and TTo type mismatch.");
			return new CopyBinding<TFrom>(fromField, toField);
		}
	}

	public class CopyBinding<T> : MappingBinding
	{
		public CopyBinding(IFieldReference from, IFieldReference to)
			: base(from, to)
		{
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<T>(To, from.ReadField<T>(From));
		}
	}
}
