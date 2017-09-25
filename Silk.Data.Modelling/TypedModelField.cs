using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A model field with a data type.
	/// </summary>
	public class TypedModelField  : ModelField
	{
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

		public TypedModelField(string name, bool canRead, bool canWrite, Type dataType,
			Type enumerableType = null)
			: base(name, canRead, canWrite)
		{
			DataType = dataType;
			IsEnumerable = enumerableType != null;
			EnumerableType = enumerableType;
		}
	}

	public class TypedModelField<T> : TypedModelField
	{
		/// <summary>
		/// Gets the model of the fields data type.
		/// </summary>
		public new TypedModel<T> DataTypeModel => (TypedModel<T>)InternalDataTypeModel;

		public TypedModelField(string name, bool canRead, bool canWrite,
			Type enumerableType = null) 
			: base(name, canRead, canWrite, typeof(T), enumerableType)
		{
		}
	}
}
