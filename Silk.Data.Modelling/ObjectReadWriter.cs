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

		public T ReadField<T>(string[] path, int offset)
		{
			if (path[offset] == ".")
				return (T)_instance;

			if (_instance == null)
				return default(T);

			var field = Model.Fields.FirstOrDefault(q => q.FieldName == path[offset]);
			if (field == null)
				throw new System.Exception("Unknown field on model.");

			if (offset == path.Length - 1)
				return _readWriteMethods.GetTypedValue<T>(_instance, field.FieldName);

			var subObject = _readWriteMethods.GetValue(_instance, field.FieldName);
			var subReadWriter = new ObjectReadWriter(subObject, field.FieldTypeModel, field.FieldType);
			return subReadWriter.ReadField<T>(path, offset + 1);
		}

		public void WriteField<T>(string[] path, int offset, T value)
		{
			if (path[offset] == ".")
			{
				_instance = value;
				return;
			}

			if (_instance == null)
				return;

			var field = Model.Fields.FirstOrDefault(q => q.FieldName == path[offset]);
			if (field == null)
				throw new System.Exception("Unknown field on model.");

			if (offset == path.Length - 1)
			{
				_readWriteMethods.SetTypedValue<T>(_instance, field.FieldName, value);
				return;
			}

			var subObject = _readWriteMethods.GetValue(_instance, field.FieldName);
			var subReadWriter = new ObjectReadWriter(subObject, field.FieldTypeModel, field.FieldType);
			subReadWriter.WriteField<T>(path, offset + 1, value);

			if (path[offset + 1] == ".")
				_readWriteMethods.SetTypedValue<T>(_instance, field.FieldName, value);
		}
	}
}
