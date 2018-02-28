namespace Silk.Data.Modelling.Mapping
{
	public interface IBindingFactory
	{
		Binding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField);
	}

	public interface IBindingFactory<T>
	{
		Binding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, T bindingOption);
	}
}
