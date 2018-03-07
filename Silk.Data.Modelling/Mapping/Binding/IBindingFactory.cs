namespace Silk.Data.Modelling.Mapping.Binding
{
	public interface IAssignmentBindingFactory
	{
		AssignmentBinding CreateBinding<TTo>(ITargetField toField);
	}

	public interface IAssignmentBindingFactory<T>
	{
		AssignmentBinding CreateBinding<TTo>(ITargetField toField, T bindingOption);
	}

	public interface IMappingBindingFactory
	{
		MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField);
	}

	public interface IMappingBindingFactory<T>
	{
		MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, T bindingOption);
	}
}
