namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ToStringBinding<TFrom, TTo> : MappingBinding
	{
		public ToStringBinding(IFieldReference from, IFieldReference to) : base(from, to)
		{
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<string>(To, from.ReadField<TFrom>(From).ToString());
		}
	}

	public class ToStringBinding : IMappingBindingFactory
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField)
		{
			if (typeof(TTo) != typeof(string))
				throw new MappingRequirementException("String type expected.");
			return new ToStringBinding<TFrom, TTo>(fromField, toField);
		}
	}
}