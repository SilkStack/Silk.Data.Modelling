using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping
{
	public abstract class ObjectGraphReaderWriterBase
	{
		public abstract void CommitEnumerable<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath,
			IEnumerable<T> enumerable);
	}

	public abstract class ObjectGraphReaderWriterBase<TGraph> :
		ObjectGraphReaderWriterBase,
		IGraphReader<TypeModel, PropertyInfoField>,
		IGraphWriter<TypeModel, PropertyInfoField>
	{
		private readonly ObjectGraphPropertyAccessor<TGraph> _propertyAccessor =
			ObjectGraphPropertyAccessor.GetFor<TGraph>();
		private readonly IFieldPath<TypeModel, PropertyInfoField> _fieldPath;

		public TGraph Graph { get; protected set; }

		public ObjectGraphReaderWriterBase(TGraph graph, IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			Graph = graph;
			_fieldPath = fieldPath;
		}

		public T Read<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			var reader = _propertyAccessor.GetPropertyReader<T>(fieldPath, _fieldPath?.Fields.Count ?? 0);
			return reader(Graph);
		}

		public void Write<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath, T value)
		{
			var writer = _propertyAccessor.GetPropertyWriter<T>(fieldPath, _fieldPath?.Fields.Count ?? 0);
			Graph = writer(Graph, value);
		}

		public bool CheckPath(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			var checker = _propertyAccessor.GetPropertyChecker(fieldPath, skipLastField: true,
				pathOffset: _fieldPath?.Fields.Count ?? 0);
			return checker(Graph);
		}

		public void CreateContainer(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			var containerCreator = _propertyAccessor.GetContainerCreator(fieldPath, _fieldPath?.Fields.Count ?? 0);
			containerCreator(Graph);
		}

		public bool CheckContainer(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			var checker = _propertyAccessor.GetPropertyChecker(fieldPath, skipLastField: false,
				pathOffset: _fieldPath?.Fields.Count ?? 0);
			return checker(Graph);
		}

		public IGraphReaderEnumerator<TypeModel, PropertyInfoField> GetEnumerator<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			var reader = _propertyAccessor.GetEnumerableReader<T>(fieldPath, _fieldPath?.Fields.Count ?? 0);
			var enumerable = reader(Graph);
			return new ObjectGraphReaderEnumerator<T>(enumerable.GetEnumerator(), fieldPath);
		}

		public IGraphWriterStream<TypeModel, PropertyInfoField> CreateEnumerableStream<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			return new OpenGraphEnumerableStream<T>(this, fieldPath);
		}

		public override void CommitEnumerable<T>(IFieldPath<TypeModel, PropertyInfoField> fieldPath,
			IEnumerable<T> enumerable)
		{
			var writer = _propertyAccessor.GetEnumerableWriter<T>(fieldPath, _fieldPath?.Fields.Count ?? 0);
			writer(Graph, enumerable);
		}
	}

	/// <summary>
	/// Combination graph reader/writer for object graphs.
	/// </summary>
	public class ObjectGraphReaderWriter<TGraph> :
		ObjectGraphReaderWriterBase<TGraph>
		where TGraph : class
	{
		public ObjectGraphReaderWriter(TGraph graph) : base(graph, null)
		{
		}
	}
}
