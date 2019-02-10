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

		public bool IsToFieldBound(IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields)
			=> IsToFieldBound(intersectedFields.RightPath);

		public bool IsToFieldBound(IFieldPath<TToModel, TToField> toPath)
			=> Bindings.Any(binding => binding.ToPath.Fields.Select(q => q.FieldName).SequenceEqual(
				toPath.Fields.Select(q => q.FieldName)
				));
	}
}
