using System.Linq;

namespace Silk.Data.Modelling
{
	public class ObjectReadWriter : IModelReadWriter
	{
		public IModel Model { get; }
		private object _instance;
		private ObjectReadWriteHelpers _readWriteMethods;

		public ObjectReadWriter(object instance, IModel model)
		{
			_instance = instance;
			Model = model;
			_readWriteMethods = ObjectReadWriteHelpers.GetForType(instance.GetType());
		}

		public T ReadField<T>(string[] path, int offset)
		{
			var field = Model.Fields.FirstOrDefault(q => q.FieldName == path[offset]);
			if (field == null)
				throw new System.Exception("Unknown field on model.");

			if (offset == path.Length - 1)
				return _readWriteMethods.GetTypedValue<T>(_instance, field.FieldName);

			var subObject = _readWriteMethods.GetValue(_instance, field.FieldName);
			if (subObject == null)
				return default(T);

			var subReadWriter = new ObjectReadWriter(subObject, field.FieldTypeModel);
			return subReadWriter.ReadField<T>(path, offset + 1);
		}

		public void WriteField<T>(string[] path, int offset, T value)
		{
			var field = Model.Fields.FirstOrDefault(q => q.FieldName == path[offset]);
			if (field == null)
				throw new System.Exception("Unknown field on model.");

			if (offset == path.Length - 1)
			{
				_readWriteMethods.SetTypedValue<T>(_instance, field.FieldName, value);
				return;
			}

			var subObject = _readWriteMethods.GetValue(_instance, field.FieldName);
			if (subObject == null)
				return;

			var subReadWriter = new ObjectReadWriter(subObject, field.FieldTypeModel);
			subReadWriter.WriteField<T>(path, offset + 1, value);
		}
	}
}
