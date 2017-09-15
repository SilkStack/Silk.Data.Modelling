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
					var value = await viewField.ModelBinding
						.ReadFromModelAsync(modelReadWriter)
						.ConfigureAwait(false);
					viewContainer.SetValue(viewField, value);
				}
			}
		}
	}
}
