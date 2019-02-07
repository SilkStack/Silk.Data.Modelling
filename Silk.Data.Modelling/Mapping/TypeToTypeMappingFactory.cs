using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Creates type to type mappings.
	/// </summary>
	public class TypeToTypeMappingFactory : IMappingFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> CreateMapping(
			IIntersection<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> intersection
			)
		{
			throw new System.NotImplementedException();
		}
	}
}
