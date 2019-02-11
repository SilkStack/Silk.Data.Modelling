namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Describes the overlapping relationship between two TypeModels.
	/// </summary>
	public class TypeToTypeIntersection : IntersectionBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeIntersection(TypeModel leftModel, TypeModel rightModel,
			IntersectedFields<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>[] intersectedFields)
			: base(leftModel, rightModel, intersectedFields)
		{
		}
	}
}
