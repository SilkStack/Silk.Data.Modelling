using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Mapping
{
	public abstract class ObjectGraphPropertyAccessor
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

	public class ObjectGraphPropertyAccessor<TGraph> : ObjectGraphPropertyAccessor
	{
		private readonly Dictionary<string, Delegate> _propertyReaders
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _propertyWriters
			= new Dictionary<string, Delegate>();

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
