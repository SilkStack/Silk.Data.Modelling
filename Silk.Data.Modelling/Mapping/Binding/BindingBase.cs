namespace Silk.Data.Modelling.Mapping.Binding
{
	public abstract class BindingBase<TFromModel, TFromField, TToModel, TToField> :
		IBinding<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public TToField ToField { get; }
		public IFieldPath<TToModel, TToField> ToPath { get; }
		public TFromField FromField { get; }
		public IFieldPath<TFromModel, TFromField> FromPath { get; }

		public BindingBase(TFromField fromField, IFieldPath<TFromModel, TFromField> fromPath,
			TToField toField, IFieldPath<TToModel, TToField> toPath)
		{
			ToField = toField;
			ToPath = toPath;
			FromField = fromField;
			FromPath = fromPath;
		}

		public abstract void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination);
	}
}
