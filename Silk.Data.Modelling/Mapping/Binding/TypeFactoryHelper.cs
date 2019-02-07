using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	/// <summary>
	/// Helper class for creating and caching parameterless type factories.
	/// </summary>
	public static class TypeFactoryHelper
	{
		private readonly static Dictionary<Type, Delegate> _cache
			= new Dictionary<Type, Delegate>();
		private readonly static object _lock = new object();

		/// <summary>
		/// Get a parameterless factory for creating instances of the provided type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Func<T> GetFactory<T>()
		{
			var type = typeof(T);
			if (_cache.TryGetValue(type, out var @delegate))
				return @delegate as Func<T>;
			lock (_lock)
			{
				if (_cache.TryGetValue(type, out @delegate))
					return @delegate as Func<T>;

				@delegate = CreateFactory<T>();
				_cache.Add(type, @delegate);
				return @delegate as Func<T>;
			}
		}

		private static Func<T> CreateFactory<T>()
		{
			var ctor = GetParameterlessConstructor<T>();
			if (ctor == null)
				throw new InvalidOperationException($"{typeof(T)} has no parameterless constructor.");

			var lambda = Expression.Lambda<Func<T>>(
				Expression.New(ctor)
				);
			return lambda.Compile();
		}

		private static ConstructorInfo GetParameterlessConstructor<T>()
		{
			return typeof(T)
				.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.FirstOrDefault(ctor => ctor.GetParameters().Length == 0);
		}
	}
}
