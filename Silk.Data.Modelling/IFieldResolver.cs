namespace Silk.Data.Modelling
{
	public interface IFieldResolver
	{
		void AddMutator(IFieldReferenceMutator mutator);
		void RemoveMutator(IFieldReferenceMutator mutator);

		ModelNode ResolveNode(IFieldReference fieldReference);
	}
}
