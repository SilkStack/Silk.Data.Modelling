﻿using Silk.Data.Modelling.Mapping.Binding;
using System;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class TargetModel : ModelBase<ITargetField>
	{
		/// <summary>
		/// Gets the <see cref="IModel"/> the <see cref="SourceModel"/> instance was built from.
		/// </summary>
		public IModel FromModel { get; }

		public override ITargetField[] Fields { get; }

		private readonly string[] _selfPath;
		private ITargetField _self;
		private readonly IModel _rootModel;

		public TargetModel(IModel fromModel, ITargetField[] fields, string[] selfPath,
			IModel rootModel = null)
		{
			FromModel = fromModel;
			Fields = fields;

			_selfPath = selfPath;
			_rootModel = rootModel ?? fromModel;
		}

		public ITargetField GetSelf()
		{
			if (_self != null)
				return _self;

			var typeModel = FromModel as TypeModel;
			if (typeModel == null)
				return null;

			var typeOfSelf = typeModel.Type;
			_self = typeof(TargetModel).GetTypeInfo().GetDeclaredMethod("MakeSelfField")
				.MakeGenericMethod(typeOfSelf).Invoke(this, new object[0]) as ITargetField;
			return _self;
		}

		private ITargetField MakeSelfField<T>()
		{
			var enumerableElementType = typeof(T).GetEnumerableElementType();
			return new TargetField<T>(".", true, true, enumerableElementType != null, enumerableElementType, _selfPath, FromModel);
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

		public override IFieldResolver CreateFieldResolver()
		{
			throw new NotSupportedException();
		}

		public override IFieldReference GetFieldReference(ISourceField sourceField)
		{
			throw new NotSupportedException();
		}

		public override IFieldReference GetFieldReference(ITargetField targetField)
		{
			throw new NotSupportedException();
		}
	}

	public interface ITargetField : IField
	{
		IModel RootModel { get; }
		string[] FieldPath { get; }
		ITargetField[] Fields { get; }
		BindingBuilder CreateBindingBuilder();
		Binding.Binding CreateBinding(IAssignmentBindingFactory bindingFactory);
		Binding.Binding CreateBinding<TOption>(IAssignmentBindingFactory<TOption> bindingFactory, TOption option);
	}

	public class TargetField<T> : FieldBase<T>, ITargetField, IField<T>
	{
		private readonly IModel _rootModel;

		public IModel RootModel => _rootModel;

		public string[] FieldPath { get; }

		private ITargetField[] _fields;
		public ITargetField[] Fields
		{
			get
			{
				if (_fields == null)
				{
					var fieldTypeSourceModel = FieldTypeModel.TransformToTargetModel(FieldPath, _rootModel);
					_fields = fieldTypeSourceModel.Fields;
				}
				return _fields;
			}
		}

		public TargetField(string fieldName, bool canRead, bool canWrite,
			bool isEnumerable, Type elementType, string[] fieldPath, IModel rootModel) :
			base(fieldName, canRead, canWrite, isEnumerable, elementType)
		{
			_rootModel = rootModel;
			FieldPath = fieldPath;
		}

		public BindingBuilder CreateBindingBuilder()
		{
			return new BindingBuilder<T>(this);
		}

		public Binding.Binding CreateBinding(IAssignmentBindingFactory bindingFactory)
		{
			return bindingFactory.CreateBinding<T>(_rootModel.GetFieldReference(this));
		}

		public Binding.Binding CreateBinding<TOption>(IAssignmentBindingFactory<TOption> bindingFactory, TOption option)
		{
			return bindingFactory.CreateBinding<T>(_rootModel.GetFieldReference(this), option);
		}
	}
}
