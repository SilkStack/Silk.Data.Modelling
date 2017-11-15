using Silk.Data.Modelling.Conventions;
using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Extension methods used for producing strongly typed views from models.
	/// </summary>
	public static class ModelTypedViewExtensions
	{
		public static TypedDefaultView<TSource> CreateTypedView<TSource>(this TypedModel<TSource> model, params ViewConvention<ViewBuilder>[] viewConventions)
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model, viewDefinition.ResourceLoaders),
				viewConventions);
		}

		public static TypedDefaultView<TSource> CreateTypedView<TSource>(this TypedModel<TSource> model,
			Type viewType, params ViewConvention<ViewBuilder>[] viewConventions)
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model, viewDefinition.ResourceLoaders),
				viewType, viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView>(this TypedModel<TSource> model,
			params ViewConvention<ViewBuilder>[] viewConventions)
		{
			return model.CreateView<TypedDefaultView<TSource, TView>, TView>(viewDefinition => new TypedDefaultView<TSource, TView>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model, viewDefinition.ResourceLoaders),
				viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView>(
			this TypedModel<TSource>.Modeller<TView> modeller,
			params ViewConvention<ViewBuilder>[] viewConventions)
		{
			return modeller.Model.CreateTypedView<TSource, TView>(viewConventions);
		}

		public static TypedDefaultView<TSource> CreateTypedView<TSource, TBuilder>(this TypedModel<TSource> model, params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model, viewDefinition.ResourceLoaders),
				viewConventions);
		}

		public static TypedDefaultView<TSource> CreateTypedView<TSource, TBuilder>(this TypedModel<TSource> model,
			Type viewType, params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model, viewDefinition.ResourceLoaders),
				viewType, viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView, TBuilder>(this TypedModel<TSource> model,
			params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			return model.CreateView<TypedDefaultView<TSource, TView>, TView, TBuilder>(viewDefinition => new TypedDefaultView<TSource, TView>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model, viewDefinition.ResourceLoaders),
				viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView, TBuilder>(
			this TypedModel<TSource>.Modeller<TView> modeller,
			params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			return modeller.Model.CreateTypedView<TSource, TView, TBuilder>(viewConventions);
		}
	}
}
