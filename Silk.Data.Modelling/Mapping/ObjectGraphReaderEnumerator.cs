using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping
{
	public class ObjectGraphReaderEnumerator<TGraph, TData> : IGraphReaderEnumerator<TypeModel, PropertyInfoField>
		where TGraph : class
	{
		private readonly IEnumerator<TData> _enumerator;
		private readonly TGraph _graph;

		public IGraphReader<TypeModel, PropertyInfoField> Current { get; private set; }

		public ObjectGraphReaderEnumerator(TGraph graph, IEnumerator<TData> enumerator)
		{
			_enumerator = enumerator;
			_graph = graph;
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

			Current = null;
			return true;
		}
	}
}
