using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public abstract class Binding
	{
		public abstract void PerformBinding(IModelReadWriter from, IModelReadWriter to);
	}

	/// <summary>
	/// Assigns a value in the result mapping graph.
	/// </summary>
	public abstract class AssignmentBinding : Binding
	{
		public string[] ToPath { get; }

		public AssignmentBinding(string[] toPath)
		{
			ToPath = toPath;
		}

		public override void PerformBinding(IModelReadWriter from, IModelReadWriter to)
		{
			AssignBindingValue(from, to);
		}

		public abstract void AssignBindingValue(IModelReadWriter from, IModelReadWriter to);
	}

	public class UseFactory<TFrom, TTo> : AssignmentBinding
	{
		private readonly Func<TFrom, TTo> _factory;

		public UseFactory(Func<TFrom, TTo> factory, string[] toPath) :
			base(toPath)
		{
			_factory = factory;
		}

		public TTo CreateInstance(TFrom from)
		{
			return _factory(from);
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(ToPath, 0, CreateInstance(from.ReadField<TFrom>(new[] { "." }, 0)));
		}
	}

	public class UseFactoryBinding<TFrom, TTo> : IAssignmentBindingFactory<Func<TFrom, TTo>>
	{
		public AssignmentBinding CreateBinding<T>(ITargetField toField, Func<TFrom, TTo> bindingOption)
		{
			return new UseFactory<TFrom, TTo>(bindingOption, toField.FieldPath);
		}
	}

	public class CreateInstanceIfNull<T> : AssignmentBinding
	{
		private readonly Func<T> _createInstance;

		public CreateInstanceIfNull(ConstructorInfo constructorInfo, string[] toPath) :
			base(toPath)
		{
			_createInstance = CreateCtorMethod(constructorInfo);
		}

		private Func<T> CreateCtorMethod(ConstructorInfo constructorInfo)
		{
			var method = new DynamicMethod("Ctor", typeof(T), new Type[0], true);
			var ilgen = method.GetILGenerator();
			ilgen.Emit(OpCodes.Newobj, constructorInfo);
			ilgen.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(Func<T>)) as Func<T>;
		}

		public T CreateInstance()
		{
			return _createInstance();
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var nullCheck = to.ReadField<T>(ToPath, 0);
			if (nullCheck == null)
			{
				to.WriteField<T>(ToPath, 0, _createInstance());
			}
		}
	}

	public class CreateInstanceIfNull : IAssignmentBindingFactory<ConstructorInfo>
	{
		public AssignmentBinding CreateBinding<TTo>(ITargetField toField, ConstructorInfo bindingOption)
		{
			return new CreateInstanceIfNull<TTo>(bindingOption, toField.FieldPath);
		}
	}

	/// <summary>
	/// Binds a value between models.
	/// </summary>
	public abstract class MappingBinding : Binding
	{
		public string[] FromPath { get; }

		public string[] ToPath { get; }

		public MappingBinding(string[] fromPath, string[] toPath)
		{
			FromPath = fromPath;
			ToPath = toPath;
		}

		public override void PerformBinding(IModelReadWriter from, IModelReadWriter to)
		{
			CopyBindingValue(from, to);
		}

		/// <summary>
		/// Copies bound value from <see cref="from"/> to <see cref="to"/>.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public abstract void CopyBindingValue(IModelReadWriter from, IModelReadWriter to);
	}

	public class CopyBinding : IMappingBindingFactory
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField)
		{
			if (typeof(TFrom) != typeof(TTo))
				throw new InvalidOperationException("TFrom and TTo type mismatch.");
			return new CopyBinding<TFrom>(fromField.FieldPath, toField.FieldPath);
		}
	}

	public class CopyBinding<T> : MappingBinding
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

	public class ConvertBinding<TFrom, TTo> : MappingBinding
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

	public class SubmappingBinding : IMappingBindingFactory<MappingStore>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, MappingStore mappingStore)
		{
			return new SubmappingBinding<TFrom, TTo>(fromField.FieldTypeModel, toField.FieldTypeModel, mappingStore,
				fromField.FieldPath, toField.FieldPath);
		}
	}

	public class SubmappingBinding<TFrom, TTo> : MappingBinding
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
			var value = from.ReadField<TFrom>(FromPath, 0);
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

	public class EnumerableBinding<TFrom, TTo> : MappingBinding
	{
		public EnumerableBinding(string[] fromPath, string[] toPath, MappingBinding elementBinding) : base(fromPath, toPath)
		{
			ElementBinding = elementBinding;
		}

		public MappingBinding ElementBinding { get; }

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			throw new NotImplementedException();
		}
	}

	public class EnumerableBindingFactory : IMappingBindingFactory<IMappingBindingFactory>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, IMappingBindingFactory bindingOption)
		{
			var elementBinding = typeof(IMappingBindingFactory).GetTypeInfo()
				.GetDeclaredMethod(nameof(IMappingBindingFactory.CreateBinding))
				.MakeGenericMethod(fromField.ElementType, toField.ElementType)
				.Invoke(bindingOption, new object[] { fromField, toField })
				as MappingBinding;
			return new EnumerableBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath, elementBinding);
		}
	}
}
