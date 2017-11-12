using Silk.Data.Modelling.Bindings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling
{
	public static class ViewMappingExtensions
	{
		public static async Task MapToViewAsync(this IView view,
			ModelReadWriter modelReadWriter, ViewReadWriter viewReadWriter)
		{
			var mappingContext = new MappingContext(BindingDirection.ModelToView);
			foreach (var resourceLoader in view.ResourceLoaders)
			{
				await resourceLoader.LoadResourcesAsync(view, new[] { modelReadWriter }, mappingContext)
					.ConfigureAwait(false);
			}

			foreach (var viewField in view.Fields)
			{
				if ((viewField.ModelBinding.Direction & BindingDirection.ModelToView) == BindingDirection.ModelToView)
				{
					viewField.ModelBinding.CopyBindingValue(modelReadWriter, viewReadWriter, mappingContext);
				}
			}
		}

		public static async Task MapToViewAsync(this IView view,
			ICollection<ModelReadWriter> modelReadWriters, ICollection<ViewReadWriter> viewReadWriters)
		{
			var mappingContext = new MappingContext(BindingDirection.ModelToView);
			foreach (var resourceLoader in view.ResourceLoaders)
			{
				await resourceLoader.LoadResourcesAsync(
					view,
					modelReadWriters.OfType<IContainerReadWriter>().ToArray(), mappingContext
					)
					.ConfigureAwait(false);
			}

			using (var sourceEnumerator = modelReadWriters.GetEnumerator())
			using (var destinationEnumerator = viewReadWriters.GetEnumerator())
			{
				while (sourceEnumerator.MoveNext() &&
					destinationEnumerator.MoveNext())
				{
					var modelReadWriter = sourceEnumerator.Current;
					var viewReadWriter = destinationEnumerator.Current;

					foreach (var viewField in view.Fields)
					{
						if ((viewField.ModelBinding.Direction & BindingDirection.ModelToView) == BindingDirection.ModelToView)
						{
							viewField.ModelBinding.CopyBindingValue(modelReadWriter, viewReadWriter, mappingContext);
						}
					}
				}
			}
		}

		public static Task MapToViewAsync<TField, TSource>(this IView<TField, TSource> view,
			TSource sourceObj, ViewReadWriter viewReadWriter)
			where TField : IViewField
		{
			return view.MapToViewAsync(new ObjectModelReadWriter(view.Model, sourceObj), viewReadWriter);
		}

		public static Task MapToViewAsync<TField, TSource>(this IView<TField, TSource> view,
			IEnumerable<TSource> sourceObjs, ICollection<ViewReadWriter> viewReadWriters)
			where TField : IViewField
		{
			return view.MapToViewAsync(
				sourceObjs.Select(obj => new ObjectModelReadWriter(view.Model, obj)).ToArray(),
				viewReadWriters);
		}

		public static Task MapToViewAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			TSource sourceObj, TView viewObj)
			where TField : IViewField
		{
			return view.MapToViewAsync(
				new ObjectModelReadWriter(view.Model, sourceObj),
				new ObjectViewReadWriter(view, viewObj)
				);
		}

		public static Task MapToViewAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			IEnumerable<TSource> sourceObjs, IEnumerable<TView> viewObjs)
			where TField : IViewField
		{
			return view.MapToViewAsync(
				sourceObjs.Select(obj => new ObjectModelReadWriter(view.Model, obj)).ToArray(),
				viewObjs.Select(obj => new ObjectViewReadWriter(view, obj)).ToArray()
				);
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
			ModelReadWriter modelReadWriter, ViewReadWriter viewReadWriter)
		{
			var mappingContext = new MappingContext(BindingDirection.ViewToModel);
			foreach (var resourceLoader in view.ResourceLoaders)
			{
				await resourceLoader.LoadResourcesAsync(view, new[] { viewReadWriter }, mappingContext)
					.ConfigureAwait(false);
			}

			foreach (var viewField in view.Fields)
			{
				if ((viewField.ModelBinding.Direction & BindingDirection.ViewToModel) == BindingDirection.ViewToModel)
				{
					viewField.ModelBinding.CopyBindingValue(viewReadWriter, modelReadWriter, mappingContext);
				}
			}
		}

		public static async Task MapToModelAsync(this IView view,
			ICollection<ModelReadWriter> modelReadWriters, ICollection<ViewReadWriter> viewReadWriters)
		{
			var mappingContext = new MappingContext(BindingDirection.ViewToModel);
			foreach (var resourceLoader in view.ResourceLoaders)
			{
				await resourceLoader.LoadResourcesAsync(
					view,
					viewReadWriters.OfType<IContainerReadWriter>().ToArray(), mappingContext
					)
					.ConfigureAwait(false);
			}

			using (var sourceEnumerator = modelReadWriters.GetEnumerator())
			using (var destinationEnumerator = viewReadWriters.GetEnumerator())
			{
				while (sourceEnumerator.MoveNext() &&
					destinationEnumerator.MoveNext())
				{
					var modelReadWriter = sourceEnumerator.Current;
					var viewReadWriter = destinationEnumerator.Current;

					foreach (var viewField in view.Fields)
					{
						if ((viewField.ModelBinding.Direction & BindingDirection.ViewToModel) == BindingDirection.ViewToModel)
						{
							viewField.ModelBinding.CopyBindingValue(viewReadWriter, modelReadWriter, mappingContext);
						}
					}
				}
			}
		}

		public static Task MapToModelAsync<TField, TSource>(this IView<TField, TSource> view,
			TSource sourceObj, ViewReadWriter viewReadWriter)
			where TField : IViewField
		{
			return view.MapToModelAsync(
				new ObjectModelReadWriter(view.Model, sourceObj),
				viewReadWriter);
		}

		public static Task MapToModelAsync<TField, TSource>(this IView<TField, TSource> view,
			IEnumerable<TSource> sourceObjs, ICollection<ViewReadWriter> viewReadWriters)
			where TField : IViewField
		{
			return view.MapToModelAsync(
				sourceObjs.Select(obj => new ObjectModelReadWriter(view.Model, obj)).ToArray(),
				viewReadWriters);
		}

		public static Task MapToModelAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			TSource sourceObj, TView viewObj)
			where TField : IViewField
		{
			return view.MapToModelAsync(
				new ObjectModelReadWriter(view.Model, sourceObj),
				new ObjectViewReadWriter(view, viewObj)
				);
		}

		public static Task MapToModelAsync<TField, TSource, TView>(this IView<TField, TSource, TView> view,
			IEnumerable<TSource> sourceObjs, IEnumerable<TView> viewObjs)
			where TField : IViewField
		{
			return view.MapToModelAsync(
				sourceObjs.Select(obj => new ObjectModelReadWriter(view.Model, obj)).ToArray(),
				viewObjs.Select(obj => new ObjectViewReadWriter(view, obj)).ToArray()
				);
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
