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
					GetDefaultViewFields(viewDefinition.FieldDefinitions), model),
				viewConventions);
		}

		public static TypedDefaultView<TSource, TView> CreateTypedView<TSource, TView>(this TypedModel<TSource> model,
			params ViewConvention[] viewConventions)
		{
			return model.CreateView(viewDefinition => new TypedDefaultView<TSource, TView>(viewDefinition.Name,
					GetDefaultViewFields(viewDefinition.FieldDefinitions), model),
				viewConventions);
		}

		private static IEnumerable<ViewField> GetDefaultViewFields(IEnumerable<ViewFieldDefinition> fieldDefinitions)
		{
			foreach (var viewDefinition in fieldDefinitions)
			{
				yield return new ViewField(viewDefinition.Name, viewDefinition.DataType,
					viewDefinition.Metadata, viewDefinition.Binding);
			}
		}
	}
}
