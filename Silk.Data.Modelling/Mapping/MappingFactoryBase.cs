using System.Collections.Generic;
using System.Linq;
using Silk.Data.Modelling.Analysis;
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
				foreach (var binding in mappingFactoryContext.Bindings)
				{
					var toFieldPath = binding.ToPath.Fields.Select(q => q.FieldName);
					if (toFieldPath.Take(path.Length).SequenceEqual(path))
					{
						var subPath = path.Concat(new[] { binding.ToField.FieldName }).ToArray();
						var subBindings = GetBindings(subPath).ToArray();
						if (subBindings.Length > 0)
							yield return new BranchBindingScope<TFromModel, TFromField, TToModel, TToField>(
								subBindings.Where(subBinding => !subBinding.ToField.IsEnumerableType), GetScopes(subPath));

						foreach (var subBinding in subBindings.Where(q => q.ToField.IsEnumerableType))
						{
							var subEnumPath = subPath.Concat(new[] { subBinding.ToField.FieldName }).ToArray();
							yield return new EnumerableBindingScope<TFromModel, TFromField, TToModel, TToField>(
								subBinding, GetScopes(subEnumPath)
								);
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
	}
}
