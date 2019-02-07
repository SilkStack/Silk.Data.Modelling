using System;
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

		protected virtual IBinding[] GetBindings(IIntersection<TFromModel, TFromField, TToModel, TToField> intersection)
		{
			var result = new List<IBinding>();
			foreach (var factory in BindingFactories)
			{
				foreach (var interesectedFields in intersection.IntersectedFields)
				{
					if (factory.GetBinding(interesectedFields, out var binding))
					{
						result.Add(binding);
					}
				}
			}
			return result.ToArray();
		}

		public IMapping<TFromModel, TFromField, TToModel, TToField> CreateMapping(IIntersection<TFromModel, TFromField, TToModel, TToField> intersection)
		{
			var bindings = GetBindings(intersection);
			throw new NotImplementedException();
		}
	}
}
