using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A field that was derived from a property on a <see cref="Type"/>.
	/// </summary>
	public abstract class PropertyField : IField
	{
		protected abstract TypeModel _FieldTypeModel { get; }

		public abstract string FieldName { get; }
		public abstract Type FieldType { get; }
		public abstract bool CanRead { get; }
		public abstract bool CanWrite { get; }
		public abstract bool IsEnumerable { get; }
		public abstract Type ElementType { get; }
		public TypeModel FieldTypeModel => _FieldTypeModel;
	}

	public class PropertyField<T> : PropertyField, IField<T>
	{
		public override Type FieldType { get; } = typeof(T);
		public override string FieldName { get; }
		public override bool CanRead { get; }
		public override bool CanWrite { get; }
		public override bool IsEnumerable { get; }
		public override Type ElementType { get; }

		private TypeModel<T> _fieldTypeModel;
		public new TypeModel<T> FieldTypeModel
		{
			get
			{
				if (_fieldTypeModel == null)
					_fieldTypeModel = TypeModel.GetModelOf<T>();
				return _fieldTypeModel;
			}
		}
		protected override TypeModel _FieldTypeModel => FieldTypeModel;

		public PropertyField(string fieldName, bool canRead, bool canWrite, bool isEnumerable,
			Type elementType)
		{
			FieldName = fieldName;
			CanRead = canRead;
			CanWrite = canWrite;
			IsEnumerable = isEnumerable;
			ElementType = elementType;
		}
	}
}
