﻿using System;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class SourceModel : ModelBase<ISourceField>
	{
		/// <summary>
		/// Gets the <see cref="IModel"/> the <see cref="SourceModel"/> instance was built from.
		/// </summary>
		public IModel FromModel { get; }

		public override ISourceField[] Fields { get; }

		public SourceModel(IModel fromModel, ISourceField[] fields)
		{
			FromModel = fromModel;
			Fields = fields;
		}

		public ISourceField GetField(params string[] fieldPath)
		{
			if (fieldPath == null)
				throw new ArgumentNullException(nameof(fieldPath));
			if (fieldPath.Length == 0)
				throw new ArgumentException("Field path must have one or more elements.", nameof(fieldPath));

			ISourceField next = null;
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

	public interface ISourceField : IField
	{
		string[] FieldPath { get; }
		ISourceField[] Fields { get; }
	}

	public class SourceField<T> : FieldBase<T>, ISourceField, IField<T>
	{
		public string[] FieldPath { get; }

		private ISourceField[] _fields;
		public ISourceField[] Fields
		{
			get
			{
				if (_fields == null)
				{
					var fieldTypeSourceModel = FieldTypeModel.TransformToSourceModel(FieldPath);
					_fields = fieldTypeSourceModel.Fields;
				}
				return _fields;
			}
		}

		public SourceField(string fieldName, bool canRead, bool canWrite,
			bool isEnumerable, Type elementType, string[] fieldPath) :
			base(fieldName, canRead, canWrite, isEnumerable, elementType)
		{
			FieldPath = fieldPath;
		}
	}
}
