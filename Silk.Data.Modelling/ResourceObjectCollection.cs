using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A collection of resource objects.
	/// </summary>
	public class ResourceObjectCollection
	{
		private readonly Dictionary<string, object> _resourceObjects = new Dictionary<string, object>();

		public void Store(string key, object value)
		{
			_resourceObjects[key] = value;
		}

		public object Retrieve(string key)
		{
			_resourceObjects.TryGetValue(key, out var value);
			return value;
		}
	}
}
