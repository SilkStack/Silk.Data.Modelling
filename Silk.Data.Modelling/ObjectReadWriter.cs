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
		public object Instance { get; set; }
		public object Value => Instance;

		public ObjectReadWriter(Type type, Model model)
		{
			Type = type;
			Model = model;
		}

		public ObjectReadWriter(TypedModel model)
			: this(model.DataType, model)
		{
		}

		public IModelReadWriter GetField(ModelField modelField)
		{
			//  todo: replace reflection with cached compiled expressions
			var property = Type.GetProperty(modelField.Name);
			if (property == null)
				return null;
			return new ObjectReadWriter(property.PropertyType, modelField.ParentModel)
			{
				Instance = property.GetValue(Instance)
			};
		}
	}
}
