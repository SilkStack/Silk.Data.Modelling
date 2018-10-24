using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CopyBinding : IMappingBindingFactory
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField)
		{
			if (typeof(TFrom) != typeof(TTo))
				throw new InvalidOperationException("TFrom and TTo type mismatch.");
			return new CopyBinding<TFrom>(fromField.FieldPath, toField.FieldPath);
		}
	}

	public class CopyBinding<T> : MappingBinding
	{
		public CopyBinding(string[] fromPath, string[] toPath)
			: base(fromPath, toPath)
		{
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<T>(ToPath, from.ReadField<T>(FromPath));
		}
	}
}
