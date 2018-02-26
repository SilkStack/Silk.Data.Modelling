using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Abstract base implementation of <see cref="IField{T}"/>.
	/// </summary>
	public abstract class FieldBase<T> : IField<T>
	{
		public Type FieldType { get; } = typeof(T);
		public string FieldName { get; }
		public bool CanRead { get; }
		public bool CanWrite { get; }
		public bool IsEnumerable { get; }
		public Type ElementType { get; }

		private TypeModel<T> _fieldTypeModel;
		public TypeModel<T> FieldTypeModel
		{
			get
			{
				if (_fieldTypeModel == null)
					_fieldTypeModel = TypeModel.GetModelOf<T>();
				return _fieldTypeModel;
			}
		}
		TypeModel IField.FieldTypeModel => FieldTypeModel;

		protected FieldBase(string fieldName, bool canRead, bool canWrite, bool isEnumerable,
			Type elementType)
		{
			FieldName = fieldName;
			CanRead = canRead;
			CanWrite = canWrite;
			IsEnumerable = isEnumerable;
			ElementType = elementType;
		}

		public virtual void Transform(IModelTransformer transformer)
		{
			transformer.VisitField<T>(this);
		}
	}
}
