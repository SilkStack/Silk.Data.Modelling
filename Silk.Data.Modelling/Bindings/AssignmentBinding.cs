namespace Silk.Data.Modelling.Bindings
{
	/// <summary>
	/// Copies values/references from models to views and vice-versa.
	/// </summary>
	public class AssignmentBinding : ModelBinding
	{
		public override BindingDirection Direction { get; }

		public AssignmentBinding(BindingDirection bindingDirection, string[] modelFieldPath, string[] viewFieldPath)
			: base(modelFieldPath, viewFieldPath)
		{
			Direction = bindingDirection;
		}
	}
}
