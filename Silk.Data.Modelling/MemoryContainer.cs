namespace Silk.Data.Modelling
{
	/// <summary>
	/// Stores a graph in memory.
	/// </summary>
	/// <remarks>Really designed as an example container.</remarks>
	public class MemoryContainer : IContainer
	{
		public TypedModel Model { get; }

		public IView View { get; }

		public MemoryContainer(TypedModel model, IView view)
		{
			Model = model;
			View = view;
		}
	}
}
