using System.Collections.Generic;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
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

		protected abstract IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext);

		public IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(IIntersection<TFromModel, TFromField, TToModel, TToField> intersection)
		{
			var context = CreateFactoryContext(intersection);
			CreateBindings(context, intersection);
			return CreateMapping(context);
		}
	}
}
