using Silk.Data.Modelling.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	public delegate IEnumerable<(ISourceField sourceField, ITargetField targetField)> GetBindCandidatePairs(SourceModel fromModel, TargetModel toModel, MappingBuilder builder);

	public class MappingOptions
	{
		public static MappingOptions DefaultObjectMappingOptions { get; }
			= CreateObjectMappingOptions();

		public static MappingOptions CreateObjectMappingOptions()
		{
			var ret = new MappingOptions();
			ret.Conventions.Add(new UseObjectMappingOverrides());
			ret.Conventions.Add(CreateInstanceAsNeeded.Instance);
			ret.Conventions.Add(MapOverriddenTypes.Instance);

			//  object type conversions
			ret.Conventions.Add(CopyExplicitCast.Instance);
			ret.Conventions.Add(MapReferenceTypes.Instance);
			ret.Conventions.Add(CreateInstancesOfPropertiesAsNeeded.Instance);

			//  straight up copies
			ret.Conventions.Add(CopySameTypes.Instance);

			//  framework types casting
			ret.Conventions.Add(CastNumericTypes.Instance);

			//  string conversions
			ret.Conventions.Add(ConvertToStringWithToString.Instance);
			ret.Conventions.Add(CopyTryParse.Instance);

			return ret;
		}

		public List<IMappingConvention> Conventions { get; }
			= new List<IMappingConvention>();

		public GetBindCandidatePairs BindingCandidatesDelegate { get; set; }
			= ConventionUtilities.GetBindCandidatePairs;

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
