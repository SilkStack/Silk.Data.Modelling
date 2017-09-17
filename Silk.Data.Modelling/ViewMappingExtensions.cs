using Silk.Data.Modelling.Bindings;
using System.Threading.Tasks;

namespace Silk.Data.Modelling
{
	public static class ViewMappingExtensions
	{
		public static async Task MapToViewAsync<TField>(this IView<TField> view,
			IModelReadWriter modelReadWriter, IContainer viewContainer)
			where TField : IViewField
		{
			foreach (var viewField in view.Fields)
			{
				if ((viewField.ModelBinding.Direction & BindingDirection.ModelToView) == BindingDirection.ModelToView)
				{
					viewContainer.SetValue(viewField, viewField.ModelBinding
						.ReadFromModel(modelReadWriter));
				}
			}
		}

		public static Task MapToViewAsync<TField, TSource>(this IView<TField, TSource> view,
			TSource obj, IContainer viewContainer)
			where TField : IViewField
		{
			var modelReadWriter = new ObjectReadWriter(view.Model, obj);
			return view.MapToViewAsync(modelReadWriter, viewContainer);
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

		public static async Task MapToModelAsync<TField>(this IView<TField> view,
			IModelReadWriter modelReadWriter, IContainer viewContainer)
			where TField : IViewField
		{
			var containerArray = new IContainer[] { viewContainer };
			var mappingContext = new MappingContext();
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
						viewContainer.GetValue(viewField), mappingContext);
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
	}
}
