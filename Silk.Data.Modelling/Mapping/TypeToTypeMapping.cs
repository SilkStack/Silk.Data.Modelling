using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// A mapping between two CLR types.
	/// </summary>
	public class TypeToTypeMapping : MappingBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeMapping(TypeModel fromModel, TypeModel toModel,
			IEnumerable<IBinding<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> bindings) :
			base(fromModel, toModel, bindings)
		{
		}
	}
}
