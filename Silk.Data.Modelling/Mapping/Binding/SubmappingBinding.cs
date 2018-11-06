using System;
using System.Linq;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class SubmappingBinding : IMappingBindingFactory<MappingStore>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, MappingStore mappingStore)
		{
			var fromElementType = fromField.Field.ElementType;
			var toElementType = toField.Field.ElementType;

			if (fromElementType != null && toElementType != null &&
				MapReferenceTypes.IsReferenceType(fromElementType) && MapReferenceTypes.IsReferenceType(toElementType))
			{
				return Activator.CreateInstance(typeof(SubmappingBinding<,>).MakeGenericType(fromElementType, toElementType), new object[] {
					TypeModel.GetModelOf(fromElementType),
					TypeModel.GetModelOf(toElementType),
					mappingStore,
					fromField, toField
				}) as MappingBinding;
			}
			else
			{
				return new SubmappingBinding<TFrom, TTo>(
					fromField.Field.FieldTypeModel, toField.Field.FieldTypeModel,
					mappingStore, fromField, toField);
			}
		}
	}

	public abstract class SubmappingBindingBase : MappingBinding
	{
		public abstract Mapping Mapping { get; }

		public SubmappingBindingBase(IFieldReference from, IFieldReference to)
			: base(from, to)
		{
		}
	}

	public class SubmappingBinding<TFrom, TTo> : SubmappingBindingBase
	{
		private readonly IModel _fromModel;
		private readonly IModel _toModel;

		public MappingStore MappingStore { get; }

		private Mapping _mapping;
		public override Mapping Mapping
		{
			get
			{
				if (_mapping == null)
					MappingStore.TryGetMapping(_fromModel, _toModel, out _mapping);
				return _mapping;
			}
		}

		public SubmappingBinding(IModel fromModel, IModel toModel, MappingStore mappingStore, IFieldReference from, IFieldReference to) :
			base(from, to)
		{
			_fromModel = fromModel;
			_toModel = toModel;
			MappingStore = mappingStore;
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var value = from.ReadField<TFrom>(From);
			if (value == null)
				return;
			Mapping.PerformMapping(
				new SubmappingModelReadWriter(from, _fromModel, new string[0]),
				new SubmappingModelReadWriter(to, _toModel, new string[0])
				);
		}

		private class SubmappingModelReadWriter : IModelReadWriter
		{
			public IModel Model { get; }
			public IModelReadWriter RealReadWriter { get; }
			public string[] PrefixPath { get; }

			public IFieldResolver FieldResolver => throw new NotImplementedException();

			public SubmappingModelReadWriter(IModelReadWriter modelReadWriter, IModel model,
				string[] prefixPath)
			{
				Model = model;
				RealReadWriter = modelReadWriter;
				PrefixPath = prefixPath;
			}

			public T ReadField<T>(Span<string> path)
			{
				var fixedPath = PrefixPath.Concat(path.ToArray()).ToArray();
				return RealReadWriter.ReadField<T>(fixedPath);
			}

			public void WriteField<T>(Span<string> path, T value)
			{
				var fixedPath = PrefixPath.Concat(path.ToArray()).ToArray();
				RealReadWriter.WriteField<T>(fixedPath, value);
			}

			public T ReadField<T>(ModelNode modelNode)
			{
				throw new NotImplementedException();
			}

			public void WriteField<T>(ModelNode modelNode, T value)
			{
				throw new NotImplementedException();
			}

			public T ReadField<T>(IFieldReference field)
			{
				throw new NotImplementedException();
			}

			public void WriteField<T>(IFieldReference field, T value)
			{
				throw new NotImplementedException();
			}
		}
	}
}
