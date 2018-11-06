namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ValueBinding<T> : AssignmentBinding
	{
		private readonly T _value;

		public ValueBinding(IFieldReference to, T value) : base(to)
		{
			_value = value;
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField(To, _value);
		}
	}

	public class ValueBindingFactory<T> : IAssignmentBindingFactory<T>
	{
		public AssignmentBinding CreateBinding<TTo>(IFieldReference toField, T value)
		{
			return new ValueBinding<T>(toField, value);
		}
	}
}
