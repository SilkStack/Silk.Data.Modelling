namespace Silk.Data.Modelling
{
	public abstract class ModelReadWriter : IContainerReadWriter
	{
		public ContainerType ContainerType => ContainerType.Model;
		public Model Model { get; }

		protected ModelReadWriter(Model model)
		{
			Model = model;
		}

		public abstract T ReadFromPath<T>(string[] path);
		public abstract void WriteToPath<T>(string[] path, T value);
	}
}
