namespace Silk.Data.Modelling.Mapping
{
	public interface IMappingConvention
	{
		void CreateMappings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder);
	}
}
