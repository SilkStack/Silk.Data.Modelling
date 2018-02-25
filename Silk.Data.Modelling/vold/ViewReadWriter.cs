namespace Silk.Data.Modelling
{
	public abstract class ViewReadWriter : IContainerReadWriter
	{
		public ContainerType ContainerType => ContainerType.View;
		public IView View { get; }

		protected ViewReadWriter(IView view)
		{
			View = view;
		}

		public abstract T ReadFromPath<T>(string[] path);
		public abstract void WriteToPath<T>(string[] path, T value);
	}
}
