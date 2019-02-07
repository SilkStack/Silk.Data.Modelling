using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;
using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CreateInstancesOfContainerTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public bool GetBinding(
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields,
			out IBinding<TFromModel, TFromField, TToModel, TToField> binding)
		{
			if (!intersectedFields.RightPath.HasParent)
			{
				binding = null;
				return false;
			}

			var parentPath = intersectedFields.RightPath.Parent;
			var bindingBuilder = new BindingBuilder(parentPath);
			if (parentPath.FinalField == null)
				parentPath.Model.Dispatch(bindingBuilder);
			else
				parentPath.FinalField.Dispatch(bindingBuilder);

			binding = bindingBuilder.Binding;
			return true;
		}

		private class BindingBuilder : IModelGenericExecutor, IFieldGenericExecutor
		{
			private readonly IFieldPath<TToModel, TToField> _path;

			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			public BindingBuilder(IFieldPath<TToModel, TToField> path)
			{
				_path = path;
			}

			public void Execute<TModel, TField, TData>(TModel model)
				where TModel : IModel<TField>
				where TField : class, IField
				=> CreateBinding<TData>();

			public void Execute<TField, TData>(IField field) where TField : class, IField
				=> CreateBinding<TData>();

			private void CreateBinding<TData>()
			{
				Binding = new CreateInstanceBinding<TFromModel, TFromField, TToModel, TToField, TData>(_path, TypeFactoryHelper.GetFactory<TData>());
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
		private readonly Func<TData> _factory;

		public CreateInstanceBinding(IFieldPath<TToModel, TToField> path, Func<TData> factory)
		{
			_path = path;
			_factory = factory;
		}
	}
}
