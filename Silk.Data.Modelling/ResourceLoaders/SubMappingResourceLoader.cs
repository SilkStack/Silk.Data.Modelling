using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.Conventions;
using System;
using System.Collections;
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
		private readonly ViewConvention[] _viewConventions;

		public SubMappingResourceLoader(Model parentModel, ViewConvention[] viewConventions)
		{
			_parentModel = parentModel;
			_viewConventions = viewConventions;
		}

		public void AddField(string fieldName, ModelBinding binding,
			Type modelType, Type viewType)
		{
			var mapping = GetOrCreateMapping(modelType, viewType);
			mapping.Fields.Add(new MappingField(fieldName, binding));
		}

		private Mapping GetOrCreateMapping(Type modelType, Type viewType)
		{
			var mapping = _mappings.FirstOrDefault(q => q.ModelType == modelType &&
				q.ViewType == viewType);
			if (mapping != null)
				return mapping;
			mapping = new Mapping(
				modelType, viewType,
				TypeModeller.GetModelOf(modelType).CreateView(viewType, _viewConventions)
				);
			_mappings.Add(mapping);
			return mapping;
		}

		public async Task LoadResourcesAsync(IView view, ICollection<IContainerReadWriter> sources, MappingContext mappingContext)
		{
			if (sources == null || sources.Count == 0)
				return;

			foreach (var mapping in _mappings)
			{
				var modelReadWriters = new List<ModelReadWriter>();
				var viewReadWriters = new List<ViewReadWriter>();
				var mappingResults = new List<MappingResult>();

				foreach (var source in sources)
				{
					foreach (var field in mapping.Fields)
					{
						var resourceFieldName = string.Join(".", field.Binding.ModelFieldPath);
						var value = field.Binding.ReadValue<object>(source);
						if (value is IEnumerable values)
						{
							foreach (var enumValue in values)
							{
								AddMapping(mapping, mappingContext, enumValue,
									field.Binding, modelReadWriters, viewReadWriters,
									mappingResults, resourceFieldName, source);
							}
						}
						else
						{
							AddMapping(mapping, mappingContext, value,
								field.Binding, modelReadWriters, viewReadWriters,
								mappingResults, resourceFieldName, source);
						}
					}
				}

				if (mappingContext.BindingDirection == BindingDirection.ViewToModel)
					await mapping.View.MapToModelAsync(modelReadWriters, viewReadWriters)
						.ConfigureAwait(false);
				else
					await mapping.View.MapToViewAsync(modelReadWriters, viewReadWriters)
						.ConfigureAwait(false);

				foreach (var mappingResult in mappingResults)
				{
					mappingContext.Resources.Store(
						mappingResult.SourceReadWriter,
						mappingResult.ResourceFieldName,
						mappingResult.ResultInstance
						);
				}
			}
		}

		private void AddMapping(Mapping mapping, MappingContext mappingContext,
			object value, ModelBinding modelBinding,
			List<ModelReadWriter> modelReadWriters, List<ViewReadWriter> viewReadWriters,
			List<MappingResult> mappingResults, string resourceFieldName,
			IContainerReadWriter sourceReadWriter)
		{
			if (mappingContext.BindingDirection == BindingDirection.ViewToModel)
			{
				var instance = mapping.CreateModelInstance();
				mappingResults.Add(new MappingResult(sourceReadWriter, resourceFieldName, instance));
				viewReadWriters.Add(
					new ObjectViewReadWriter(mapping.View, value)
					);
				modelReadWriters.Add(
					new ObjectModelReadWriter(mapping.View.Model, instance)
					);
			}
			else
			{
				var instance = mapping.CreateViewInstance();
				mappingResults.Add(new MappingResult(sourceReadWriter, resourceFieldName, instance));
				modelReadWriters.Add(
					new ObjectModelReadWriter(mapping.View.Model, value)
					);
				viewReadWriters.Add(
					new ObjectViewReadWriter(mapping.View, instance)
					);
			}
		}

		private class MappingResult
		{
			public IContainerReadWriter SourceReadWriter { get; }
			public string ResourceFieldName { get; }
			public object ResultInstance { get; }

			public MappingResult(IContainerReadWriter sourceReadWriter,
				string resourceFieldName,
				object resultInstance)
			{
				SourceReadWriter = sourceReadWriter;
				ResourceFieldName = resourceFieldName;
				ResultInstance = resultInstance;
			}
		}

		private class Mapping
		{
			public Type ModelType { get; }
			public Type ViewType { get; }
			public IView View { get; }
			public List<MappingField> Fields { get; } = new List<MappingField>();

			public Mapping(Type modelType, Type viewType, IView view)
			{
				ModelType = modelType;
				ViewType = viewType;
				View = view;
			}

			public object CreateModelInstance()
			{
				//  todo: stop using activator, use a compiled expression
				return Activator.CreateInstance(ModelType);
			}

			public object CreateViewInstance()
			{
				//  todo: stop using activator, use a compiled expression
				return Activator.CreateInstance(ViewType);
			}
		}

		private class MappingField
		{
			public string FieldName { get; }
			public ModelBinding Binding { get; }

			public MappingField(string fieldName, ModelBinding binding)
			{
				FieldName = fieldName;
				Binding = binding;
			}
		}
	}
}
