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
		public IReadOnlyList<IBinding<TFromModel, TFromField, TToModel, TToField>> Bindings { get; }

		public MappingBase(TFromModel fromModel, TToModel toModel,
			IEnumerable<IBinding<TFromModel, TFromField, TToModel, TToField>> bindings)
		{
			FromModel = fromModel;
			ToModel = toModel;
			Bindings = bindings.ToArray();
		}

		public void Map(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			using (var enumerator = Bindings.GetEnumerator())
			while (enumerator.MoveNext())
			{
				RunBinding(source, destination, enumerator);
			}
		}

		private void RunBinding(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination,
			IEnumerator<IBinding<TFromModel, TFromField, TToModel, TToField>> enumerator)
		{
			var binding = enumerator.Current;
			if (binding.ToField.IsEnumerableType)
				MapEnumerable(source, destination, enumerator);
			else
				binding.Run(source, destination);
		}

		private void MapEnumerable(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination,
			IEnumerator<IBinding<TFromModel, TFromField, TToModel, TToField>> enumerator)
		{
			var binding = enumerator.Current;
			//  todo: introduce IBinding.DependencyPath to use here instead of FromPath so bindings without a bound source can still enumerate properly
			using (var readEnumerator = source.GetEnumerator(binding.FromPath))
			using (var writeStream = destination.CreateEnumerableStream(binding.ToPath))
			while (readEnumerator.MoveNext())
			{
				RunBinding(readEnumerator.Current, writeStream.CreateNew(), enumerator);
			}
		}
	}
}
