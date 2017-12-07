using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling
{
	public class ObjectModelReadWriter : ModelReadWriter
	{
		public ObjectModelReadWriter(Model model, object modelInstance)
			: base(model)
		{
			(ModelType, EnumType) = modelInstance.GetType()
				.GetDataAndEnumerableType();
			Instance = modelInstance;
		}

		public Type ModelType { get; }
		public Type EnumType { get; }
		public object Instance { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			//  todo: replace reflection with cached expressions
			if (Instance == null)
				return default(T);

			object ret = null;
			var dataType = ModelType;
			var enumType = EnumType;
			foreach (var pathComponent in path)
			{
				var property = dataType.GetTypeInfo().GetDeclaredProperty(pathComponent);
				if (property == null)
					throw new InvalidOperationException($"Field cannot be retrieved on view: {string.Join(".", path)} ({pathComponent}).");
				if (enumType == null)
					ret = property.GetValue(ret ?? Instance);
				else
					ret = ((IEnumerable)(ret ?? Instance)).OfType<object>().Select(q => property.GetValue(q));
				if (ret == null)
					break;
				(dataType, enumType) = ret.GetType().GetDataAndEnumerableType();
			}
			return (T)ret;
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			//  todo: replace reflection with cached expressions
			var objType = ModelType;
			var objInstance = Instance;
			for (var i = 0; i < path.Length; i++)
			{
				var property = objType.GetTypeInfo().GetDeclaredProperty(path[i]);
				if (property == null)
					throw new InvalidOperationException($"Field cannot be assigned on view: {string.Join(".", path)} ({path[0]}).");

				if (i == path.Length - 1)
				{
					property.SetValue(objInstance, value);
				}
				else
				{
					objType = property.PropertyType;
					var subObjInstance = property.GetValue(objInstance);
					if (subObjInstance == null)
					{
						subObjInstance = Activator.CreateInstance(objType);
						property.SetValue(objInstance, subObjInstance);
					}
					objInstance = subObjInstance;
				}
			}
		}
	}

	public class ObjectViewReadWriter : ViewReadWriter
	{
		public ObjectViewReadWriter(IView view, object viewInstance)
			: base(view)
		{
			(ViewType, EnumType) = viewInstance.GetType()
				.GetDataAndEnumerableType();
			Instance = viewInstance;
		}

		public Type ViewType { get; }
		public Type EnumType { get; }
		public object Instance { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			//  todo: replace reflection with cached expressions
			if (Instance == null)
				return default(T);

			object ret = null;
			var dataType = ViewType;
			var enumType = EnumType;
			foreach (var pathComponent in path)
			{
				var property = dataType.GetTypeInfo().GetDeclaredProperty(pathComponent);
				if (property == null)
					throw new InvalidOperationException($"Field cannot be retrieved on view: {string.Join(".", path)} ({pathComponent}).");
				if (enumType == null)
					ret = property.GetValue(ret ?? Instance);
				else
					ret = ((IEnumerable)(ret ?? Instance)).OfType<object>().Select(q => property.GetValue(q));
				if (ret == null)
					break;
				(dataType, enumType) = ret.GetType().GetDataAndEnumerableType();
			}
			return (T)ret;
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			//  todo: replace reflection with cached expressions
			var objType = ViewType;
			var objInstance = Instance;
			for (var i = 0; i < path.Length; i++)
			{
				var property = objType.GetTypeInfo().GetDeclaredProperty(path[i]);
				if (property == null)
					throw new InvalidOperationException($"Field cannot be assigned on view: {string.Join(".", path)} ({path[0]}).");

				if (i == path.Length - 1)
				{
					property.SetValue(objInstance, value);
				}
				else
				{
					objType = property.PropertyType;
					var subObjInstance = property.GetValue(objInstance);
					if (subObjInstance == null)
					{
						subObjInstance = Activator.CreateInstance(objType);
						property.SetValue(objInstance, subObjInstance);
					}
					objInstance = subObjInstance;
				}
			}
		}
	}
}
