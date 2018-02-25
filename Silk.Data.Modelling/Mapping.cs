namespace Silk.Data.Modelling
{
	/// <summary>
	/// Mapping between two types.
	/// </summary>
	public class Mapping
	{
		/// <summary>
		/// Gets the model the mapping maps from.
		/// </summary>
		public IModel FromModel { get; }
		/// <summary>
		/// Gets the model the mapping maps to.
		/// </summary>
		public IModel ToModel { get; }
		/// <summary>
		/// Gets the value bindings for the mapping operation.
		/// </summary>
		public Binding[] ValueBindings { get; }

		public void PerformMapping(IModelReadWriter from, IModelReadWriter to)
		{
			foreach (var binding in ValueBindings)
			{
				binding.CopyBindingValue(from, to);
			}
		}
	}
}
