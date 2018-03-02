using Silk.Data.Modelling.Mapping;
using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	public class MappingOptions
	{
		public static MappingOptions DefaultObjectMappingOptions { get; }
			= CreateObjectMappingOptions();

		public static MappingOptions CreateObjectMappingOptions()
		{
			var ret = new MappingOptions();
			ret.Conventions.Add(new UseObjectMappingOverrides());
			ret.Conventions.Add(CreateInstanceAsNeeded.Instance);
			ret.Conventions.Add(CopySameTypes.Instance);
			ret.Conventions.Add(FlattenSameTypes.Instance);
			ret.Conventions.Add(MapReferenceTypes.Instance);
			return ret;
		}

		public List<IMappingConvention> Conventions { get; }
			= new List<IMappingConvention>();
	}
}
