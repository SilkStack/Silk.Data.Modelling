using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Silk.Data.Modelling
{
	public class ObjectModelReadWriter : ModelReadWriter
	{
		public ObjectModelReadWriter(Model model, object modelInstance)
			: base(model)
		{
			ModelType = modelInstance.GetType();
			Instance = modelInstance;
		}

		public Type ModelType { get; }
		public object Instance { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			//  todo: replace reflection with cached expressions
			if (Instance == null)
				return default(T);

			object ret = null;
			var dataType = ModelType;
			foreach (var pathComponent in path)
			{
				var property = dataType.GetProperty(pathComponent);
				if (property == null)
					throw new InvalidOperationException($"Field cannot be retrieved on view: {string.Join(".", path)} ({pathComponent}).");
				ret = property.GetValue(ret ?? Instance);
				if (ret == null)
					break;
				dataType = ret.GetType();
			}
			return (T)ret;
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			//  note: writing to properties deep in the graph isn't supported here for now
			//        that functionality is handled by the submapping resource loader and binding
			//  todo: replace reflection with cached expressions
			var property = ModelType.GetProperty(path[0]);
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
			ViewType = viewInstance.GetType();
			Instance = viewInstance;
		}

		public Type ViewType { get; }
		public object Instance { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			//  todo: replace reflection with cached expressions
			if (Instance == null)
				return default(T);

			object ret = null;
			var dataType = ViewType;
			foreach (var pathComponent in path)
			{
				var property = dataType.GetProperty(pathComponent);
				if (property == null)
					throw new InvalidOperationException($"Field cannot be retrieved on view: {string.Join(".", path)} ({pathComponent}).");
				ret = property.GetValue(ret ?? Instance);
				if (ret == null)
					break;
				dataType = ret.GetType();
			}
			return (T)ret;
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			//  note: writing to properties deep in the graph isn't supported here for now
			//        that functionality is handled by the submapping resource loader and binding
			//  todo: replace reflection with cached expressions
			var property = ViewType.GetProperty(path[0]);
			if (property == null)
				throw new InvalidOperationException($"Field cannot be assigned on view: {string.Join(".", path)} ({path[0]}).");
			property.SetValue(Instance, value);
		}
	}
}
