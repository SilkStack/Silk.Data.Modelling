using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Creates mappings between two models.
	/// </summary>
	public interface IMappingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		/// <summary>
		/// Create a mapping based on an intersection between two models.
		/// </summary>
		/// <param name="intersection"></param>
		/// <returns></returns>
		IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(IIntersection<TFromModel, TFromField, TToModel, TToField> intersection);
	}
}
