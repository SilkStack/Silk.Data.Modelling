namespace Silk.Data.Modelling
{
	public interface ITypeInstanceFactory
	{
		T CreateInstance<T>();
	}
}
