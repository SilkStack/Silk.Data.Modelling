namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Converts from one type to another.
	/// </summary>
	public abstract class Converter<TFrom, TTo>
	{
		/// <summary>
		/// Performs the type conversion.
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public abstract TTo Convert(TFrom from);
	}
}
