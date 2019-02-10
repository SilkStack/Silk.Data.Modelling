using Silk.Data.Modelling.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping.Binding
{
	/// <summary>
	/// Creates a binding that produces a new instance of a container type if the container needs to be written to by other bindings.
	/// </summary>
	/// <typeparam name="TFromModel"></typeparam>
	/// <typeparam name="TFromField"></typeparam>
	/// <typeparam name="TToModel"></typeparam>
	/// <typeparam name="TToField"></typeparam>
	public class CreateContainersForBoundFieldsFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields
			)
		{
		}

		private Node GetNode(List<Node> nodes, IEnumerable<string> path, IFieldPath<TToModel, TToField> fieldPath,
			List<EnumerableClaim> enumerableClaims)
		{
			var node = nodes.FirstOrDefault(q => q.Path.SequenceEqual(path));
			if (node != null)
				return node;

			node = new Node(path, fieldPath, enumerableClaims);
			nodes.Add(node);
			return node;
		}

		private void AddToNodes(List<Node> nodes, IBinding<TFromModel, TFromField, TToModel, TToField> binding,
			List<EnumerableClaim> enumerableClaims)
		{
			if (binding.ToField == null || binding.ToPath == null || !binding.ToPath.HasParent)
				return;

			var parent = binding.ToPath.Parent;
			while (parent.HasParent)
			{
				var node = GetNode(nodes, parent.Fields.Select(q => q.FieldName), parent, enumerableClaims);
				node.Bindings.Add(binding);
				parent = parent.Parent;
			}
		}

		private bool HasBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			string[] path)
		{
			return mappingFactoryContext.Bindings.Any(
				q => q.ToPath != null && q.ToPath.Fields.Select(field => field.FieldName).SequenceEqual(path)
				);
		}

		private void EnsureNodesAreBound(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			List<Node> nodes
			)
		{
			foreach (var node in nodes)
			{
				if (HasBinding(mappingFactoryContext, node.Path))
					continue;

				if (node.FieldPath.FinalField.IsEnumerableType)
				{
					var enumSource = node.GetNextUnclaimedEnumerable();
					mappingFactoryContext.Bindings.Add(
						new CreateContainersForBoundFieldsBinding<TFromModel, TFromField, TToModel, TToField>(
							node.FieldPath, node.Bindings.Select(q => q.FromPath).ToArray(), enumSource
							)
						);
				}
				else
				{
					mappingFactoryContext.Bindings.Add(
						new CreateContainersForBoundFieldsBinding<TFromModel, TFromField, TToModel, TToField>(
							node.FieldPath, node.Bindings.Select(q => q.FromPath).ToArray()
							)
						);
				}
			}
		}

		public void PostBindings(MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext)
		{
			//  group each node so that 1 node has multiple value checks for all bound values beneath the node
			var nodeList = new List<Node>();
			var enumerableClaims = new List<EnumerableClaim>();
			foreach (var binding in mappingFactoryContext.Bindings)
			{
				AddToNodes(nodeList, binding, enumerableClaims);
			}

			//  sort the node list by depth, deepest first, for binding to enumerable sources
			nodeList = nodeList.OrderByDescending(q => q.Path.Length).ToList();

			EnsureNodesAreBound(mappingFactoryContext, nodeList);
		}

		private class Node
		{
			public string[] Path { get; }
			public IFieldPath<TToModel, TToField> FieldPath { get; }

			private readonly List<EnumerableClaim> _enumerableClaims;

			public List<IBinding<TFromModel, TFromField, TToModel, TToField>> Bindings { get; }
				= new List<IBinding<TFromModel, TFromField, TToModel, TToField>>();

			public Node(IEnumerable<string> path, IFieldPath<TToModel, TToField> fieldPath,
				List<EnumerableClaim> enumerableClaims)
			{
				Path = path.ToArray();
				FieldPath = fieldPath;

				_enumerableClaims = enumerableClaims;
			}

			public IFieldPath<TFromModel, TFromField> GetNextUnclaimedEnumerable()
			{
				var binding = Bindings.FirstOrDefault();
				if (binding == null || !binding.FromPath.HasParent)
					throw new InvalidOperationException("No source enumerable found for mapping.");

				var parent = binding.FromPath.Parent;
				while (parent.HasParent)
				{
					if (!parent.FinalField.IsEnumerableType || IsClaimed(parent))
						continue;

					var claim = new EnumerableClaim(parent);
					_enumerableClaims.Add(claim);

					return parent;
				}

				throw new InvalidOperationException("No source enumerable found for mapping.");
			}

			private bool IsClaimed(IFieldPath<TFromModel, TFromField> fieldPath)
			{
				return _enumerableClaims.Any(
					q => q.Path.SequenceEqual(fieldPath.Fields.Select(fld => fld.FieldName))
					);
			}
		}

		private class EnumerableClaim
		{
			public string[] Path { get; }

			public EnumerableClaim(IFieldPath<TFromModel, TFromField> fieldPath)
			{
				Path = fieldPath.Fields.Select(q => q.FieldName).ToArray();
			}
		}
	}

	public class CreateContainersForBoundFieldsBinding<TFromModel, TFromField, TToModel, TToField> :
		BindingBase<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		/// <summary>
		/// Dependent path.
		/// NOT a FromPath, this isn't the source of a binding operation, it's a path that needs checking for nulls before performing the assignment operation.
		/// </summary>
		private readonly IFieldPath<TFromModel, TFromField>[] _dependentPaths;

		public CreateContainersForBoundFieldsBinding(IFieldPath<TToModel, TToField> path, IFieldPath<TFromModel, TFromField>[] dependentPaths) :
			base(null, null, path.FinalField, path)
		{
			_dependentPaths = dependentPaths;
		}

		public CreateContainersForBoundFieldsBinding(IFieldPath<TToModel, TToField> path, IFieldPath<TFromModel, TFromField>[] dependentPaths,
			IFieldPath<TFromModel, TFromField> enumSource) :
			base(null, null, path.FinalField, path, enumSource)
		{
			_dependentPaths = dependentPaths;
		}

		public override void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			var destinationReader = destination as IGraphReader<TToModel, TToField>;
			if (destinationReader != null)
			{
				if (!destinationReader.CheckPath(ToPath) ||
					destinationReader.CheckContainer(ToPath))
					return;
			}

			if (_dependentPaths != null && _dependentPaths.Length > 0)
			{
				var foundValidPath = false;
				//  check dependent path on source graph for accessability
				//  if it's inaccessible then the container object in the destination graph should be null
				foreach (var path in _dependentPaths)
				{
					if (source.CheckPath(path))
					{
						foundValidPath = true;
						break;
					}
				}
				if (!foundValidPath)
					return;
			}

			destination.CreateContainer(ToPath);
		}
	}
}
