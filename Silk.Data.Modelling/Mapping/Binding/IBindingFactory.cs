using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public interface IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields
			);
	}
}
