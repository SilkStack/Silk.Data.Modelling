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

		public MappingBuilder(IModel fromModel, IModel toModel)
		{
			FromModel = fromModel;
			ToModel = toModel;
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
			var sourceModel = FromModel.TransformToSourceModel();
			var targetModel = ToModel.TransformToTargetModel();
			foreach (var convention in _conventions)
				convention.CreateBindings(sourceModel, targetModel, this);
			return new Mapping(FromModel, ToModel, _bindings.Select(q => q.Binding).ToArray());
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
	}
}
