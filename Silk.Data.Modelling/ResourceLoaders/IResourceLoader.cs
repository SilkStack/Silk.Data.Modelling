using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.ResourceLoaders
{
	/// <summary>
	/// Can bulk-load resources needed to complete a mapping operation.
	/// </summary>
	public interface IResourceLoader
	{
		Task LoadResourcesAsync(ICollection<IContainerReadWriter> sources, MappingContext mappingContext);

		//  OLD API

		///// <summary>
		///// Load all resources from a set of views into the provided mapping context.
		///// </summary>
		///// <returns></returns>
		//Task LoadResourcesAsync(ICollection<IContainer> containers, MappingContext mappingContext);

		///// <summary>
		///// Load all resources from a set of models into the provided mapping context.
		///// </summary>
		///// <returns></returns>
		//Task LoadResourcesAsync(ICollection<IModelReadWriter> modelReadWriters, MappingContext mappingContext);
	}
}
