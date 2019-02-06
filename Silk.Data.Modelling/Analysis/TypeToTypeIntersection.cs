namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Describes the overlapping relationship between two TypeModels.
	/// </summary>
	public class TypeToTypeIntersection : IntersectionBase<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
	{
		public TypeToTypeIntersection(TypeModel leftModel, TypeModel rightModel,
			IntersectedFields<PropertyInfoField, PropertyInfoField>[] intersectedFields)
			: base(leftModel, rightModel, intersectedFields)
		{
		}
	}
}
