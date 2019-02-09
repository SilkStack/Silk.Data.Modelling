using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class MappingFactoryContext<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public TFromModel FromModel { get; }
		public TToModel ToModel { get; }

		public IMappingFactory<TFromModel, TFromField, TToModel, TToField> Factory { get; }

		public List<IBinding<TFromModel, TFromField, TToModel, TToField>> Bindings { get; }
			= new List<IBinding<TFromModel, TFromField, TToModel, TToField>>();

		public MappingFactoryContext(
			TFromModel fromModel, TToModel toModel,
			IMappingFactory<TFromModel, TFromField, TToModel, TToField> factory
			)
		{
			Factory = factory;
			ToModel = toModel;
			FromModel = fromModel;
		}

		public bool IsToFieldBound(IntersectedFields intersectedFields)
			=> IsToFieldBound(intersectedFields.RightField);

		public bool IsToFieldBound(IField toField)
			=> Bindings.Any(binding => binding.ToField == toField);
	}
}
