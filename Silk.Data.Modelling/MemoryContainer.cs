using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Stores a graph in memory.
	/// </summary>
	/// <remarks>Really designed as an example container.</remarks>
	public class MemoryContainer : IContainer
	{
		public TypedModel Model { get; }

		public IView View { get; }

		public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

		public MemoryContainer(TypedModel model, IView view)
		{
			Model = model;
			View = view;
		}

		public void SetValue(string[] fieldPath, object value)
		{
			Data[string.Join(".", fieldPath)] = value;
		}

		public object GetValue(string[] fieldPath)
		{
			Data.TryGetValue(string.Join(".", fieldPath), out var value);
			return value;
		}
	}
}
