using System.Collections.Generic;
using System.Linq;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;
using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Mapping factory base that utilizes BindingScopes to create mappings.
	/// </summary>
	/// <typeparam name="TFromModel"></typeparam>
	/// <typeparam name="TFromField"></typeparam>
	/// <typeparam name="TToModel"></typeparam>
	/// <typeparam name="TToField"></typeparam>
	public abstract class MappingFactoryBase<TFromModel, TFromField, TToModel, TToField> :
		IMappingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		/// <summary>
		/// Gets a writable collection of binding factories used when creating mappings.
		/// </summary>
		public ICollection<IBindingFactory<TFromModel, TFromField, TToModel, TToField>> BindingFactories { get; }
			= new List<IBindingFactory<TFromModel, TFromField, TToModel, TToField>>();

		protected virtual MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> CreateFactoryContext(IIntersection<TFromModel, TFromField, TToModel, TToField> intersection)
		{
			return new MappingFactoryContext<TFromModel, TFromField, TToModel, TToField>(
				intersection.LeftModel, intersection.RightModel, this);
		}

		protected virtual void CreateBindings(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> context,
			IIntersection<TFromModel, TFromField, TToModel, TToField> intersection
			)
		{
			foreach (var factory in BindingFactories)
			{
				foreach (var interesectedFields in intersection.IntersectedFields)
				{
					factory.CreateBinding(context, interesectedFields);
				}
			}
		}

		protected abstract IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			BindingScope<TFromModel, TFromField, TToModel, TToField> bindingScope
			);

		protected virtual BindingScope<TFromModel, TFromField, TToModel, TToField> CreateScopedBindings(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext
			)
		{
			return new BranchBindingScope<TFromModel, TFromField, TToModel, TToField>(
				GetBindings(new string[0]).Where(binding => !binding.ToField.IsEnumerableType),
				GetScopes(new string[0])
				);

			IEnumerable<IBinding<TFromModel, TFromField, TToModel, TToField>> GetBindings(
				string[] path
				)
			{
				foreach (var binding in mappingFactoryContext.Bindings)
				{
					var toFieldPath = binding.ToPath.Fields.Select(q => q.FieldName).ToArray();
					if (toFieldPath.Length - 1 == path.Length && toFieldPath.Take(path.Length).SequenceEqual(path))
						yield return binding;
				}
			}

			IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> GetScopes(
				string[] path
				)
			{
				foreach (var binding in GetBindings(path))
				{
					var subPath = path.Concat(new[] { binding.ToField.FieldName }).ToArray();

					if (binding.ToField.IsEnumerableType)
					{
						var scopeBuilder = new EnumerableScopeBuilder(
							binding,
							GetScopes(subPath),
							binding.FromField
							);
						binding.ToField.Dispatch(scopeBuilder);
						yield return scopeBuilder.Scope;
					}
					else
					{
						var bindings = GetBindings(subPath).Where(q => !q.ToField.IsEnumerableType).ToArray();
						var scopes = GetScopes(subPath).ToArray();
						if (bindings.Length > 0 || scopes.Length > 0)
						{
							yield return new BranchBindingScope<TFromModel, TFromField, TToModel, TToField>(
								bindings, scopes);
						}
					}
				}
			}
		}

		public IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(IIntersection<TFromModel, TFromField, TToModel, TToField> intersection)
		{
			var context = CreateFactoryContext(intersection);
			CreateBindings(context, intersection);
			var bindingScope = CreateScopedBindings(context);
			return CreateMapping(context, bindingScope);
		}

		private class EnumerableScopeBuilder : IFieldGenericExecutor
		{
			private readonly IBinding<TFromModel, TFromField, TToModel, TToField> _binding;
			private readonly IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> _scopes;
			private readonly TFromField _fromField;

			public BindingScope<TFromModel, TFromField, TToModel, TToField> Scope { get; private set; }

			public EnumerableScopeBuilder(
				IBinding<TFromModel, TFromField, TToModel, TToField> binding,
				IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> scopes,
				TFromField fromField
				)
			{
				_binding = binding;
				_scopes = scopes;
				_fromField = fromField;
			}

			void IFieldGenericExecutor.Execute<TField, TData>(IField field)
			{
				var subBuilder = new EnumerableScopeBuilder<TData>(_binding, _scopes);
				_fromField.Dispatch(subBuilder);
				Scope = subBuilder.Scope;
			}
		}

		private class EnumerableScopeBuilder<TToData> : IFieldGenericExecutor
		{
			private readonly IBinding<TFromModel, TFromField, TToModel, TToField> _binding;
			private readonly IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> _scopes;

			public BindingScope<TFromModel, TFromField, TToModel, TToField> Scope { get; private set; }

			public EnumerableScopeBuilder(
				IBinding<TFromModel, TFromField, TToModel, TToField> binding,
				IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> scopes
				)
			{
				_binding = binding;
				_scopes = scopes;
			}

			void IFieldGenericExecutor.Execute<TField, TData>(IField field)
			{
				Scope = new EnumerableBindingScope<TFromModel, TFromField, TToModel, TToField, TData, TToData>(
					_binding, _scopes
					);
			}
		}
	}
}
