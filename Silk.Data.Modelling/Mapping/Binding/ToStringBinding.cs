namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ToStringBinding<TFrom, TTo> : MappingBinding
	{
		public ToStringBinding(string[] fromPath, string[] toPath) : base(fromPath, toPath)
		{
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<string>(ToPath, 0, from.ReadField<TFrom>(FromPath, 0).ToString());
		}
	}

	public class ToStringBinding : IMappingBindingFactory
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField)
		{
			if (typeof(TTo) != typeof(string))
				throw new MappingRequirementException("String type expected.");
			return new ToStringBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath);
		}
	}
}