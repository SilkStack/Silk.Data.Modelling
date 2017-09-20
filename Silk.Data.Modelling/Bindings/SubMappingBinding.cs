namespace Silk.Data.Modelling.Bindings
{
	public class SubMappingBinding : ModelBinding
	{
		public override BindingDirection Direction => BindingDirection.Bidirectional;

		public SubMappingBinding(string[] modelFieldPath, string[] viewFieldPath)
			: base(modelFieldPath, viewFieldPath)
		{
		}

		public override object ReadFromModel(IModelReadWriter modelReadWriter, MappingContext mappingContext)
		{
			return mappingContext.Resources.Retrieve(modelReadWriter, $"subMapped:{string.Join(".", ModelFieldPath)}");
		}

		public override object ReadFromContainer(IContainer container, MappingContext mappingContext)
		{
			return mappingContext.Resources.Retrieve(container, $"subMapped:{string.Join(".", ModelFieldPath)}");
		}
	}
}
