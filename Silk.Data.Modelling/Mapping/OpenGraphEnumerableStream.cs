using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class OpenGraphEnumerableStream<TData> : IGraphWriterStream<TypeModel, PropertyInfoField>
	{
		private readonly List<StreamReaderWriter> _writers
			= new List<StreamReaderWriter>();
		private readonly ObjectGraphReaderWriterBase _objectReaderWriter;
		private readonly IFieldPath<TypeModel, PropertyInfoField> _fieldPath;

		public OpenGraphEnumerableStream(ObjectGraphReaderWriterBase objectReaderWriter,
			IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			_objectReaderWriter = objectReaderWriter;
			_fieldPath = fieldPath;
		}

		public IGraphWriter<TypeModel, PropertyInfoField> CreateNew()
		{
			var writer = new StreamReaderWriter(default(TData), _fieldPath);
			_writers.Add(writer);
			return writer;
		}

		public void Dispose()
		{
			_objectReaderWriter.CommitEnumerable<TData>(_fieldPath, _writers.Select(q => q.Graph));
		}

		private class StreamReaderWriter : ObjectGraphReaderWriterBase<TData>
		{
			public StreamReaderWriter(TData graph, IFieldPath<TypeModel, PropertyInfoField> fieldPath) :
				base(graph, fieldPath)
			{
			}
		}
	}
}
