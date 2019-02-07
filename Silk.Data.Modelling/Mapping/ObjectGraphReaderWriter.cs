namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Combination graph reader/writer for object graphs.
	/// </summary>
	public class ObjectGraphReaderWriter<TGraph> :
		IGraphReader<TypeModel, PropertyInfoField>,
		IGraphWriter<TypeModel, PropertyInfoField>
		where TGraph : class
	{
		private readonly ObjectGraphPropertyAccessor<TGraph> _propertyAccessor =
			ObjectGraphPropertyAccessor.GetFor<TGraph>();

		public TGraph Graph { get; }

		public ObjectGraphReaderWriter(TGraph graph)
		{
			Graph = graph;
		}

		public T Read<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			var reader = _propertyAccessor.GetPropertyReader<T>(fieldPath);
			return reader(Graph);
		}

		public void Write<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath, T value)
		{
			var writer = _propertyAccessor.GetPropertyWriter<T>(fieldPath);
			writer(Graph, value);
		}
	}
}
