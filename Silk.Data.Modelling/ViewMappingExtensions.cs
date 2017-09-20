using Silk.Data.Modelling.Bindings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling
{
	public static class ViewMappingExtensions
	{
		public static async Task MapToViewAsync(this IView view,
			IModelReadWriter modelReadWriter, IContainer viewContainer)
		{
			var mappingContext = new MappingContext(BindingDirection.ModelToView);
			foreach (var resouceLoader in view.ResourceLoaders)
			{
				await resouceLoader.LoadResourcesAsync(new[] { modelReadWriter }, mappingContext)
					.ConfigureAwait(false);
			}
			foreach (var viewField in view.Fields)
			{
				if ((viewField.ModelBinding.Direction & BindingDirection.ModelToView) == BindingDirection.ModelToView)
				{
					viewField.ModelBinding.WriteToContainer(viewContainer,
						viewField.ModelBinding.ReadFromModel(modelReadWriter, mappingContext),
						mappingContext);
				}
			}
		}

		public static Task MapToViewAsync<TField, TSource>(this IView<TField, TSource> view,
			TSource obj, IContainer viewContainer)
			where TField : IViewField
		{
			return view.MapToViewAsync(new ObjectReadWriter(view.Model, obj), viewContainer);
		}

		public static Task MapToViewAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			TSource sourceObj, TView viewObj)
			where TField : IViewField
		{
			var modelReaderWriter = new ObjectReadWriter(view.Model, sourceObj);
			var container = new ObjectContainer<TView>(view.Model, view)
			{
				Instance = viewObj
			};
			return view.MapToViewAsync(modelReaderWriter, container);
		}

		public static async Task<TView> MapToViewAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			TSource sourceObj)
			where TField : IViewField
			where TView : new()
		{
			var viewObj = new TView();
			await view.MapToViewAsync(sourceObj, viewObj)
				.ConfigureAwait(false);
			return viewObj;
		}

		public static async Task MapToViewAsync(this IView view,
			IEnumerable<IModelReadWriter> modelReadWriters, ICollection<IContainer> viewContainers)
		{
			var modelReadWritersArray = modelReadWriters.ToArray();
			var readWriterEnum = modelReadWritersArray.GetEnumerator();
			using (var containerEnum = viewContainers.GetEnumerator())
			{
				var mappingContext = new MappingContext(BindingDirection.ModelToView);

				foreach (var resouceLoader in view.ResourceLoaders)
				{
					await resouceLoader.LoadResourcesAsync(modelReadWritersArray, mappingContext)
						.ConfigureAwait(false);
				}

				while (readWriterEnum.MoveNext() &&
					containerEnum.MoveNext())
				{
					var modelReadWriter = readWriterEnum.Current as IModelReadWriter;
					var viewContainer = containerEnum.Current;
					foreach (var viewField in view.Fields)
					{
						if ((viewField.ModelBinding.Direction & BindingDirection.ModelToView) == BindingDirection.ModelToView)
						{
							viewField.ModelBinding.WriteToContainer(viewContainer,
								viewField.ModelBinding.ReadFromModel(modelReadWriter, mappingContext),
								mappingContext);
						}
					}
				}
			}
		}

		public static Task MapToViewAsync<TField, TSource>(this IView<TField, TSource> view,
			IEnumerable<TSource> objs, ICollection<IContainer> viewContainers)
			where TField : IViewField
		{
			return view.MapToViewAsync(objs.Select(q => new ObjectReadWriter(view.Model, q)), viewContainers);
		}

		public static Task MapToViewAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			IEnumerable<TSource> sourceObjs, IEnumerable<TView> viewObjs)
			where TField : IViewField
		{
			return view.MapToViewAsync(sourceObjs.Select(q => new ObjectReadWriter(view.Model, q)),
				viewObjs.Select(q => new ObjectContainer<TView>(view.Model, view) { Instance = q }).ToArray());
		}

		public static async Task<TView[]> MapToViewAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			ICollection<TSource> sourceObjs)
			where TField : IViewField
			where TView : new()
		{
			var viewObjs = new TView[sourceObjs.Count];
			for (var i = 0; i < viewObjs.Length; i++)
				viewObjs[i] = new TView();

			await view.MapToViewAsync(sourceObjs, viewObjs)
				.ConfigureAwait(false);
			return viewObjs;
		}

		public static async Task MapToModelAsync(this IView view,
			IModelReadWriter modelReadWriter, IContainer viewContainer)
		{
			var containerArray = new IContainer[] { viewContainer };
			var mappingContext = new MappingContext(BindingDirection.ViewToModel);
			foreach (var resouceLoader in view.ResourceLoaders)
			{
				await resouceLoader.LoadResourcesAsync(containerArray, mappingContext)
					.ConfigureAwait(false);
			}
			foreach (var viewField in view.Fields)
			{
				if ((viewField.ModelBinding.Direction & BindingDirection.ViewToModel) == BindingDirection.ViewToModel)
				{
					viewField.ModelBinding.WriteToModel(modelReadWriter,
						viewField.ModelBinding.ReadFromContainer(viewContainer, mappingContext),
						mappingContext);
				}
			}
		}

		public static Task MapToModelAsync<TField, TSource>(this IView<TField, TSource> view,
			TSource obj, IContainer viewContainer)
			where TField : IViewField
		{
			var modelReadWriter = new ObjectReadWriter(view.Model, obj);
			return view.MapToModelAsync(modelReadWriter, viewContainer);
		}

		public static Task MapToModelAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			TSource sourceObj, TView viewObj)
			where TField : IViewField
		{
			var modelReaderWriter = new ObjectReadWriter(view.Model, sourceObj);
			var container = new ObjectContainer<TView>(view.Model, view)
			{
				Instance = viewObj
			};
			return view.MapToModelAsync(modelReaderWriter, container);
		}

		public static async Task<TSource> MapToModelAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			TView viewObj)
			where TField : IViewField
			where TSource : new()
		{
			var sourceObj = new TSource();
			await view.MapToModelAsync(sourceObj, viewObj)
				.ConfigureAwait(false);
			return sourceObj;
		}

		public static async Task MapToModelAsync(this IView view,
			IEnumerable<IModelReadWriter> modelReadWriters, ICollection<IContainer> viewContainers)
		{
			var mappingContext = new MappingContext(BindingDirection.ViewToModel);
			foreach (var resouceLoader in view.ResourceLoaders)
			{
				await resouceLoader.LoadResourcesAsync(viewContainers, mappingContext)
					.ConfigureAwait(false);
			}
			using (var readWriterEnum = modelReadWriters.GetEnumerator())
			using (var containerEnum = viewContainers.GetEnumerator())
			{
				while (readWriterEnum.MoveNext() &&
					containerEnum.MoveNext())
				{
					foreach (var viewField in view.Fields)
					{
						if ((viewField.ModelBinding.Direction & BindingDirection.ViewToModel) == BindingDirection.ViewToModel)
						{
							viewField.ModelBinding.WriteToModel(readWriterEnum.Current,
								viewField.ModelBinding.ReadFromContainer(containerEnum.Current, mappingContext),
								mappingContext);
						}
					}
				}
			}
		}

		public static Task MapToModelAsync<TField, TSource>(this IView<TField, TSource> view,
			IEnumerable<TSource> objs, ICollection<IContainer> viewContainers)
			where TField : IViewField
		{
			return view.MapToModelAsync(objs.Select(q => new ObjectReadWriter(view.Model, q)), viewContainers);
		}

		public static Task MapToModelAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			IEnumerable<TSource> sourceObjs, IEnumerable<TView> viewObjs)
			where TField : IViewField
		{
			return view.MapToModelAsync(
				sourceObjs.Select(q => new ObjectReadWriter(view.Model, q)),
				viewObjs.Select(q => new ObjectContainer<TView>(view.Model, view) { Instance = q }).ToArray()
				);
		}

		public static async Task<TSource[]> MapToModelAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			ICollection<TView> viewObjs)
			where TField : IViewField
			where TSource : new()
		{
			var sourceObjs = new TSource[viewObjs.Count];
			for (var i = 0; i < sourceObjs.Length; i++)
				sourceObjs[i] = new TSource();
			await view.MapToModelAsync(sourceObjs, viewObjs)
				.ConfigureAwait(false);
			return sourceObjs;
		}
	}
}
