namespace Silk.Data.Modelling.Mapping.Binding
{
	public interface IBinding<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		/// <summary>
		/// Perform the binding.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination);
	}
}
