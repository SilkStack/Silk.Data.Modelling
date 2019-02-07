namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// A mapping between two CLR types.
	/// </summary>
	public class TypeToTypeMapping : IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeModel FromModel { get; }

		public TypeModel ToModel { get; }
	}
}
