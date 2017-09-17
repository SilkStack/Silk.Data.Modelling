using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Can bulk-load resources needed to complete a mapping operation.
	/// </summary>
	public interface IResourceLoader
	{
		/// <summary>
		/// Load all resources into the provided mapping context.
		/// </summary>
		/// <returns></returns>
		Task LoadResourcesAsync(IEnumerable<IContainer> containers, MappingContext mappingContext);
	}
}
