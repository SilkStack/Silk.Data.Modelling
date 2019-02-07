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
			BindingFactories.Add(new CreateInstancesOfContainerTypesFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
			BindingFactories.Add(new CopySameTypesFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>());
		}
	}
}
