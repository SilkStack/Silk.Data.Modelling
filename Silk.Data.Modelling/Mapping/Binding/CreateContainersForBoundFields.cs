using Silk.Data.Modelling.Analysis;
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
					bindings.Add(
						new CreateContainersForBoundFieldsBinding<TFromModel, TFromField, TToModel, TToField>(
							parentPath, dependentPath
							)
						);
				}
				parentPath = parentPath.Parent;
			}

			bindings.Reverse();
			mappingFactoryContext.Bindings.InsertRange(firstDependentBindingIndex, bindings);
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
		private readonly IFieldPath<TFromModel, TFromField> _dependentPath;

		public CreateContainersForBoundFieldsBinding(IFieldPath<TToModel, TToField> path, IFieldPath<TFromModel, TFromField> dependentPath) :
			base(null, null, path.FinalField, path)
		{
			_dependentPath = dependentPath;
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

			if (_dependentPath != null)
			{
				//  check dependent path on source graph for accessability
				//  if it's inaccessible then the container object in the destination graph should be null
				if (!source.CheckPath(_dependentPath))
					return;
			}

			destination.CreateContainer(ToPath);
		}
	}
}
