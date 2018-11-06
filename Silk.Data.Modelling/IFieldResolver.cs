namespace Silk.Data.Modelling
{
	public interface IFieldResolver
	{
		ModelNode ResolveNode(IFieldReference fieldReference);
	}
}
