using Silk.Data.Modelling.Mapping;
using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	public class MappingOptions
	{
		public static MappingOptions DefaultObjectMappingOptions { get; }

		static MappingOptions()
		{
			DefaultObjectMappingOptions = new MappingOptions();
			DefaultObjectMappingOptions.Conventions.Add(CopySameTypes.Instance);
			DefaultObjectMappingOptions.Conventions.Add(FlattenSameTypes.Instance);
			DefaultObjectMappingOptions.Conventions.Add(MapReferenceTypes.Instance);
		}

		public List<IMappingConvention> Conventions { get; }
			= new List<IMappingConvention>();
	}
}
