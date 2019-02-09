using System.Collections.Generic;
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
				GetBindings(),
				GetScopes()
				);

			IEnumerable<IBinding<TFromModel, TFromField, TToModel, TToField>> GetBindings(
				)
			{
				foreach (var binding in mappingFactoryContext.Bindings)
					yield return binding;
			}

			IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> GetScopes()
			{
				yield break;
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
