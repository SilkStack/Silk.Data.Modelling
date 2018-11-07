using Silk.Data.Modelling.Mapping;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Common abstract implementation of <see cref="IModel{TField}"/>.
	/// </summary>
	/// <typeparam name="TField"></typeparam>
	public abstract class ModelBase<TField> : IModel<TField>
		where TField : class, IField
	{
		public abstract TField[] Fields { get; }
		IField[] IModel.Fields => Fields;

		public abstract IFieldReference Root { get; }

		public virtual void Transform(IModelTransformer transformer)
		{
			transformer.VisitModel(this);
			foreach (var field in Fields)
			{
				field.Transform(transformer);
			}
		}

		public abstract IFieldResolver CreateFieldResolver();
		public abstract IFieldReference GetFieldReference(ISourceField sourceField);
		public abstract IFieldReference GetFieldReference(ITargetField targetField);
	}
}
