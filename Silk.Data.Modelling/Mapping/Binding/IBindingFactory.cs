namespace Silk.Data.Modelling.Mapping.Binding
{
	public interface IAssignmentBindingFactory
	{
		AssignmentBinding CreateBinding<TTo>(IFieldReference toField);
	}

	public interface IAssignmentBindingFactory<T>
	{
		AssignmentBinding CreateBinding<TTo>(IFieldReference toField, T bindingOption);
	}

	public interface IMappingBindingFactory
	{
		MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField);
	}

	public interface IMappingBindingFactory<T>
	{
		MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, T bindingOption);
	}
}
