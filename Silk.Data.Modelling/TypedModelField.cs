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

		public TypedModelField(string name, bool canRead, bool canWrite, Type dataType)
			: base(name, canRead, canWrite)
		{
			DataType = dataType;
		}
	}

	public class TypedModelField<T> : TypedModelField
	{
		/// <summary>
		/// Gets the model of the fields data type.
		/// </summary>
		public new TypedModel<T> DataTypeModel => (TypedModel<T>)InternalDataTypeModel;

		public TypedModelField(string name, bool canRead, bool canWrite) 
			: base(name, canRead, canWrite, typeof(T))
		{
		}
	}
}
