using Silk.Data.Modelling.Mapping;
using System.Collections.Generic;
using System.Linq;

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
			ret.Conventions.Add(CopyExplicitCast.Instance);
			ret.Conventions.Add(CastNumericTypes.Instance);
			ret.Conventions.Add(MapReferenceTypes.Instance);
			ret.Conventions.Add(ConvertToStringWithToString.Instance);

			ret.Conventions.Add(FlattenSameTypes.Instance);
			ret.Conventions.Add(new InflateSameTypes(true));
			return ret;
		}

		public List<IMappingConvention> Conventions { get; }
			= new List<IMappingConvention>();

		public void AddMappingOverride<TFrom, TTo>(IObjectMappingOverride<TFrom, TTo> mappingOverride)
		{
			var objectMappingConvention = Conventions.OfType<UseObjectMappingOverrides>().FirstOrDefault();
			if (objectMappingConvention == null)
				throw new System.InvalidOperationException("UseObjectMappingOverrides is required.");
			objectMappingConvention.AddMappingOverride<TFrom, TTo>(mappingOverride);
		}

		public void RemoveMappingOverride<TFrom, TTo>(IObjectMappingOverride<TFrom, TTo> mappingOverride)
		{
			var objectMappingConvention = Conventions.OfType<UseObjectMappingOverrides>().FirstOrDefault();
			if (objectMappingConvention == null)
				throw new System.InvalidOperationException("UseObjectMappingOverrides is required.");
			objectMappingConvention.RemoveMappingOverride<TFrom, TTo>(mappingOverride);
		}
	}
}
