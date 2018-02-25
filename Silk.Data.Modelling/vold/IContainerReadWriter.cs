namespace Silk.Data.Modelling
{
	public interface IContainerReadWriter
	{
		ContainerType ContainerType { get; }

		void WriteToPath<T>(string[] path, T value);
		T ReadFromPath<T>(string[] path);
	}
}
