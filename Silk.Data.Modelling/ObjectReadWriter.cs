using System;
using System.Linq;

namespace Silk.Data.Modelling
{
	public class ObjectReadWriter : IModelReadWriter
	{
		public IModel Model { get; }
		private object _instance;
		private ObjectReadWriteHelpers _readWriteMethods;

		public ObjectReadWriter(object instance, IModel model, Type objectType)
		{
			_instance = instance;
			Model = model;
			_readWriteMethods = ObjectReadWriteHelpers.GetForType(objectType);
		}

		public T ReadField<T>(Span<string> path)
		{
			if (path[0] == ".")
				return (T)_instance;

			if (_instance == null)
				return default(T);

			var firstFieldName = path[0];
			var field = Model.Fields.FirstOrDefault(q => q.FieldName == firstFieldName);
			if (field == null)
				throw new Exception("Unknown field on model.");

			if (path.Length == 1)
				return _readWriteMethods.GetTypedValue<T>(_instance, field.FieldName);

			var subObject = _readWriteMethods.GetValue(_instance, field.FieldName);
			var subReadWriter = new ObjectReadWriter(subObject, field.FieldTypeModel, field.FieldType);
			return subReadWriter.ReadField<T>(path.Slice(1));
		}

		public void WriteField<T>(Span<string> path, T value)
		{
			if (path[0] == ".")
			{
				_instance = value;
				return;
			}

			if (_instance == null)
				return;

			var firstFieldName = path[0];
			var field = Model.Fields.FirstOrDefault(q => q.FieldName == firstFieldName);
			if (field == null)
				throw new Exception("Unknown field on model.");

			if (path.Length == 1)
			{
				_readWriteMethods.SetTypedValue<T>(_instance, field.FieldName, value);
				return;
			}

			var subObject = _readWriteMethods.GetValue(_instance, field.FieldName);
			var subReadWriter = new ObjectReadWriter(subObject, field.FieldTypeModel, field.FieldType);
			subReadWriter.WriteField<T>(path.Slice(1), value);

			if (path[1] == ".")
				_readWriteMethods.SetTypedValue<T>(_instance, field.FieldName, value);
		}
	}
}
