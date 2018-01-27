using System;
using System.Threading.Tasks;

namespace Silk.Data.Modelling
{
	public interface IMapper<T> where T : new()
	{
		Type ModelType { get; }
		TypedModel<T> Model { get; }

		Task<T> MapAsync<TView>(TView viewInstance)
			where TView : new();
		Task<TView> MapAsync<TView>(T modelInstance)
			where TView : new();
		Task MapAsync<TView>(T from, TView onto);
		Task MapAsync<TView>(TView from, T onto);
	}
}
