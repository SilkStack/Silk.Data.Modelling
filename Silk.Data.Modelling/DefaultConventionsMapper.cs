using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Data.Modelling
{
	public class DefaultConventionsMapper<T> : IMapper<T>
		 where T : new()
	{
		public Type ModelType { get; }
		public TypedModel<T> Model { get; }

		private readonly Dictionary<Type, TypedDefaultView<T>> _viewCache = new Dictionary<Type, TypedDefaultView<T>>();

		public DefaultConventionsMapper()
		{
			ModelType = typeof(T);
			Model = TypeModeller.GetModelOf<T>();
		}

		private TypedDefaultView<T,TView> GetView<TView>()
		{
			var viewType = typeof(TView);
			TypedDefaultView<T> view;
			if (_viewCache.TryGetValue(viewType, out view))
				return view as TypedDefaultView<T, TView>;

			lock (_viewCache)
			{
				if (_viewCache.TryGetValue(viewType, out view))
					return view as TypedDefaultView<T, TView>;

				view = Model.CreateTypedView<T, TView>();
				_viewCache.Add(viewType, view);
				return view as TypedDefaultView<T, TView>;
			}
		}

		public Task<T> MapAsync<TView>(TView viewInstance) where TView : new()
		{
			return GetView<TView>()
				.MapToModelAsync(viewInstance);
		}

		public Task<TView> MapAsync<TView>(T modelInstance) where TView : new()
		{
			return GetView<TView>()
				.MapToViewAsync(modelInstance);
		}

		public Task MapAsync<TView>(T from, TView onto)
		{
			return GetView<TView>()
				.MapToViewAsync(from, onto);
		}

		public Task MapAsync<TView>(TView from, T onto)
		{
			return GetView<TView>()
				.MapToModelAsync(onto, from);
		}
	}
}
