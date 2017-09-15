namespace Silk.Data.Modelling
{
	/// <summary>
	/// Contains view data.
	/// </summary>
	public interface IContainer
	{
		/// <summary>
		/// Gets the model that reresents the containers business object type.
		/// </summary>
		TypedModel Model { get; }

		/// <summary>
		/// Gets the view the container can read/write.
		/// </summary>
		IView View { get; }

		/// <summary>
		/// Sets the value of a field.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="value"></param>
		void SetValue(IViewField field, object value);
	}
}
