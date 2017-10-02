using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A collection of resource objects.
	/// </summary>
	public class ResourceObjectCollection
	{
		private readonly Dictionary<string, object> _resourceObjects = new Dictionary<string, object>();
		private readonly ConditionalWeakTable<object, Dictionary<string, object>> _objectResourceObjects = new ConditionalWeakTable<object, Dictionary<string, object>>();

		public void Store(string key, object value)
		{
			_resourceObjects[key] = value;
		}

		public void Store(object forObject, string key, object value)
		{
			if (!_objectResourceObjects.TryGetValue(forObject, out var resourceObjects))
			{
				resourceObjects = new Dictionary<string, object>();
				_objectResourceObjects.Add(forObject, resourceObjects);
			}
			if (resourceObjects.TryGetValue(key, out var existingObject))
			{
				if (existingObject is List<object> list)
				{
					list.Add(value);
				}
				else
				{
					list = new List<object>();
					list.Add(existingObject);
					list.Add(value);
					resourceObjects[key] = list;
				}
			}
			else
			{
				resourceObjects[key] = value;
			}
		}

		public object Retrieve(string key)
		{
			_resourceObjects.TryGetValue(key, out var value);
			return value;
		}

		public object Retrieve(object forObject, string key)
		{
			if (!_objectResourceObjects.TryGetValue(forObject, out var resourceObjects))
				return null;
			resourceObjects.TryGetValue(key, out var value);
			return value;
		}
	}
}
