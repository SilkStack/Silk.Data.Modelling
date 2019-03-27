using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Silk.Data.Modelling
{
	public class ObjectMapper : IObjectMapper
	{
		private readonly ITypeInstanceFactory _typeInstanceFactory;
		private readonly IReaderWriterFactory<TypeModel, PropertyInfoField> _readerWriterFactory;
		private readonly IIntersectionAnalyzer<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> _intersectionAnalyzer;
		private readonly IMappingFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> _mappingFactory;
		private readonly Dictionary<Type, FromTypeMappings> _fromTypeMappings = new Dictionary<Type, FromTypeMappings>();

		public ObjectMapper(
			ITypeInstanceFactory typeInstanceFactory = null,
			IReaderWriterFactory<TypeModel, PropertyInfoField> readerWriterFactory = null,
			IIntersectionAnalyzer<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> intersectionAnalyzer = null,
			IMappingFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> mappingFactory = null
			)
		{
			_typeInstanceFactory = typeInstanceFactory ?? new DefaultTypeInstanceFactory();
			_readerWriterFactory = readerWriterFactory ?? new DefaultReaderWriterFactory();
			_intersectionAnalyzer = intersectionAnalyzer ?? new TypeToTypeIntersectionAnalyzer();
			_mappingFactory = mappingFactory ?? new TypeToTypeMappingFactory();
		}

		private IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> GetMapping<TFrom, TTo>()
			where TTo : class
		{
			var fromType = typeof(TFrom);
			if (_fromTypeMappings.TryGetValue(fromType, out var fromMappings))
				return fromMappings.GetMapping<TTo>();

			lock(_fromTypeMappings)
			{
				if (_fromTypeMappings.TryGetValue(fromType, out fromMappings))
					return fromMappings.GetMapping<TTo>();

				fromMappings = new FromTypeMappings<TFrom>(_intersectionAnalyzer, _mappingFactory);
				_fromTypeMappings.Add(fromType, fromMappings);
			}

			return fromMappings.GetMapping<TTo>();
		}

		public void Inject<TFrom, TTo>(TFrom from, TTo to)
			where TTo : class
		{
			var reader = _readerWriterFactory.CreateGraphReader<TFrom>(from);
			var writer = _readerWriterFactory.CreateGraphWriter<TTo>(to);
			var mapping = GetMapping<TFrom, TTo>();
			mapping.Map(reader, writer);
		}

		public void InjectAll<TFrom, TTo>(IEnumerable<TFrom> from, IEnumerable<TTo> to)
			where TTo : class
		{
			var mapping = GetMapping<TFrom, TTo>();

			using (var fromEnumerator = from.GetEnumerator())
			using (var toEnumerator = to.GetEnumerator())
			{
				while (fromEnumerator.MoveNext() && toEnumerator.MoveNext())
				{
					var reader = _readerWriterFactory.CreateGraphReader<TFrom>(fromEnumerator.Current);
					var writer = _readerWriterFactory.CreateGraphWriter<TTo>(toEnumerator.Current);
					mapping.Map(reader, writer);
				}
			}
		}

		public TTo Map<TFrom, TTo>(TFrom from)
			where TTo : class
		{
			var graph = _typeInstanceFactory.CreateInstance<TTo>();
			Inject(from, graph);
			return graph;
		}

		public IEnumerable<TTo> MapAll<TFrom, TTo>(IEnumerable<TFrom> from)
			where TTo : class
		{
			foreach (var obj in from)
				yield return Map<TFrom, TTo>(obj);
		}

		private abstract class FromTypeMappings
		{
			public abstract IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> GetMapping<TTo>();
		}

		private class FromTypeMappings<TFrom> : FromTypeMappings
		{
			private readonly TypeModel<TFrom> _fromModel = TypeModel.GetModelOf<TFrom>();
			private readonly Dictionary<Type, IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>> _mappings
				= new Dictionary<Type, IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>>();
			private readonly IIntersectionAnalyzer<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> _intersectionAnalyzer;
			private readonly IMappingFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> _mappingFactory;

			public FromTypeMappings(
				IIntersectionAnalyzer<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> intersectionAnalyzer,
				IMappingFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> mappingFactory
				)
			{
				_intersectionAnalyzer = intersectionAnalyzer;
				_mappingFactory = mappingFactory;
			}

			public override IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> GetMapping<TTo>()
			{
				var type = typeof(TTo);
				if (_mappings.TryGetValue(type, out var mapping))
					return mapping;

				lock (_mappings)
				{
					if (_mappings.TryGetValue(type, out mapping))
						return mapping;

					mapping = CreateMapping<TTo>();
					_mappings.Add(type, mapping);
					return mapping;
				}
			}

			private IMapping<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField> CreateMapping<TTo>()
			{
				var intersection = _intersectionAnalyzer.CreateIntersection(
					_fromModel,
					TypeModel.GetModelOf<TTo>()
					);
				return _mappingFactory.CreateMapping(intersection);
			}
		}

		private class DefaultReaderWriterFactory : IReaderWriterFactory<TypeModel, PropertyInfoField>
		{
			public IGraphReader<TypeModel, PropertyInfoField> CreateGraphReader<T>(T graph)
				=> new ObjectGraphReader<T>(graph);

			public IGraphWriter<TypeModel, PropertyInfoField> CreateGraphWriter<T>(T graph)
				where T : class
				=> new ObjectGraphReaderWriter<T>(graph);
		}

		private class DefaultTypeInstanceFactory : ITypeInstanceFactory
		{
			public T CreateInstance<T>()
			{
				var factory = GetFactory<T>();
				return factory();
			}

			private static readonly Dictionary<Type, Delegate> _factories
				= new Dictionary<Type, Delegate>();

			private static ConstructorInfo GetParameterlessConstructor(Type type)
			{
				return type
					.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.FirstOrDefault(ctor => ctor.GetParameters().Length == 0);
			}

			private static Func<T> GetFactory<T>()
			{
				var type = typeof(T);
				if (_factories.TryGetValue(type, out var factory))
					return factory as Func<T>;

				lock (_factories)
				{
					if (_factories.TryGetValue(type, out factory))
						return factory as Func<T>;

					factory = CreateFactory<T>();
					_factories.Add(type, factory);
					return factory as Func<T>;
				}
			}

			private static Func<T> CreateFactory<T>()
			{
				var ctor = GetParameterlessConstructor(typeof(T));
				if (ctor == null)
					throw new InvalidOperationException($"{typeof(T).FullName} doesn't have a parameterless constructor.");

				var lambda = Expression.Lambda<Func<T>>(
					Expression.New(ctor)
					);
				return lambda.Compile();
			}
		}
	}
}
