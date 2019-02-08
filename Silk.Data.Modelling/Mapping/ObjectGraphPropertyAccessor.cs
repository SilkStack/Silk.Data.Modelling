using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	internal abstract class ObjectGraphPropertyAccessor
	{
		private static readonly Dictionary<Type, ObjectGraphPropertyAccessor> _cache
			= new Dictionary<Type, ObjectGraphPropertyAccessor>();
		private static readonly object _lock = new object();

		public static ObjectGraphPropertyAccessor<TGraph> GetFor<TGraph>()
		{
			var type = typeof(TGraph);
			if (_cache.TryGetValue(type, out var accessor))
				return accessor as ObjectGraphPropertyAccessor<TGraph>;

			lock (_lock)
			{
				if (_cache.TryGetValue(type, out accessor))
					return accessor as ObjectGraphPropertyAccessor<TGraph>;

				accessor = new ObjectGraphPropertyAccessor<TGraph>();
				_cache.Add(type, accessor);
				return accessor as ObjectGraphPropertyAccessor<TGraph>;
			}
		}
	}

	internal class ObjectGraphPropertyAccessor<TGraph> : ObjectGraphPropertyAccessor
	{
		private readonly Dictionary<string, Delegate> _propertyReaders
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _propertyWriters
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _propertyCheckers
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _containerCreators
			= new Dictionary<string, Delegate>();

		public Action<TGraph> GetContainerCreator(IFieldPath<PropertyInfoField> fieldPath)
		{
			var flattenedPath = string.Join(".", fieldPath.Fields.Select(field => field.FieldName));

			if (_containerCreators.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Action<TGraph>;

			lock (_containerCreators)
			{
				if (_containerCreators.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Action<TGraph>;

				@delegate = CreateContainerCreator(fieldPath);
				_containerCreators.Add(flattenedPath, @delegate);
				return @delegate as Action<TGraph>;
			}
		}

		public Func<TGraph, bool> GetPropertyChecker(IFieldPath<PropertyInfoField> fieldPath, bool skipLastField)
		{
			var pathSource = fieldPath.Fields.Select(field => field.FieldName);
			if (skipLastField)
				pathSource = pathSource.Take(fieldPath.Fields.Count - 1);

			var flattenedPath = string.Join(".", pathSource);

			if (_propertyCheckers.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, bool>;

			lock (_propertyCheckers)
			{
				if (_propertyCheckers.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, bool>;

				@delegate = CreatePropertyChecker(fieldPath, skipLastField);
				_propertyCheckers.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, bool>;
			}
		}

		public Func<TGraph, T> GetPropertyReader<T>(IFieldPath<PropertyInfoField> fieldPath)
		{
			var flattenedPath = string.Join(".", fieldPath.Fields.Select(field => field.FieldName));

			if (_propertyReaders.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, T>;

			lock (_propertyReaders)
			{
				if (_propertyReaders.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, T>;

				@delegate = CreatePropertyReader<T>(fieldPath);
				_propertyReaders.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, T>;
			}
		}

		public Action<TGraph, T> GetPropertyWriter<T>(IFieldPath<PropertyInfoField> fieldPath)
		{
			var flattenedPath = string.Join(".", fieldPath.Fields.Select(field => field.FieldName));

			if (_propertyWriters.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Action<TGraph, T>;

			lock (_propertyWriters)
			{
				if (_propertyWriters.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Action<TGraph, T>;

				@delegate = CreatePropertyWriter<T>(fieldPath);
				_propertyWriters.Add(flattenedPath, @delegate);
				return @delegate as Action<TGraph, T>;
			}
		}

		private static ConstructorInfo GetParameterlessConstructor(Type type)
		{
			return type
				.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.FirstOrDefault(ctor => ctor.GetParameters().Length == 0);
		}

		private Action<TGraph> CreateContainerCreator(IFieldPath<PropertyInfoField> fieldPath)
		{
			var ctor = GetParameterlessConstructor(fieldPath.FinalField.FieldDataType);
			if (ctor == null)
				throw new InvalidOperationException($"{fieldPath.FinalField.FieldDataType.FullName} doesn't have a parameterless constructor.");

			var graph = Expression.Parameter(typeof(TGraph), "graph");

			Expression property = graph;

			foreach (var field in fieldPath.Fields)
				property = Expression.Property(property, field.FieldName);

			var lambda = Expression.Lambda<Action<TGraph>>(
				Expression.Assign(property, Expression.New(ctor)), graph
				);
			return lambda.Compile();
		}

		private Func<TGraph, bool> CreatePropertyChecker(IFieldPath<PropertyInfoField> fieldPath, bool skipLastField)
		{
			var graph = Expression.Parameter(typeof(TGraph), "graph");
			var result = Expression.Variable(typeof(bool), "result");

			var ifTree = Expression.IfThen(
				Expression.NotEqual(graph, Expression.Constant(null)),
				NextPropertyBranch(fieldPath.Fields, 1)
				);

			var body = Expression.Block(
				new[] { result },
				Expression.Assign(result, Expression.Constant(false)),
				ifTree,
				result);

			var lambda = Expression.Lambda<Func<TGraph, bool>>(
				body, graph
				);
			return lambda.Compile();

			Expression NextPropertyBranch(IReadOnlyList<IField> fields, int offset)
			{
				if (
					(skipLastField && offset > fields.Count - 1) ||
					offset > fields.Count
					)
					return Expression.Assign(result, Expression.Constant(true));

				Expression property = graph;
				for (var i = 0; i < offset; i++)
				{
					property = Expression.Property(property, fields[i].FieldName);
				}

				return Expression.IfThen(
					Expression.NotEqual(property, Expression.Constant(null)),
					NextPropertyBranch(fieldPath.Fields, offset + 1)
				);
			}
		}

		private Func<TGraph, T> CreatePropertyReader<T>(IFieldPath<PropertyInfoField> fieldPath)
		{
			var graph = Expression.Parameter(typeof(TGraph));
			Expression body = graph;

			foreach (var field in fieldPath.Fields)
				body = Expression.Property(body, field.FieldName);

			var lambda = Expression.Lambda<Func<TGraph, T>>(
				body, graph
				);
			return lambda.Compile();
		}

		private Action<TGraph, T> CreatePropertyWriter<T>(IFieldPath<PropertyInfoField> fieldPath)
		{
			var graph = Expression.Parameter(typeof(TGraph));
			var value = Expression.Parameter(typeof(T));

			Expression property = graph;

			foreach (var field in fieldPath.Fields)
				property = Expression.Property(property, field.FieldName);

			var body = Expression.Assign(
				property,
				value
				);

			var lambda = Expression.Lambda<Action<TGraph, T>>(
				body, graph, value
				);
			return lambda.Compile();
		}
	}
}
