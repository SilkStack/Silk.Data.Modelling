using System;

namespace Silk.Data.Modelling.Mapping
{
	public class OpenGraphEnumerableStream<TData> : IGraphWriterStream<TypeModel, PropertyInfoField>
	{
		public IGraphWriter<TypeModel, PropertyInfoField> CreateNew()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
