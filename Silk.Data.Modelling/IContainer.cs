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
	}
}
