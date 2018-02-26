using System;

namespace Silk.Data.Modelling.Mapping
{
	public class MappingBuilder
	{
		public BindingBuilder For(ITargetField field)
		{
			return field.CreateBindingBuilder();
		}
	}

	public abstract class BindingBuilder
	{
		public abstract BindingBuilder MapFrom(ISourceField sourceField);
		public abstract BindingBuilder WithCopyBinding();
	}

	public class BindingBuilder<T> : BindingBuilder
	{
		public TargetField<T> Target { get; }
		public ISourceField Source { get; private set; }
		public Binding Binding { get; private set; }

		public BindingBuilder(TargetField<T> targetField)
		{
			Target = targetField;
		}

		public override BindingBuilder MapFrom(ISourceField sourceField)
		{
			Source = sourceField;
			return this;
		}

		public override BindingBuilder WithCopyBinding()
		{
			if (Source == null)
				throw new InvalidOperationException("Must specify a source field first.");
			if (Source.FieldType != typeof(T))
				throw new InvalidOperationException("Source field type and target field type mismatched.");
			Binding = new CopyBinding<T>(Source.FieldPath, Target.FieldPath);
			return this;
		}
	}
}
