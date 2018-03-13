using Silk.Data.Modelling.Mapping.Binding;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Mapping
{
	public interface IObjectMappingOverride
	{
	}

	public interface IObjectMappingOverride<TFrom, TTo> : IObjectMappingOverride
	{
		void CreateBindings(ObjectMappingBuilder<TFrom, TTo> builder);
	}

	public class ObjectMappingBuilder<TFrom, TTo>
	{
		private readonly MappingBuilder _mappingBuilder;
		private readonly SourceModel _sourceModel;
		private readonly TargetModel _targetModel;

		public ObjectMappingBuilder(MappingBuilder mappingBuilder, SourceModel sourceModel,
			TargetModel targetModel)
		{
			_mappingBuilder = mappingBuilder;
			_sourceModel = sourceModel;
			_targetModel = targetModel;
		}

		public void ConstructWithFactory(Func<TFrom, TTo> factory)
		{
			_mappingBuilder
				.Bind(_targetModel.GetSelf())
				.AssignUsing<UseFactoryBinding<TFrom,TTo>, Func<TFrom, TTo>>(factory);
		}

		public PropertyMappingBuilder<T,TFrom, TTo> Bind<T>(Expression<Func<TTo, T>> property)
		{
			var memberExpression = property.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("Must specify a property to bind.", nameof(property));
			var targetField = _targetModel.Fields.OfType<TargetField<T>>().FirstOrDefault(q => q.FieldName == memberExpression.Member.Name);
			if (targetField == null)
				throw new ArgumentException("Must specify a property to bind.", nameof(property));
			return new PropertyMappingBuilder<T, TFrom, TTo>(targetField, _sourceModel, (BindingBuilder<T>)_mappingBuilder.Bind(targetField));
		}
	}

	public class PropertyMappingBuilder<TProperty, TFrom, TTo>
	{
		public TargetField<TProperty> TargetField { get; }
		public SourceModel SourceModel { get; }
		public BindingBuilder<TProperty> BindingBuilder { get; }

		public PropertyMappingBuilder(TargetField<TProperty> targetField,
			SourceModel sourceModel, BindingBuilder<TProperty> bindingBuilder)
		{
			TargetField = targetField;
			SourceModel = sourceModel;
			BindingBuilder = bindingBuilder;
		}

		public PropertyMappingBuilder<TProperty, TFrom, TTo> ToValue(TProperty value)
		{
			BindingBuilder
				.AssignUsing<ValueBindingFactory<TProperty>, TProperty>(value);
			return this;
		}

		public PropertyMappingBuilder<TProperty, TFrom, TTo> From(Expression<Func<TFrom, TProperty>> property)
		{
			var memberExpression = property.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("Must specify a property to bind.", nameof(property));
			var sourceField = SourceModel.Fields.OfType<SourceField<TProperty>>().FirstOrDefault(q => q.FieldName == memberExpression.Member.Name);
			if (sourceField == null)
				throw new ArgumentException("Must specify a property to bind.", nameof(property));
			BindingBuilder
				.From(sourceField)
				.MapUsing<CopyBinding>();
			return this;
		}

		public PropertyMappingBuilder<TProperty, TFrom, TTo> From<T>(Expression<Func<TFrom, T>> property, Func<T, TProperty> @delegate)
		{
			var memberExpression = property.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("Must specify a property to bind.", nameof(property));
			var sourceField = SourceModel.Fields.OfType<SourceField<TProperty>>().FirstOrDefault(q => q.FieldName == memberExpression.Member.Name);
			if (sourceField == null)
				throw new ArgumentException("Must specify a property to bind.", nameof(property));
			BindingBuilder
				.From(sourceField)
				.MapUsing<DelegateBinding, Delegate>(@delegate);
			return this;
		}
	}
}
