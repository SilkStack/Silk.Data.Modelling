using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;
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
	public class CreateInstancesOfContainerTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private bool IsBindingCandidate(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields
			)
		{
			if (
				!intersectedFields.RightPath.HasParent || //  field has no parent, ie. no container
				!intersectedFields.RightField.CanWrite || //  field isn't writeable so can't have a binding, early abort
				mappingFactoryContext.IsToFieldBound(intersectedFields.RightPath.Parent.FinalField) //  parent is already bound, nothing to do here then
				)
				return false;
			return true;
		}

		private int FindDependentBindingIndex(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IFieldPath<TToModel, TToField> parentPath
			)
		{
			return mappingFactoryContext.Bindings.FindIndex(
				binding => binding.ToPath.Fields.Contains(parentPath.FinalField)
				);
		}

		public void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields
			)
		{
			if (!IsBindingCandidate(mappingFactoryContext, intersectedFields))
				return;

			var parentPath = intersectedFields.RightPath.Parent;
			var firstDependentBindingIndex = FindDependentBindingIndex(
				mappingFactoryContext, parentPath
				);
			if (firstDependentBindingIndex < 0)
				return;

			//  we know there are objects depending on the graph having containers down to this level now
			//  check for any that aren't bound and bind to a factory as needed
			var bindings = new List<IBinding<TFromModel, TFromField, TToModel, TToField>>();
			var dependentPath = mappingFactoryContext.Bindings[firstDependentBindingIndex].FromPath;

			while (parentPath != null && parentPath.HasParent)
			{
				if (!mappingFactoryContext.IsToFieldBound(parentPath.FinalField))
				{
					var bindingBuilder = new BindingBuilder(parentPath, dependentPath);
					parentPath.FinalField.Dispatch(bindingBuilder);
					bindings.Add(bindingBuilder.Binding);
				}
				parentPath = parentPath.Parent;
			}

			bindings.Reverse();
			mappingFactoryContext.Bindings.InsertRange(firstDependentBindingIndex, bindings);
		}

		private class BindingBuilder : IFieldGenericExecutor
		{
			private readonly IFieldPath<TToModel, TToField> _path;
			private readonly IFieldPath<TFromModel, TFromField> _dependentPath;

			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			public BindingBuilder(IFieldPath<TToModel, TToField> path, IFieldPath<TFromModel, TFromField> dependentPath)
			{
				_path = path;
				_dependentPath = dependentPath;
			}

			public void Execute<TField, TData>(IField field) where TField : class, IField
				=> CreateBinding<TData>();

			private void CreateBinding<TData>()
			{
				Binding = new CreateInstanceBinding<TFromModel, TFromField, TToModel, TToField, TData>(
					_path,
					_dependentPath,
					TypeFactoryHelper.GetFactory<TData>());
			}
		}
	}

	public class CreateInstanceBinding<TFromModel, TFromField, TToModel, TToField, TData> :
		IBinding<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		private readonly IFieldPath<TToModel, TToField> _path;
		/// <summary>
		/// Dependent path.
		/// NOT a FromPath, this isn't the source of a binding operation, it's a path that needs checking for nulls before performing the assignment operation.
		/// </summary>
		private readonly IFieldPath<TFromModel, TFromField> _dependentPath;
		private readonly Func<TData> _factory;

		public TToField ToField => _path.FinalField;

		public TFromField FromField => null;

		public IFieldPath<TToModel, TToField> ToPath => _path;

		public IFieldPath<TFromModel, TFromField> FromPath => null;

		public CreateInstanceBinding(IFieldPath<TToModel, TToField> path, IFieldPath<TFromModel, TFromField> dependentPath, Func<TData> factory)
		{
			_path = path;
			_dependentPath = dependentPath;
			_factory = factory;
		}

		public void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			var destinationReader = destination as IGraphReader<TToModel, TToField>;
			if (destinationReader != null)
			{
				if (!destinationReader.CheckPath(_path) ||
					destinationReader.Read<TData>(_path) != null)
					return;
			}

			if (_dependentPath != null)
			{
				//  check dependent path on source graph for accessability
				//  if it's inaccessible then the container object in the destination graph should be null
				if (!source.CheckPath(_dependentPath))
					return;
			}

			destination.Write(_path, _factory());
		}
	}
}
