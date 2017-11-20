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
			//  note: writing to properties deep in the graph isn't supported here for now
			//        that functionality is handled by the submapping resource loader and binding
			//  todo: replace reflection with cached expressions
			var property = ModelType.GetTypeInfo().GetDeclaredProperty(path[0]);
			if (property == null)
				throw new InvalidOperationException($"Field cannot be assigned on view: {string.Join(".", path)} ({path[0]}).");
			property.SetValue(Instance, value);
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
			//  note: writing to properties deep in the graph isn't supported here for now
			//        that functionality is handled by the submapping resource loader and binding
			//  todo: replace reflection with cached expressions
			var property = ViewType.GetTypeInfo().GetDeclaredProperty(path[0]);
			if (property == null)
				throw new InvalidOperationException($"Field cannot be assigned on view: {string.Join(".", path)} ({path[0]}).");
			property.SetValue(Instance, value);
		}
	}
}
