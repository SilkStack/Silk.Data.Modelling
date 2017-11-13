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

		public override void CopyBindingValue(IContainerReadWriter from, IContainerReadWriter to, MappingContext mappingContext)
		{
			WriteValue<object>(to, mappingContext.Resources.Retrieve(this, string.Join(".", ModelFieldPath)));
		}
	}
}
