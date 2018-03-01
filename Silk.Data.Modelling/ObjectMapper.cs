using Silk.Data.Modelling.Mapping;
using System;

namespace Silk.Data.Modelling
{
	public class ObjectMapper : IObjectMapper
	{
		private static readonly string[] _selfPath = new[] { "." };
		private readonly MappingStore _mappingStore = new MappingStore();
		private readonly object _syncObject = new object();

		public MappingOptions Options { get; }

		public ObjectMapper()
		{
			Options = MappingOptions.DefaultObjectMappingOptions;
		}

		public ObjectMapper(MappingOptions mappingOptions)
		{
			Options = mappingOptions;
		}

		private Mapping.Mapping GetMapping(Type fromType, Type toType)
		{
			var fromModel = TypeModel.GetModelOf(fromType);
			var toModel = TypeModel.GetModelOf(toType);
			if (_mappingStore.TryGetMapping(fromModel, toModel, out var mapping))
				return mapping;

			lock (_syncObject)
			{
				if (_mappingStore.TryGetMapping(fromModel, toModel, out mapping))
					return mapping;

				var mappingBuilder = new MappingBuilder(fromModel, toModel, _mappingStore);
				foreach (var convention in Options.Conventions)
				{
					mappingBuilder.AddConvention(convention);
				}
				return mappingBuilder.BuildMapping();
			}
		}

		public void Inject(object from, object to)
		{
			var mapping = GetMapping(from.GetType(), to.GetType());
			mapping.PerformMapping(
				new ObjectReadWriter(from, mapping.FromModel, from.GetType()),
				new ObjectReadWriter(to, mapping.ToModel, to.GetType())
				);
		}

		public void Inject<TFrom, TTo>(TFrom from, TTo to)
		{
			var mapping = GetMapping(typeof(TFrom), typeof(TTo));
			mapping.PerformMapping(
				new ObjectReadWriter(from, mapping.FromModel, typeof(TFrom)),
				new ObjectReadWriter(to, mapping.ToModel, typeof(TTo))
				);
		}

		public TTo Map<TTo>(object from)
		{
			var mapping = GetMapping(from.GetType(), typeof(TTo));
			var toModel = TypeModel.GetModelOf<TTo>();
			var toWriter = new ObjectReadWriter(null, toModel, typeof(TTo));
			mapping.PerformMapping(
				new ObjectReadWriter(from, mapping.FromModel, from.GetType()),
				toWriter
				);
			return toWriter.ReadField<TTo>(_selfPath, 0);
		}

		public TTo Map<TTo, TFrom>(TFrom from)
		{
			var mapping = GetMapping(typeof(TFrom), typeof(TTo));
			var toModel = TypeModel.GetModelOf<TTo>();
			var toWriter = new ObjectReadWriter(null, toModel, typeof(TTo));
			mapping.PerformMapping(
				new ObjectReadWriter(from, mapping.FromModel, typeof(TFrom)),
				toWriter
				);
			return toWriter.ReadField<TTo>(_selfPath, 0);
		}
	}
}
