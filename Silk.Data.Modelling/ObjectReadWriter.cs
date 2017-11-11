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
			throw new NotImplementedException();
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			throw new NotImplementedException();
		}
	}

	public class ObjectViewReadWriter : ViewReadWriter
	{
		public ObjectViewReadWriter(IView view, object viewInstance)
			: base(view)
		{
			ViewType = viewInstance.GetType();
			ViewInstance = viewInstance;
		}

		public Type ViewType { get; }
		public object ViewInstance { get; }

		public override T ReadFromPath<T>(string[] path)
		{
			throw new NotImplementedException();
		}

		public override void WriteToPath<T>(string[] path, T value)
		{
			throw new NotImplementedException();
		}
	}

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
			if (Value == null)
				return null;
			//  todo: replace reflection with cached compiled expressions
			var property = Type.GetProperty(modelField.Name);
			if (property == null)
				return null;
			if (modelField is TypedModelField typedField &&
				typedField.IsEnumerable)
			{
				return new ObjectField(typedField.EnumerableType, typedField.DataTypeModel,
					() => property.GetValue(Value), value => property.SetValue(Value, value));
			}
			return new ObjectField(property.PropertyType, modelField.ParentModel,
				() => {
					return property.GetValue(Value);
					}, value => property.SetValue(Value, value));
		}

		private class ObjectField : IModelReadWriter
		{
			private readonly Type _dataType;
			private readonly Type _enumType;
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
				(_dataType, _enumType) = type.GetDataAndEnumerableType();
				_getter = getter;
				_setter = setter;
				Model = model;
			}

			public IModelReadWriter GetField(ModelField modelField)
			{
				if (Value == null)
					return null;
				
				//  todo: replace reflection with cached compiled expressions
				var property = _dataType.GetProperty(modelField.Name);
				if (property == null)
					return null;
				if (_enumType != null)
				{
					return new ObjectField(property.PropertyType, modelField.ParentModel,
						() => GetEnumProperty(property, Value as IEnumerable), value => { });
				}
				return new ObjectField(property.PropertyType, modelField.ParentModel,
					() => property.GetValue(Value), value => property.SetValue(Value, value));
			}

			private static IEnumerable<object> GetEnumProperty(PropertyInfo property, IEnumerable sourceEnum)
			{
				foreach (var value in sourceEnum)
				{
					yield return property.GetValue(value);
				}
			}
		}
	}
}
