namespace Silk.Data.Modelling
{
	/// <summary>
	/// A reference to a field on a model.
	/// </summary>
	public interface IFieldReference
	{
		/// <summary>
		/// Gets the referenced field.
		/// </summary>
		IField Field { get; }
		/// <summary>
		/// Gets the model the reference is relative to.
		/// </summary>
		IModel Model { get; }
	}
}
