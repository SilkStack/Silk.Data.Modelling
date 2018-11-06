using System;
using System.Collections.Generic;
using System.Linq;
using Silk.Data.Modelling.Mapping;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A model of a <see cref="Type"/>.
	/// </summary>
	public abstract partial class TypeModel : ModelBase<IPropertyField>
	{
		public abstract Type Type { get; }

		protected abstract class PropertyReference
		{
			public abstract IReadOnlyCollection<ModelPathNode> Path { get; }
		}
	}

	/// <summary>
	/// A model of a <see cref="Type"/>.
	/// </summary>
	public class TypeModel<T> : TypeModel
	{
		public override Type Type { get; } = typeof(T);

		public override IPropertyField[] Fields { get; }

		internal TypeModel(IPropertyField[] fields)
		{
			Fields = fields;
		}

		public override IFieldResolver CreateFieldResolver()
			=> new FieldResolver(this);

		public override IFieldReference GetFieldReference(ISourceField sourceField)
			=> GetFieldReference(sourceField.FieldPath);

		public override IFieldReference GetFieldReference(ITargetField targetField)
			=> GetFieldReference(targetField.FieldPath);

		private IFieldReference GetFieldReference(string[] path)
		{
			if (path.Length == 1 && path[0] == ".")
				return new RootReference(this);
			var (field, pathNodes) = GetField(path);
			return new FieldReference(this, field, pathNodes);
		}

		private (IPropertyField, List<ModelPathNode>) GetField(string[] path)
		{
			var pathSpan = new Span<string>(path);
			var fieldCandidates = Fields;
			var field = default(IPropertyField);
			var pathNodes = new List<ModelPathNode>();
			foreach (var fieldName in pathSpan.Slice(0, pathSpan.Length - 1))
			{
				field = fieldCandidates.FirstOrDefault(q => q.FieldName == fieldName);
				if (field == null)
					throw new ArgumentException("Invalid field path.", nameof(path));
				fieldCandidates = field.FieldTypeModel.Fields;
				pathNodes.Add(new TreePathNode(fieldName, field));
			}

			if (path[path.Length - 1] == ".")
			{
				field = default(IPropertyField);
				pathNodes.Add(RootPathNode.Instance);
			}
			else
			{
				var fieldName = path[path.Length - 1];
				field = fieldCandidates.FirstOrDefault(q => q.FieldName == fieldName);
				if (field == null)
					throw new ArgumentException("Invalid field path.", nameof(path));
				pathNodes.Add(new FieldPathNode(fieldName, field));
			}

			return (field, pathNodes);
		}

		private class RootReference : PropertyReference, IFieldReference
		{
			private readonly TypeModel<T> _model;

			public IField Field => null;

			public IModel Model => _model;

			public override IReadOnlyCollection<ModelPathNode> Path { get; }

			public RootReference(TypeModel<T> model)
			{
				_model = model;
				Path = new ModelPathNode[] { RootPathNode.Instance };
			}
		}

		private class FieldReference : PropertyReference, IFieldReference
		{
			private readonly TypeModel<T> _model;
			private readonly IPropertyField _field;

			public IField Field => _field;
			public IModel Model => _model;

			public override IReadOnlyCollection<ModelPathNode> Path { get; }

			public FieldReference(TypeModel<T> model, IPropertyField propertyField,
				IReadOnlyCollection<ModelPathNode> path)
			{
				_model = model;
				_field = propertyField;
				Path = path;
			}
		}

		private class FieldResolver : IFieldResolver
		{
			private readonly TypeModel<T> _model;
			private readonly List<IFieldReferenceMutator> _mutators
				= new List<IFieldReferenceMutator>();

			public FieldResolver(TypeModel<T> model)
			{
				_model = model;
			}

			public void AddMutator(IFieldReferenceMutator mutator)
				=> _mutators.Add(mutator);

			public void RemoveMutator(IFieldReferenceMutator mutator)
				=> _mutators.Remove(mutator);

			public ModelNode ResolveNode(IFieldReference fieldReference)
			{
				switch (fieldReference)
				{
					case PropertyReference propertyReference:
						if (_mutators.Count == 0)
							return new ModelNode(fieldReference.Field, propertyReference.Path);

						var pathCopy = new List<ModelPathNode>(propertyReference.Path);
						foreach (var mutator in _mutators)
							mutator.MutatePath(pathCopy);
						return new ModelNode(fieldReference.Field, pathCopy);
					default:
						throw new InvalidOperationException($"Field reference type '{fieldReference.GetType().FullName}' not supported.");
				}
			}
		}
	}
}
