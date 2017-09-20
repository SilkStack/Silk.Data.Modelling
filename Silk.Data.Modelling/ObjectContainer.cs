using System;
using System.Linq;

namespace Silk.Data.Modelling
{
	public class ObjectContainer<T> : IContainer
	{
		public TypedModel Model { get; }

		public IView View { get; }

		public T Instance { get; set; }

		public Type DataType { get; } = typeof(T);

		public ObjectContainer(TypedModel model, IView view)
		{
			Model = model;
			View = view;
		}

		public void SetValue(string[] fieldPath, object value)
		{
			//  todo: replace reflection with cached expressions
			//  todo: how to support field paths with more than 1 element?
			var property = DataType.GetProperty(fieldPath[0]);
			if (property == null)
				throw new InvalidOperationException("Field cannot be set on view.");
			property.SetValue(Instance, value);
		}

		public object GetValue(string[] fieldPath)
		{
			//  todo: replace reflection with cached expressions
			//  todo: how to support field paths with more than 1 element?
			var property = DataType.GetProperty(fieldPath[0]);
			if (property == null)
				throw new InvalidOperationException("Field cannot be got on view.");
			return property.GetValue(Instance);
		}
	}
}
