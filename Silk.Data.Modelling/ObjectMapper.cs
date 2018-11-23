using Silk.Data.Modelling.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	public class ObjectMapper : IObjectMapper
	{
		private readonly object _syncObject = new object();

		public MappingStore MappingStore { get; }
		public MappingOptions Options { get; }

		public ObjectMapper(MappingOptions mappingOptions = null, MappingStore mappingStore = null)
		{
			Options = mappingOptions ?? MappingOptions.DefaultObjectMappingOptions;
			MappingStore = mappingStore ?? new MappingStore();
		}

		private Mapping.Mapping GetMapping(Type fromType, Type toType)
		{
			var fromModel = TypeModel.GetModelOf(fromType);
			var toModel = TypeModel.GetModelOf(toType);
			if (MappingStore.TryGetMapping(fromModel, toModel, out var mapping))
				return mapping;

			lock (_syncObject)
			{
				if (MappingStore.TryGetMapping(fromModel, toModel, out mapping))
					return mapping;

				var mappingBuilder = new MappingBuilder(fromModel, toModel, Options, MappingStore);
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
			return toWriter.ReadField<TTo>(toModel.Root);
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
			return toWriter.ReadField<TTo>(toModel.Root);
		}

		public IEnumerable<TTo> MapAll<TTo>(IEnumerable from)
		{
			foreach (var obj in from)
			{
				yield return Map<TTo>(obj);
			}
		}

		public IEnumerable<TTo> MapAll<TTo, TFrom>(IEnumerable<TFrom> from)
		{
			foreach (var obj in from)
			{
				yield return Map<TTo, TFrom>(obj);
			}
		}

		public void InjectAll(IEnumerable from, ICollection to)
		{
			var fromEnumerator = from.GetEnumerator();
			var toEnumerator = to.GetEnumerator();
			try
			{
				while (fromEnumerator.MoveNext() &&
					toEnumerator.MoveNext())
				{
					Inject(fromEnumerator.Current, toEnumerator.Current);
				}
			}
			finally
			{
				if (fromEnumerator is IDisposable fromDisposable)
					fromDisposable.Dispose();
				if (toEnumerator is IDisposable toDisposable)
					toDisposable.Dispose();
			}
		}

		public void InjectAll<TFrom, TTo>(IEnumerable<TFrom> from, ICollection<TTo> to)
		{
			using (var fromEnumerator = from.GetEnumerator())
			using (var toEnumerator = to.GetEnumerator())
			{
				while (fromEnumerator.MoveNext() &&
					toEnumerator.MoveNext())
				{
					Inject<TFrom, TTo>(fromEnumerator.Current, toEnumerator.Current);
				}
			}
		}
	}
}
