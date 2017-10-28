using Silk.Data.Modelling.ResourceLoaders;

namespace Silk.Data.Modelling.Bindings
{
	public class SubMappingBinding : ModelBinding
	{
		public override BindingDirection Direction { get; }

		public SubMappingBinding(BindingDirection bindingDirection, string[] modelFieldPath,
			string[] viewFieldPath, IResourceLoader[] resourceLoaders)
			: base(modelFieldPath, viewFieldPath, resourceLoaders)
		{
			Direction = bindingDirection;
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
