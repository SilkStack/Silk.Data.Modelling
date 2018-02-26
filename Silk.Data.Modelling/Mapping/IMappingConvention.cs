namespace Silk.Data.Modelling.Mapping
{
	public interface IMappingConvention
	{
		void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder);
	}
}
