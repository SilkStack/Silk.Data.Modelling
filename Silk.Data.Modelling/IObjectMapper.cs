namespace Silk.Data.Modelling
{
	/// <summary>
	/// Performs mappings on objects.
	/// </summary>
	public interface IObjectMapper
	{
		/// <summary>
		/// Map from <see cref="from"/> to a new instance of <see cref="TTo"/>.
		/// </summary>
		/// <typeparam name="TTo"></typeparam>
		/// <param name="from"></param>
		/// <returns></returns>
		TTo Map<TTo>(object from);

		/// <summary>
		/// Map from <see cref="from"/> to a new instance of <see cref="TTo"/>.
		/// </summary>
		TTo Map<TTo, TFrom>(TFrom from);

		/// <summary>
		/// Inject values from <see cref="from"/> into <see cref="to"/>.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		void Inject(object from, object to);

		/// <summary>
		/// Inject values from <see cref="from"/> into <see cref="to"/>.
		/// </summary>
		void Inject<TFrom, TTo>(TFrom from, TTo to);
	}
}
