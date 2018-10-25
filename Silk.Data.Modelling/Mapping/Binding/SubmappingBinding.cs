using System;
using System.Linq;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class SubmappingBinding : IMappingBindingFactory<MappingStore>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, MappingStore mappingStore)
		{
			var fromElementType = fromField.ElementType;
			var toElementType = toField.ElementType;

			if (fromElementType != null && toElementType != null &&
				MapReferenceTypes.IsReferenceType(fromElementType) && MapReferenceTypes.IsReferenceType(toElementType))
			{
				return Activator.CreateInstance(typeof(SubmappingBinding<,>).MakeGenericType(fromElementType, toElementType), new object[] {
					TypeModel.GetModelOf(fromElementType),
					TypeModel.GetModelOf(toElementType),
					mappingStore,
					fromField.FieldPath, toField.FieldPath
				}) as MappingBinding;
			}
			else
			{
				return new SubmappingBinding<TFrom, TTo>(fromField.FieldTypeModel, toField.FieldTypeModel, mappingStore,
					fromField.FieldPath, toField.FieldPath);
			}
		}
	}

	public abstract class SubmappingBindingBase : MappingBinding
	{
		public abstract Mapping Mapping { get; }

		public SubmappingBindingBase(string[] fromPath, string[] toPath)
			: base(fromPath, toPath)
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

		public SubmappingBinding(IModel fromModel, IModel toModel, MappingStore mappingStore, string[] fromPath, string[] toPath) :
			base(fromPath, toPath)
		{
			_fromModel = fromModel;
			_toModel = toModel;
			MappingStore = mappingStore;
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var value = from.ReadField<TFrom>(FromPath);
			if (value == null)
				return;
			Mapping.PerformMapping(
				new SubmappingModelReadWriter(from, _fromModel, FromPath),
				new SubmappingModelReadWriter(to, _toModel, ToPath)
				);
		}

		private class SubmappingModelReadWriter : IModelReadWriter
		{
			public IModel Model { get; }
			public IModelReadWriter RealReadWriter { get; }
			public string[] PrefixPath { get; }

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
		}
	}
}
