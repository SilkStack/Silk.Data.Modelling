using Silk.Data.Modelling.Conventions;
using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Extension methods used for producing strongly typed views from models.
	/// </summary>
	public static class ModelTypedViewExtensions
	{
		public static TypedDefaultView<TSource> CreateTypedView<TSource>(this TypedModel<TSource> model, params ViewConvention[] viewConventions)
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model),
				viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView>(this TypedModel<TSource> model,
			params ViewConvention[] viewConventions)
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource, TView>(viewDefinition.Name,
					ViewField.FromDefinitions(viewDefinition.FieldDefinitions), model),
				viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView>(
			this TypedModel<TSource>.Modeller<TView> modeller,
			params ViewConvention[] viewConventions)
		{
			return modeller.Model.CreateTypedView<TSource, TView>(viewConventions);
		}
	}
}
