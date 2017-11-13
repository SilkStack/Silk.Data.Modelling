using System.Collections.Generic;

namespace Silk.Data.Modelling
{
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
			Store.TryGetValue(string.Join(".", path), out var value);
			if (value == null)
				return default(T);
			return (T)value;
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			Store[string.Join(".", path)] = value;
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
			Store.TryGetValue(string.Join(".", path), out var value);
			if (value == null)
				return default(T);
			return (T)value;
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			Store[string.Join(".", path)] = value;
		}
	}
}
