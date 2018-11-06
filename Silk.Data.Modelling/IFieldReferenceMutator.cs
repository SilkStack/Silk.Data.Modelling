using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	public interface IFieldReferenceMutator
	{
		void MutatePath(List<ModelPathNode> pathNodes);
	}
}
