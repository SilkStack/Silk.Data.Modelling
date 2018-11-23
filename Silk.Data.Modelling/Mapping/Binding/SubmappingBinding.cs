using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class SubmappingBinding : IMappingBindingFactory<MappingStore>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, MappingStore mappingStore)
		{
			var fromElementType = fromField.Field.ElementType;
			var toElementType = toField.Field.ElementType;

			if (fromElementType != null && toElementType != null &&
				MapReferenceTypes.IsReferenceType(fromElementType) && MapReferenceTypes.IsReferenceType(toElementType))
			{
				return Activator.CreateInstance(typeof(SubmappingBinding<,>).MakeGenericType(fromElementType, toElementType), new object[] {
					TypeModel.GetModelOf(fromElementType),
					TypeModel.GetModelOf(toElementType),
					mappingStore,
					fromField, toField
				}) as MappingBinding;
			}
			else
			{
				return new SubmappingBinding<TFrom, TTo>(
					fromField.Field.FieldModel, toField.Field.FieldModel,
					mappingStore, fromField, toField);
			}
		}
	}

	public abstract class SubmappingBindingBase : MappingBinding
	{
		public abstract Mapping Mapping { get; }

		public SubmappingBindingBase(IFieldReference from, IFieldReference to)
			: base(from, to)
		{
		}
	}

	public class SubmappingBinding<TFrom, TTo> : SubmappingBindingBase
	{
		private readonly IModel _fromModel;
		private readonly IModel _toModel;
		private readonly IFieldReferenceMutator _fromMutator;
		private readonly IFieldReferenceMutator _toMutator;

		public MappingStore MappingStore { get; }

		private Mapping _mapping;
		public override Mapping Mapping
		{
			get
			{
				if (_mapping == null)
					MappingStore.TryGetMapping(_fromModel, _toModel, out _mapping);
				return _mapping;
			}
		}

		public SubmappingBinding(IModel fromModel, IModel toModel, MappingStore mappingStore,
			IFieldReference from, IFieldReference to) :
			base(from, to)
		{
			_fromModel = fromModel;
			_toModel = toModel;
			MappingStore = mappingStore;

			var fromNode = fromModel.CreateFieldResolver().ResolveNode(from);
			var toNode = toModel.CreateFieldResolver().ResolveNode(to);

			_fromMutator = new Mutator(fromNode.Path);
			_toMutator = new Mutator(toNode.Path);
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var value = from.ReadField<TFrom>(From);
			if (value == null)
				return;

			from.FieldResolver.AddMutator(_fromMutator);
			to.FieldResolver.AddMutator(_toMutator);

			Mapping.PerformMapping(from, to);

			from.FieldResolver.RemoveMutator(_fromMutator);
			to.FieldResolver.RemoveMutator(_toMutator);
		}

		private class Mutator : IFieldReferenceMutator
		{
			private readonly IReadOnlyCollection<ModelPathNode> _pathNodes;

			public Mutator(IReadOnlyCollection<ModelPathNode> pathNodes)
			{
				_pathNodes = FixMutatePath(pathNodes);
			}

			public void MutatePath(List<ModelPathNode> pathNodes)
			{
				pathNodes.InsertRange(0, _pathNodes);
			}

			private static IReadOnlyCollection<ModelPathNode> FixMutatePath(IReadOnlyCollection<ModelPathNode> pathNodes)
			{
				if (!pathNodes.OfType<FieldPathNode>().Any())
					return pathNodes;
				return new List<ModelPathNode>(
					pathNodes.Select(q => FixMutateNode(q))
					);
			}

			private static ModelPathNode FixMutateNode(ModelPathNode node)
			{
				if (node.PathNodeType != ModelPathNodeType.Field)
					return node;
				return new TreePathNode(node.PathNodeName, node.Field);
			}
		}
	}
}
