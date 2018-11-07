using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Describes a node on a model.
	/// </summary>
	public struct ModelNode
	{
		public IField Field { get; }
		public IReadOnlyCollection<ModelPathNode> Path { get; }

		public ModelNode(IField field, IReadOnlyCollection<ModelPathNode> path)
		{
			Field = field;
			Path = path;
		}
	}

	public enum ModelPathNodeType
	{
		Root,
		Field,
		Tree
	}

	public abstract class ModelPathNode
	{
		public abstract ModelPathNodeType PathNodeType { get; }
		public abstract string PathNodeName { get; }
		public abstract IField Field { get; }
	}

	public class RootPathNode : ModelPathNode
	{
		public static RootPathNode Instance { get; } = new RootPathNode();

		public override ModelPathNodeType PathNodeType => ModelPathNodeType.Root;
		public override string PathNodeName => throw new System.NotSupportedException();
		public override IField Field => throw new System.NotSupportedException();
	}

	public class FieldPathNode : ModelPathNode
	{
		public override ModelPathNodeType PathNodeType => ModelPathNodeType.Field;

		public override string PathNodeName { get; }

		public override IField Field { get; }

		public FieldPathNode(string pathNodeName, IField field)
		{
			PathNodeName = pathNodeName;
			Field = field;
		}
	}

	public class TreePathNode : ModelPathNode
	{
		public override ModelPathNodeType PathNodeType => ModelPathNodeType.Tree;

		public override string PathNodeName { get; }

		public override IField Field { get; }

		public TreePathNode(string pathNodeName, IField field)
		{
			PathNodeName = pathNodeName;
			Field = field;
		}
	}
}
