using System;

namespace Silk.Data.Modelling
{
	public class TypedModelField<T> : ModelField
	{
		/// <summary>
		/// Gets the model of the fields data type.
		/// </summary>
		public new TypedModel<T> DataTypeModel => (TypedModel<T>)InternalDataTypeModel;

		public TypedModelField(string name, bool canRead, bool canWrite,
			object[] metadata, Type enumerableType = null) 
			: base(name, canRead, canWrite, metadata, typeof(T), enumerableType)
		{
		}
	}
}
