using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.ResourceLoaders
{
	/// <summary>
	/// Can bulk-load resources needed to complete a mapping operation.
	/// </summary>
	public interface IResourceLoader
	{
		Task LoadResourcesAsync(IView view, ICollection<IContainerReadWriter> sources, MappingContext mappingContext);
	}
}
