using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// A mapping between two CLR types.
	/// </summary>
	public class TypeToTypeMapping : MappingBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeMapping(TypeModel fromModel, TypeModel toModel,
			BindingScope<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> bindingScope) :
			base(fromModel, toModel, bindingScope)
		{
		}
	}
}
