using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping
{
	public class DefaultMappingFactory<TFromModel, TFromField, TToModel, TToField> :
		MappingFactoryBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public DefaultMappingFactory(IEnumerable<IBindingFactory<TFromModel, TFromField, TToModel, TToField>> bindingFactories = null)
		{
			foreach (var factory in bindingFactories ?? GetDefaultBindingFactories())
				BindingFactories.Add(factory);
		}

		protected virtual IEnumerable<IBindingFactory<TFromModel, TFromField, TToModel, TToField>> GetDefaultBindingFactories()
		{
			yield return new BindValuesWithConvertersFactory<TFromModel, TFromField, TToModel, TToField>();
			yield return new CreateContainersForBoundFieldsFactory<TFromModel, TFromField, TToModel, TToField>();
		}

		protected override IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			BindingScope<TFromModel, TFromField, TToModel, TToField> bindingScope
			)
			=> new Mapping(mappingFactoryContext.FromModel, mappingFactoryContext.ToModel, bindingScope);

		public class Mapping : MappingBase<TFromModel, TFromField, TToModel, TToField>
		{
			public Mapping(TFromModel fromModel, TToModel toModel,
				BindingScope<TFromModel, TFromField, TToModel, TToField> bindingScope) :
				base(fromModel, toModel, bindingScope)
			{
			}
		}
	}
}
