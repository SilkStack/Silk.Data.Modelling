using System;
using Silk.Data.Modelling.Analysis;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CreateInstancesOfContainerTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : IField
		where TToField : IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public bool GetBinding(IntersectedFields intersectedFields, out IBinding binding)
		{
			throw new NotImplementedException();
		}
	}
}
