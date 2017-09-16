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

		public void SetValue(IViewField field, object value)
		{
			//  todo: replace reflection with cached expressions
			if (!View.Fields.Contains(field))
				throw new InvalidOperationException("Field does not exist on view.");
			var property = DataType.GetProperty(field.Name);
			if (property == null)
				throw new InvalidOperationException("Field cannot be set on view.");
			property.SetValue(Instance, value);
		}
	}
}
