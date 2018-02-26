using System;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class TargetModel : ModelBase<ITargetField>
	{
		/// <summary>
		/// Gets the <see cref="IModel"/> the <see cref="SourceModel"/> instance was built from.
		/// </summary>
		public IModel FromModel { get; }

		public override ITargetField[] Fields { get; }

		public TargetModel(IModel fromModel, ITargetField[] fields)
		{
			FromModel = fromModel;
			Fields = fields;
		}

		public ITargetField GetField(params string[] fieldPath)
		{
			if (fieldPath == null)
				throw new ArgumentNullException(nameof(fieldPath));
			if (fieldPath.Length == 0)
				throw new ArgumentException("Field path must have one or more elements.", nameof(fieldPath));

			ITargetField next = null;
			foreach (var fieldName in fieldPath)
			{
				if (next == null)
					next = Fields.FirstOrDefault(q => q.FieldName == fieldName);
				else
					next = next.Fields.FirstOrDefault(q => q.FieldName == fieldName);

				if (next == null)
					return null;
			}
			return next;
		}
	}

	public interface ITargetField : IField
	{
		string[] FieldPath { get; }
		ITargetField[] Fields { get; }
		BindingBuilder CreateBindingBuilder();
	}

	public class TargetField<T> : FieldBase<T>, ITargetField, IField<T>
	{
		public string[] FieldPath { get; }

		private ITargetField[] _fields;
		public ITargetField[] Fields
		{
			get
			{
				if (_fields == null)
				{
					var fieldTypeSourceModel = FieldTypeModel.TransformToTargetModel(FieldPath);
					_fields = fieldTypeSourceModel.Fields;
				}
				return _fields;
			}
		}

		public TargetField(string fieldName, bool canRead, bool canWrite,
			bool isEnumerable, Type elementType, string[] fieldPath) :
			base(fieldName, canRead, canWrite, isEnumerable, elementType)
		{
			FieldPath = fieldPath;
		}

		public BindingBuilder CreateBindingBuilder()
		{
			return new BindingBuilder<T>(this);
		}
	}
}
