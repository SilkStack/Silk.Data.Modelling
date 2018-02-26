namespace Silk.Data.Modelling.Mapping
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
		/// Gets the bindings for the mapping operation.
		/// </summary>
		public Binding[] Bindings { get; }

		public Mapping(IModel fromModel, IModel toModel, Binding[] bindings)
		{
			FromModel = fromModel;
			ToModel = toModel;
			Bindings = bindings;
		}

		public void PerformMapping(IModelReadWriter from, IModelReadWriter to)
		{
			foreach (var binding in Bindings)
			{
				binding.CopyBindingValue(from, to);
			}
		}
	}
}
