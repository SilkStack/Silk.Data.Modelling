using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class MapOverriddenTypes : IMappingConvention
	{
		public static MapOverriddenTypes Instance { get; } = new MapOverriddenTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			var mappingOverridesConvention = builder.Conventions.OfType<UseObjectMappingOverrides>()
				.FirstOrDefault();
			if (mappingOverridesConvention == null)
				return;

			foreach (var (fromField, toField) in builder.BindingCandidatesDelegate(fromModel, toModel, builder))
			{
				var (fromType, toType) = ConventionUtilities.GetCompareTypes(fromField, toField);

				if (builder.IsBound(toField))
					continue;

				var mappingOverrides = mappingOverridesConvention.GetOverrides(fromType, toType);
				if (mappingOverrides.Length > 0)
				{
					var fromTypeModel = TypeModel.GetModelOf(fromType);
					var toTypeModel = TypeModel.GetModelOf(toType);

					//  if a mapping already exists, use it, otherwise build it
					if (!builder.MappingStore.TryGetMapping(fromTypeModel, toTypeModel, out var subMapping) &&
						!builder.BuilderStack.IsBeingMapped(fromTypeModel, toTypeModel))
					{
						var options = new MappingOptions();
						foreach (var convention in builder.Conventions)
							options.Conventions.Add(convention);

						var subBuilder = new MappingBuilder(fromTypeModel, toTypeModel,
							options, builder.MappingStore, builder.BuilderStack);
						subMapping = subBuilder.BuildMapping();
					}

					builder
						.Bind(toField)
						.From(fromField)
						.MapUsing<SubmappingBinding, MappingStore>(builder.MappingStore);
				}
			}
		}
	}
}
