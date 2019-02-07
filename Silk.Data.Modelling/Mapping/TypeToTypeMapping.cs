using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// A mapping between two CLR types.
	/// </summary>
	public class TypeToTypeMapping : IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeModel FromModel { get; }

		public TypeModel ToModel { get; }

		public IBinding<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>[] Bindings { get; }

		public TypeToTypeMapping(IEnumerable<IBinding<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> bindings)
		{
			Bindings = bindings.ToArray();
		}

		public void Map(IGraphReader<TypeModel, PropertyInfoField> source, IGraphWriter<TypeModel, PropertyInfoField> destination)
		{
			foreach (var binding in Bindings)
				binding.Run(source, destination);
		}
	}
}
