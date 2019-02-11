using Silk.Data.Modelling.Mapping;

namespace Silk.Data.Modelling
{
	public interface IReaderWriterFactory<TModel, TField>
		where TModel : IModel<TField>
		where TField : class, IField
	{
		IGraphReader<TModel, TField> CreateGraphReader<T>(T graph)
			where T : class;
		IGraphWriter<TModel, TField> CreateGraphWriter<T>(T graph)
			where T : class;
	}
}
