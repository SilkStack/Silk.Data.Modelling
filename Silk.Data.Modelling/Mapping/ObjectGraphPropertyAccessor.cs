using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
		private readonly Dictionary<string, Delegate> _propertyCheckers
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _containerCreators
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _enumerableReaders
			= new Dictionary<string, Delegate>();
		private readonly Dictionary<string, Delegate> _enumerableWriters
			= new Dictionary<string, Delegate>();

		public Action<TGraph, IEnumerable<T>> GetEnumerableWriter<T>(IFieldPath fieldPath, int pathOffset = 0)
		{
			var flattenedPath = string.Join(".", fieldPath.Fields.Select(field => field.FieldName));

			if (_enumerableWriters.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Action<TGraph, IEnumerable<T>>;

			lock (_enumerableWriters)
			{
				if (_enumerableWriters.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Action<TGraph, IEnumerable<T>>;

				@delegate = CreateEnumerableWriter<T>(fieldPath, pathOffset);
				_enumerableWriters.Add(flattenedPath, @delegate);
				return @delegate as Action<TGraph, IEnumerable<T>>;
			}
		}

		public Func<TGraph, IEnumerable<T>> GetEnumerableReader<T>(IFieldPath fieldPath, int pathOffset = 0)
		{
			var flattenedPath = string.Join(".", fieldPath.Fields.Select(field => field.FieldName));

			if (_enumerableReaders.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, IEnumerable<T>>;

			lock (_enumerableReaders)
			{
				if (_enumerableReaders.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, IEnumerable<T>>;

				@delegate = CreateEnumerableReader<T>(fieldPath, pathOffset);
				_enumerableReaders.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, IEnumerable<T>>;
			}
		}

		public Func<TGraph, TGraph> GetContainerCreator(IFieldPath fieldPath, int pathOffset = 0)
		{
			var flattenedPath = string.Join(".", fieldPath.Fields.Select(field => field.FieldName));

			if (_containerCreators.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, TGraph>;

			lock (_containerCreators)
			{
				if (_containerCreators.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, TGraph>;

				@delegate = CreateContainerCreator(fieldPath, pathOffset);
				_containerCreators.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, TGraph>;
			}
		}

		public Func<TGraph, bool> GetPropertyChecker(IFieldPath fieldPath, bool skipLastField, int pathOffset = 0)
		{
			var pathSource = fieldPath.Fields.Select(field => field.FieldName).Skip(pathOffset);
			if (skipLastField)
				pathSource = pathSource.Take(fieldPath.Fields.Count - 1 - pathOffset);

			var flattenedPath = $"{skipLastField}-{string.Join(".", pathSource)}";

			if (_propertyCheckers.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, bool>;

			lock (_propertyCheckers)
			{
				if (_propertyCheckers.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, bool>;

				@delegate = CreatePropertyChecker(fieldPath, skipLastField, pathOffset);
				_propertyCheckers.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, bool>;
			}
		}

		public Func<TGraph, T> GetPropertyReader<T>(IFieldPath fieldPath, int pathOffset = 0)
		{
			var flattenedPath = $"{typeof(T).FullName}::{string.Join(".", fieldPath.Fields.Skip(pathOffset).Select(field => field.FieldName))}";

			if (_propertyReaders.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, T>;

			lock (_propertyReaders)
			{
				if (_propertyReaders.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, T>;

				@delegate = CreatePropertyReader<T>(fieldPath, pathOffset);
				_propertyReaders.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, T>;
			}
		}

		public Func<TGraph, T, TGraph> GetPropertyWriter<T>(IFieldPath fieldPath, int pathOffset = 0)
		{
			var flattenedPath = $"{typeof(T).FullName}::{string.Join(".", fieldPath.Fields.Skip(pathOffset).Select(field => field.FieldName))}";

			if (_propertyWriters.TryGetValue(flattenedPath, out var @delegate))
				return @delegate as Func<TGraph, T, TGraph>;

			lock (_propertyWriters)
			{
				if (_propertyWriters.TryGetValue(flattenedPath, out @delegate))
					return @delegate as Func<TGraph, T, TGraph>;

				@delegate = CreatePropertyWriter<T>(fieldPath, pathOffset);
				_propertyWriters.Add(flattenedPath, @delegate);
				return @delegate as Func<TGraph, T, TGraph>;
			}
		}

		private static ConstructorInfo GetParameterlessConstructor(Type type)
		{
			return type
				.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.FirstOrDefault(ctor => ctor.GetParameters().Length == 0);
		}

		private static T[] ConvertToArray<T>(IEnumerable<T> source)
			=> source.ToArray();

		private static List<T> ConvertToList<T>(IEnumerable<T> source)
			=> source.ToList();

		private Action<TGraph, IEnumerable<T>> CreateEnumerableWriter<T>(IFieldPath fieldPath, int pathOffset)
		{
			var graph = Expression.Parameter(typeof(TGraph));
			var enumerable = Expression.Parameter(typeof(IEnumerable<T>));
			Expression property = graph;

			foreach (var field in fieldPath.Fields.Skip(pathOffset))
				property = Expression.Property(property, field.FieldName);

			MethodInfo convertMethod;
			if (fieldPath.FinalField.FieldDataType.IsArray)
				convertMethod = typeof(ObjectGraphPropertyAccessor<TGraph>).GetMethod("ConvertToArray", BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(fieldPath.FinalField.FieldElementType);
			else
				convertMethod = typeof(ObjectGraphPropertyAccessor<TGraph>).GetMethod("ConvertToList", BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(fieldPath.FinalField.FieldElementType);

			var assignment = Expression.Assign(property, Expression.Call(convertMethod, enumerable));

			var lambda = Expression.Lambda<Action<TGraph, IEnumerable<T>>>(
				assignment, graph, enumerable
				);
			return lambda.Compile();
		}

		private Func<TGraph, IEnumerable<T>> CreateEnumerableReader<T>(IFieldPath fieldPath, int pathOffset)
		{
			var graph = Expression.Parameter(typeof(TGraph));
			Expression body = graph;

			foreach (var field in fieldPath.Fields.Skip(pathOffset))
				body = Expression.Property(body, field.FieldName);

			body = Expression.Convert(body, typeof(IEnumerable<T>));

			var lambda = Expression.Lambda<Func<TGraph, IEnumerable<T>>>(
				body, graph
				);
			return lambda.Compile();
		}

		private Func<TGraph, TGraph> CreateContainerCreator(IFieldPath fieldPath, int pathOffset)
		{
			var ctor = GetParameterlessConstructor(fieldPath.FinalField.RemoveEnumerableType());
			if (ctor == null)
				throw new InvalidOperationException($"{fieldPath.FinalField.FieldDataType.FullName} doesn't have a parameterless constructor.");

			var graph = Expression.Parameter(typeof(TGraph), "graph");

			Expression property = graph;

			foreach (var field in fieldPath.Fields.Skip(pathOffset))
				property = Expression.Property(property, field.FieldName);

			var lambda = Expression.Lambda<Func<TGraph, TGraph>>(
				Expression.Block(
					Expression.Assign(property, Expression.New(ctor)),
					graph
					), graph
				);
			return lambda.Compile();
		}

		private Func<TGraph, bool> CreatePropertyChecker(IFieldPath fieldPath, bool skipLastField, int pathOffset)
		{
			var graph = Expression.Parameter(typeof(TGraph), "graph");
			var result = Expression.Variable(typeof(bool), "result");

			if (skipLastField && fieldPath.Fields.Count - pathOffset < 1)
			{
				var shortLambda = Expression.Lambda<Func<TGraph, bool>>(
					Expression.Constant(true), graph
				);
				return shortLambda.Compile();
			}

			var ifTree = Expression.IfThen(
				Expression.NotEqual(graph, Expression.Constant(null)),
				NextPropertyBranch(fieldPath.Fields.Skip(pathOffset).ToArray(), 1)
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

		private Func<TGraph, T> CreatePropertyReader<T>(IFieldPath fieldPath, int pathOffset)
		{
			var graph = Expression.Parameter(typeof(TGraph));
			Expression body = graph;

			foreach (var field in fieldPath.Fields.Skip(pathOffset))
				body = Expression.Property(body, field.FieldName);

			if (typeof(T) != fieldPath.FinalField.FieldDataType)
				body = Expression.Convert(body, typeof(T));

			var lambda = Expression.Lambda<Func<TGraph, T>>(
				body, graph
				);
			return lambda.Compile();
		}

		private Func<TGraph, T, TGraph> CreatePropertyWriter<T>(IFieldPath fieldPath, int pathOffset)
		{
			var graph = Expression.Parameter(typeof(TGraph));
			var value = Expression.Parameter(typeof(T));

			Expression property = graph;

			foreach (var field in fieldPath.Fields.Skip(pathOffset))
				property = Expression.Property(property, field.FieldName);

			Expression body;
			if (!fieldPath.FinalField.IsEnumerableType && typeof(T) != fieldPath.FinalField.FieldDataType)
				body = Expression.Assign(
					property,
					Expression.Convert(value, fieldPath.FinalField.FieldDataType)
				);
			else
				body = Expression.Assign(
					property,
					value
				);

			body = Expression.Block(
				body,
				graph
				);

			var lambda = Expression.Lambda<Func<TGraph, T, TGraph>>(
				body, graph, value
				);
			return lambda.Compile();
		}
	}
}
