using System;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Binds a value between models.
	/// </summary>
	public abstract class Binding
	{
		public string[] FromPath { get; }

		public string[] ToPath { get; }

		public Binding(string[] fromPath, string[] toPath)
		{
			FromPath = fromPath;
			ToPath = toPath;
		}

		/// <summary>
		/// Copies bound value from <see cref="from"/> to <see cref="to"/>.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public abstract void CopyBindingValue(IModelReadWriter from, IModelReadWriter to); 
	}

	public class CopyBinding : IBindingFactory
	{
		public Binding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField)
		{
			if (typeof(TFrom) != typeof(TTo))
				throw new InvalidOperationException("TFrom and TTo type mismatch.");
			return new CopyBinding<TFrom>(fromField.FieldPath, toField.FieldPath);
		}
	}

	public class CopyBinding<T> : Binding
	{
		public CopyBinding(string[] fromPath, string[] toPath)
			: base(fromPath, toPath)
		{
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<T>(ToPath, 0, from.ReadField<T>(FromPath, 0));
		}
	}

	public class ConvertBinding<TFrom, TTo> : Binding
	{
		public Converter<TFrom, TTo> Converter { get; }

		public ConvertBinding(Converter<TFrom, TTo> converter, string[] fromPath, string[] toPath)
			: base(fromPath, toPath)
		{
			Converter = converter;
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(ToPath, 0, Converter.Convert(from.ReadField<TFrom>(FromPath, 0)));
		}
	}

	public class SubmappingBinding : IBindingFactory<MappingStore>
	{
		public Binding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, MappingStore mappingStore)
		{
			return new SubmappingBinding<TFrom, TTo>(fromField.FieldTypeModel, toField.FieldTypeModel, mappingStore,
				fromField.FieldPath, toField.FieldPath);
		}
	}

	public class SubmappingBinding<TFrom, TTo> : Binding
	{
		private readonly IModel _fromModel;
		private readonly IModel _toModel;

		public MappingStore MappingStore { get; }

		private Mapping _mapping;
		public Mapping Mapping
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

			public T ReadField<T>(string[] path, int offset)
			{
				var fixedPath = path.Take(offset).Concat(PrefixPath).Concat(path.Skip(offset)).ToArray();
				return RealReadWriter.ReadField<T>(fixedPath, offset);
			}

			public void WriteField<T>(string[] path, int offset, T value)
			{
				var fixedPath = path.Take(offset).Concat(PrefixPath).Concat(path.Skip(offset)).ToArray();
				RealReadWriter.WriteField<T>(fixedPath, offset, value);
			}
		}
	}
}
