using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public interface IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : IField
		where TToField : IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		bool GetBinding(IntersectedFields intersectedFields, out IBinding binding);
	}
}
