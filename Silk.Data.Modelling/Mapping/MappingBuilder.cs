using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class MappingBuilder
	{
		private readonly List<IMappingConvention> _conventions = new List<IMappingConvention>();
		private readonly List<BindingBuilder> _bindings = new List<BindingBuilder>();

		public IModel FromModel { get; }
		public IModel ToModel { get; }
		public IReadOnlyCollection<IMappingConvention> Conventions => _conventions;
		public MappingStore MappingStore { get; }
		public MappingBuilderStack BuilderStack { get; }

		public MappingBuilder(IModel fromModel, IModel toModel,
			MappingStore mappingStore = null, MappingBuilderStack builderStack = null)
		{
			FromModel = fromModel;
			ToModel = toModel;
			MappingStore = mappingStore ?? new MappingStore();
			BuilderStack = builderStack ?? new MappingBuilderStack();
		}

		public bool IsBound(ITargetField field)
		{
			return _bindings.Any(q => q.Target == field);
		}

		public bool IsBound(ISourceField field)
		{
			return _bindings.Any(q => q.Source != null && q.Source == field);
		}

		public BindingBuilder Bind(ITargetField field)
		{
			var builder = field.CreateBindingBuilder();
			_bindings.Add(builder);
			return builder;
		}

		public void AddConvention<T>()
			where T : IMappingConvention, new()
		{
			AddConvention(new T());
		}

		public void AddConvention(IMappingConvention convention)
		{
			_conventions.Add(convention);
		}

		public Mapping BuildMapping()
		{
			var sourceModel = FromModel as SourceModel;
			if (sourceModel == null)
				sourceModel = FromModel.TransformToSourceModel();
			var targetModel = ToModel as TargetModel;
			if (targetModel == null)
				targetModel = ToModel.TransformToTargetModel();

			BuilderStack.Push(this);

			foreach (var convention in _conventions)
				convention.CreateBindings(sourceModel, targetModel, this);

			BuilderStack.Pop();

			var mapping = new Mapping(FromModel, ToModel, _bindings.Select(q => q.Binding).ToArray());
			MappingStore.AddMapping(FromModel, ToModel, mapping);
			return mapping;
		}
	}

	public class MappingBuilderStack : Stack<MappingBuilder>
	{
		public bool IsBeingMapped(IModel fromModel, IModel toModel)
		{
			return this.Any(q => q.FromModel.Equals(fromModel) && q.ToModel.Equals(toModel));
		}
	}

	public abstract class BindingBuilder
	{
		public ITargetField Target { get; protected set; }
		public ISourceField Source { get; protected set; }

		public abstract Binding Binding { get; }
		public abstract BindingBuilder From(ISourceField sourceField);
		public abstract BindingBuilder Using<TBinding>()
			where TBinding : IBindingFactory, new();
		public abstract BindingBuilder Using<TBinding, TOption>(TOption option)
			where TBinding : IBindingFactory<TOption>, new();
	}

	public class BindingBuilder<T> : BindingBuilder
	{
		private Binding _binding;

		public new TargetField<T> Target { get; }
		public override Binding Binding => _binding;

		public BindingBuilder(TargetField<T> targetField)
		{
			Target = targetField;
			base.Target = targetField;
		}

		public override BindingBuilder From(ISourceField sourceField)
		{
			Source = sourceField;
			return this;
		}

		public override BindingBuilder Using<TBinding>()
		{
			if (Source == null)
				throw new InvalidOperationException("Must assign a source field before assigning a binding.");
			_binding = Source.CreateBinding<T>(new TBinding(), Target);
			return this;
		}

		public override BindingBuilder Using<TBinding, TOption>(TOption option)
		{
			if (Source == null)
				throw new InvalidOperationException("Must assign a source field before assigning a binding.");
			_binding = Source.CreateBinding<T, TOption>(new TBinding(), Target, option);
			return this;
		}
	}
}
