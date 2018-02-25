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

		public override T ReadTransformedValue<T>(IContainerReadWriter from, MappingContext mappingContext)
		{
			return (T)mappingContext.Resources.Retrieve(from, string.Join(".", ModelFieldPath));
		}
	}
}
