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

	public class MemoryModelReadWriter : ModelReadWriter
	{
		public MemoryModelReadWriter(Model model, Dictionary<string,object> store = null)
			: base(model)
		{
			if (store != null)
				Store = store;
			else
				Store = new Dictionary<string, object>();
		}

		public Dictionary<string, object> Store { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			throw new System.NotImplementedException();
		}
	}

	public class MemoryViewReadWriter : ViewReadWriter
	{
		public MemoryViewReadWriter(IView view, Dictionary<string, object> store = null)
			: base(view)
		{
			if (store != null)
				Store = store;
			else
				Store = new Dictionary<string, object>();
		}

		public Dictionary<string, object> Store { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			throw new System.NotImplementedException();
		}
	}
}
