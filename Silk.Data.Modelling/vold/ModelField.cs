using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A field member of a model.
	/// </summary>
	public class ModelField
	{
		/// <summary>
		/// Gets the model the field is present on.
		/// </summary>
		public Model ParentModel { get; internal set; }

		/// <summary>
		/// Gets a value indicating if the field is readable.
		/// </summary>
		public bool CanRead { get; }

		/// <summary>
		/// Gets a value indicating if the field is writeable.
		/// </summary>
		public bool CanWrite { get; }

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets an array of metadata on the field.
		/// </summary>
		public object[] Metadata { get; }

		/// <summary>
		/// Gets the data type of the field.
		/// </summary>
		public Type DataType { get; }

		/// <summary>
		/// Gets a value indicating if the field is an enumerable type.
		/// </summary>
		public bool IsEnumerable { get; }

		/// <summary>
		/// Gets the enumerable type.
		/// </summary>
		public Type EnumerableType { get; }

		protected TypedModel InternalDataTypeModel { get; private set; }
		/// <summary>
		/// Gets the model of the fields data type.
		/// </summary>
		public TypedModel DataTypeModel
		{
			get
			{
				if (InternalDataTypeModel == null)
					InternalDataTypeModel = TypeModeller.GetModelOf(DataType);
				return InternalDataTypeModel;
			}
		}

		public ModelField(string name, bool canRead, bool canWrite,
			object[] metadata, Type dataType, Type enumerableType = null)
		{
			Name = name;
			CanRead = canRead;
			CanWrite = canWrite;
			Metadata = metadata;
			DataType = dataType;
			IsEnumerable = enumerableType != null;
			EnumerableType = enumerableType;
		}
	}
}
