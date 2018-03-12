namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ValueBinding<T> : AssignmentBinding
	{
		private readonly T _value;

		public ValueBinding(string[] toPath, T value) : base(toPath)
		{
			_value = value;
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField(ToPath, 0, _value);
		}
	}

	public class ValueBindingFactory<T> : IAssignmentBindingFactory<T>
	{
		public AssignmentBinding CreateBinding<TTo>(ITargetField toField, T value)
		{
			return new ValueBinding<T>(toField.FieldPath, value);
		}
	}
}
