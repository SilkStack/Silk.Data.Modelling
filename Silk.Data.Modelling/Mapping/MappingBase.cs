using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public abstract class MappingBase<TFromModel, TFromField, TToModel, TToField> : IMapping<TFromModel, TFromField, TToModel, TToField>
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
		where TFromField : class, IField
		where TToField : class, IField
	{
		public TFromModel FromModel { get; }
		public TToModel ToModel { get; }
		public IBinding<TFromModel, TFromField, TToModel, TToField>[] Bindings { get; }

		public MappingBase(TFromModel fromModel, TToModel toModel,
			IEnumerable<IBinding<TFromModel, TFromField, TToModel, TToField>> bindings)
		{
			FromModel = fromModel;
			ToModel = toModel;
			Bindings = bindings.ToArray();
		}

		public void Map(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			foreach (var binding in Bindings)
			{
				if (binding.ToField.IsEnumerableType)
					MapEnumerable(source, destination, binding);
				else
					binding.Run(source, destination);
			}
		}

		private void MapEnumerable(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination, IBinding<TFromModel, TFromField, TToModel, TToField> binding)
		{
			using (var enumerator = source.GetEnumerator(binding.FromPath))
			using (var writeStream = destination.CreateEnumerableStream(binding.ToPath))
			while (enumerator.MoveNext())
			{

			}
		}
	}
}
