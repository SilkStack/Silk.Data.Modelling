using System;
using System.Collections.Generic;
using System.Linq;
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

		public virtual void Transform(IModelTransformer transformer)
		{
			transformer.VisitModel(this);
			foreach (var field in Fields)
			{
				field.Transform(transformer);
			}
		}

		public ModelNode ResolveNode(Span<string> path)
		{
			var fieldCandidates = ((IModel)this).Fields;
			var field = default(IField);
			var modelNodePath = new List<ModelPathNode>();
			foreach (var fieldName in path.Slice(0, path.Length - 1))
			{
				field = fieldCandidates.FirstOrDefault(q => q.FieldName == fieldName);
				if (field == null)
					throw new ArgumentException("Invalid field path.", nameof(path));
				modelNodePath.Add(new TreePathNode(fieldName, field));
				fieldCandidates = ((IModel)field.FieldTypeModel).Fields;
			}

			if (path[path.Length - 1] == ".")
			{
				field = default(IField);
				modelNodePath.Add(RootPathNode.Instance);
			}
			else
			{
				var fieldName = path[path.Length - 1];
				field = fieldCandidates.FirstOrDefault(q => q.FieldName == fieldName);
				if (field == null)
					throw new ArgumentException("Invalid field path.", nameof(path));
				modelNodePath.Add(new FieldPathNode(fieldName, field));
			}

			return new ModelNode(field, modelNodePath);
		}

		public abstract IFieldResolver CreateFieldResolver();
		public abstract IFieldReference GetFieldReference(ISourceField sourceField);
		public abstract IFieldReference GetFieldReference(ITargetField targetField);
	}
}
