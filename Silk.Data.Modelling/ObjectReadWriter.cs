using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Reads/writes to objects.
	/// </summary>
	public class ObjectReadWriter : IModelReadWriter
	{
		public Type Type { get; }
		public Model Model { get; }
		public object Value { get; set; }

		public ObjectReadWriter(Type type, Model model, object value)
		{
			Type = type;
			Model = model;
			Value = value;
		}

		public ObjectReadWriter(TypedModel model, object value)
			: this(model.DataType, model, value)
		{
		}

		public IModelReadWriter GetField(ModelField modelField)
		{
			//  todo: replace reflection with cached compiled expressions
			var property = Type.GetProperty(modelField.Name);
			if (property == null)
				return null;
			return new ObjectField(property.PropertyType, modelField.ParentModel,
				() => property.GetValue(Value), value => property.SetValue(Value, value));
		}

		private class ObjectField : IModelReadWriter
		{
			private readonly Type _type;
			private readonly Func<object> _getter;
			private readonly Action<object> _setter;

			public Model Model { get; }

			public object Value
			{
				get
				{
					return _getter();
				}
				set
				{
					_setter(value);
				}
			}

			public ObjectField(Type type, Model model, Func<object> getter, Action<object> setter)
			{
				_type = type;
				_getter = getter;
				_setter = setter;
				Model = model;
			}

			public IModelReadWriter GetField(ModelField modelField)
			{
				//  todo: replace reflection with cached compiled expressions
				var property = _type.GetProperty(modelField.Name);
				if (property == null)
					return null;
				return new ObjectField(property.PropertyType, modelField.ParentModel,
					() => property.GetValue(Value), value => property.SetValue(Value, value));
			}
		}
	}
}
