using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping
{
	public class ObjectGraphReaderEnumerator<TData> : IGraphReaderEnumerator<TypeModel, PropertyInfoField>
	{
		private readonly IEnumerator<TData> _enumerator;
		private readonly IFieldPath<TypeModel, PropertyInfoField> _fieldPath;

		public IGraphReader<TypeModel, PropertyInfoField> Current { get; private set; }

		public ObjectGraphReaderEnumerator(IEnumerator<TData> enumerator, IFieldPath<TypeModel, PropertyInfoField> fieldPath)
		{
			_enumerator = enumerator;
			_fieldPath = fieldPath;
		}

		public void Dispose()
		{
			_enumerator.Dispose();
		}

		public bool MoveNext()
		{
			var ok = _enumerator.MoveNext();
			if (!ok)
				return false;

			Current = new EnumeratorReaderWriter(_enumerator.Current, _fieldPath);
			return true;
		}

		private class EnumeratorReaderWriter : ObjectGraphReaderWriterBase<TData>
		{
			public EnumeratorReaderWriter(TData graph, IFieldPath<TypeModel, PropertyInfoField> fieldPath) :
				base(graph, fieldPath)
			{
			}
		}
	}
}
