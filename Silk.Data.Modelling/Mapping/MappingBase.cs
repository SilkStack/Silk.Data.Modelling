using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	public abstract class MappingBase<TFromModel, TFromField, TToModel, TToField> : IMapping<TFromModel, TFromField, TToModel, TToField>
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
		where TFromField : class, IField
		where TToField : class, IField
	{
		public TFromModel FromModel { get; }
		public TToModel ToModel { get; }
		public BindingScope<TFromModel, TFromField, TToModel, TToField> BindingScope { get; }

		public MappingBase(TFromModel fromModel, TToModel toModel,
			BindingScope<TFromModel, TFromField, TToModel, TToField> bindingScope)
		{
			FromModel = fromModel;
			ToModel = toModel;
			BindingScope = bindingScope;
		}

		public void Map(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			BindingScope.RunScope(source, destination);
		}
	}
}
