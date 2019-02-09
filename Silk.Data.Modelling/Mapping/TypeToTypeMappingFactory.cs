using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Creates type to type mappings.
	/// </summary>
	public class TypeToTypeMappingFactory : MappingFactoryBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeMappingFactory()
		{
			BindingFactories.Add(new CopySameValueTypesFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
			BindingFactories.Add(new CreateContainersForBoundFieldsFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
		}

		protected override IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> CreateMapping(
			MappingFactoryContext<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> mappingFactoryContext,
			BindingScope<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> bindingScope)
			=> new TypeToTypeMapping(mappingFactoryContext.FromModel, mappingFactoryContext.ToModel, bindingScope);
	}
}
