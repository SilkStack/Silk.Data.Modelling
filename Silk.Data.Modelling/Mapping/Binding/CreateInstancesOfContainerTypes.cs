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
		public void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields)
		{
			if (!intersectedFields.RightPath.HasParent || !intersectedFields.RightField.CanWrite ||
				intersectedFields.RightPath.Parent.FinalField == null ||
				mappingFactoryContext.IsToFieldBound(intersectedFields))
			{
				return;
			}

			var parentPath = intersectedFields.RightPath.Parent;
			var bindingBuilder = new BindingBuilder(parentPath);

			parentPath.FinalField.Dispatch(bindingBuilder);

			mappingFactoryContext.Bindings.Add(bindingBuilder.Binding);
		}

		private class BindingBuilder : IFieldGenericExecutor
		{
			private readonly IFieldPath<TToModel, TToField> _path;

			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			public BindingBuilder(IFieldPath<TToModel, TToField> path)
			{
				_path = path;
			}

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

		public TToField ToField => _path.FinalField;

		public TFromField FromField => null;

		public CreateInstanceBinding(IFieldPath<TToModel, TToField> path, Func<TData> factory)
		{
			_path = path;
			_factory = factory;
		}

		public void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			var destinationReader = destination as IGraphReader<TToModel, TToField>;
			if (destinationReader != null)
			{
				var currentValue = destinationReader.Read<TData>(_path);
				if (currentValue != null)
					return;
			}

			destination.Write(_path, _factory());
		}
	}
}
