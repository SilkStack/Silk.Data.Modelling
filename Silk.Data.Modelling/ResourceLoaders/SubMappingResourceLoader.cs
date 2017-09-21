using Silk.Data.Modelling.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.ResourceLoaders
{
	/// <summary>
	/// Performs sub-mappings before a mapping operation.
	/// </summary>
	public class SubMappingResourceLoader : IResourceLoader
	{
		private readonly List<Mapping> _mappings = new List<Mapping>();
		private readonly Model _parentModel;

		public SubMappingResourceLoader(Model parentModel)
		{
			_parentModel = parentModel;
		}

		public void AddMappedField(string modelFieldName, string viewFieldName, 
			Type modelType, Type viewType)
		{
			var mapping = _mappings.FirstOrDefault(q => q.ModelType == modelType &&
				q.ViewType == viewType);
			if (mapping == null)
				throw new ArgumentException("Mapping for provided types is unknown.");
			mapping.MappingFields.Add(new MappingField(modelFieldName, viewFieldName));
		}

		public bool HasMapping(Type modelType, Type viewType)
		{
			return _mappings.Any(q => q.ModelType == modelType && q.ViewType == viewType);
		}

		public void AddMapping(TypedModel model, Type viewType, params ViewConvention[] viewConventions)
		{
			var modelType = model.DataType;
			var view = model.CreateView(viewType, viewConventions);
			_mappings.Add(new Mapping(model, view, viewType));
		}

		public async Task LoadResourcesAsync(ICollection<IContainer> containers, MappingContext mappingContext)
		{
			foreach (var mapping in _mappings)
			{
				var mappedResourceList = new List<MappedResource>();
				foreach (var mappingField in mapping.MappingFields)
				{
					var modelField = _parentModel.Fields
						.First(q => q.Name == mappingField.ModelFieldName);
					foreach (var container in containers)
					{
						var subContainer = mapping.CreateViewContainer(container.GetValue(new[] { mappingField.ViewFieldName }));
						var readWriterTuple = mapping.CreateModelReaderAndInstance();
						mappedResourceList.Add(new MappedResource(container, readWriterTuple.readWriter,
							subContainer, mappingField, readWriterTuple.instance
							));
					}
				}
				await mapping.View
					.MapToModelAsync(
						mappedResourceList.Select(q => q.ModelReadWriter).ToArray(),
						mappedResourceList.Select(q => q.ViewContainer).ToArray())
					.ConfigureAwait(false);
				foreach (var mappedResource in mappedResourceList)
				{
					mappingContext.Resources.Store(
						mappedResource.StoreForObject,
						$"subMapped:{mappedResource.MappingField.ModelFieldName}",
						mappedResource.MappingResult);
				}
			}
		}

		public async Task LoadResourcesAsync(ICollection<IModelReadWriter> modelReadWriters, MappingContext mappingContext)
		{
			foreach (var mapping in _mappings)
			{
				var mappedResourceList = new List<MappedResource>();
				foreach (var mappingField in mapping.MappingFields)
				{
					var modelField = _parentModel.Fields
						.First(q => q.Name == mappingField.ModelFieldName);
					foreach (var modelReaderWriter in modelReadWriters)
					{
						var containerTuple = mapping.CreateViewContainerAndInstance();
						var readWriter = mapping.CreateModelReader();
						readWriter.Value = modelReaderWriter.GetField(modelField).Value;
						mappedResourceList.Add(new MappedResource(
							modelReaderWriter, readWriter,
							containerTuple.container, mappingField, containerTuple.instance
							));
					}
				}
				await mapping.View
					.MapToViewAsync(
						mappedResourceList.Select(q => q.ModelReadWriter).ToArray(),
						mappedResourceList.Select(q => q.ViewContainer).ToArray())
					.ConfigureAwait(false);
				foreach (var mappedResource in mappedResourceList)
				{
					mappingContext.Resources.Store(
						mappedResource.StoreForObject,
						$"subMapped:{mappedResource.MappingField.ModelFieldName}",
						mappedResource.MappingResult);
				}
			}
		}

		private class MappedResource
		{
			public object StoreForObject { get; }
			public IModelReadWriter ModelReadWriter { get; }
			public IContainer ViewContainer { get; }
			public MappingField MappingField { get; }
			public object MappingResult { get; }

			public MappedResource(object storeForObject,
				IModelReadWriter modelReadWriter, IContainer viewContainer,
				MappingField mappingField, object mappingResult)
			{
				StoreForObject = storeForObject;
				ModelReadWriter = modelReadWriter;
				ViewContainer = viewContainer;
				MappingField = mappingField;
				MappingResult = mappingResult;
			}
		}

		private class Mapping
		{
			public IView View { get; }
			public TypedModel Model { get; }
			public Type ViewType { get; }
			public Type ModelType => Model.DataType;
			public Func<object> ViewFactory { get; }
			public Func<object> ModelFactory { get; }
			public Func<IModelReadWriter> CreateModelReader { get; }
			public Func<object, IContainer> CreateViewContainer { get; }
			public Func<(IContainer container,object instance)> CreateViewContainerAndInstance { get; }
			public Func<(IModelReadWriter readWriter, object instance)> CreateModelReaderAndInstance { get; }
			public List<MappingField> MappingFields { get; } = new List<MappingField>();

			public Mapping(TypedModel model, IView view, Type viewType)
			{
				View = view;
				Model = model;
				ViewType = viewType;
				ViewFactory = CreateFactory(viewType);
				ModelFactory = CreateFactory(model.DataType);

				CreateModelReader = () => new ObjectReadWriter(Model, null);

				//  todo: replace reflection with cached expressions
				var containerType = typeof(ObjectContainer<>).MakeGenericType(ViewType);
				var instanceProperty = containerType.GetProperty("Instance");
				CreateViewContainer = obj =>
				{
					var container = Activator.CreateInstance(containerType, new object[] { Model, View }) as IContainer;
					instanceProperty.SetValue(container, obj);
					return container;
				};
				CreateViewContainerAndInstance = () =>
				{
					var container = Activator.CreateInstance(containerType, new object[] { Model, View }) as IContainer;
					var instance = ViewFactory();
					instanceProperty.SetValue(container, instance);

					return (container, instance);
				};
				CreateModelReaderAndInstance = () =>
				{
					var modelInstance = ModelFactory();
					var modelReader = new ObjectReadWriter(Model, modelInstance);
					return (modelReader, modelInstance);
				};
			}

			private Func<object> CreateFactory(Type type)
			{
				//  todo: replace reflection with cached compiled expressions?
				if (type.GetConstructors().Any(q => q.GetParameters().Length == 0))
				{
					return () => Activator.CreateInstance(type);
				}
				return () => null;
			}
		}

		private class MappingField
		{
			public string ModelFieldName { get; }
			public string ViewFieldName { get; }

			public MappingField(string modelFieldName, string viewFieldName)
			{
				ModelFieldName = modelFieldName;
				ViewFieldName = viewFieldName;
			}
		}
	}
}
