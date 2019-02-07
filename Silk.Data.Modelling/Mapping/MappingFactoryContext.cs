using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping
{
	public class MappingFactoryContext<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public IMappingFactory<TFromModel, TFromField, TToModel, TToField> Factory { get; }

		public List<IBinding<TFromModel, TFromField, TToModel, TToField>> Bindings { get; }
			= new List<IBinding<TFromModel, TFromField, TToModel, TToField>>();

		public MappingFactoryContext(IMappingFactory<TFromModel, TFromField, TToModel, TToField> factory)
		{
			Factory = factory;
		}
	}
}
