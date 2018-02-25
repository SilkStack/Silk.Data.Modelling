using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling
{
	public partial class TypeModel
	{
		private static readonly Dictionary<Type, TypeModel> _cache
			= new Dictionary<Type, TypeModel>();
		private static readonly object _genModelLock = new object();
		private static readonly MethodInfo _createModelMethod = typeof(TypeModel).GetTypeInfo().GetDeclaredMethod("CreateModelOf");
		private static readonly object[] _noArgs = new object[0];

		public static TypeModel GetModelOf(Type type)
		{
			if (_cache.TryGetValue(type, out var model))
				return model;

			lock(_genModelLock)
			{
				if (_cache.TryGetValue(type, out model))
					return model;

				model = _createModelMethod.MakeGenericMethod(type).Invoke(null, _noArgs) as TypeModel;
				_cache.Add(type, model);

				return model;
			}
		}

		public static TypeModel<T> GetModelOf<T>()
		{
			return GetModelOf(typeof(T)) as TypeModel<T>;
		}

		private static TypeModel<T> CreateModelOf<T>()
		{
			return new TypeModel<T>(GetPropertyFields(typeof(T)));
		}

		private static PropertyField[] GetPropertyFields(Type type)
		{
			var ret = new List<PropertyField>();
			var baseType = typeof(PropertyField<>);
			foreach (var property in GetProperties(type)
					.Where(q => (q.CanRead && !q.GetMethod.IsStatic) ||
						(q.CanWrite && !q.SetMethod.IsStatic)))
			{
				var enumElementType = property.PropertyType.GetEnumerableElementType();
				var propertyType = baseType.MakeGenericType(property.PropertyType);
				ret.Add(
					Activator.CreateInstance(propertyType, new object[] {
						property.Name, property.CanRead, property.CanWrite,
						enumElementType != null, enumElementType
					}) as PropertyField);
			}
			return ret.ToArray();
		}

		private static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.BaseType == null)
				return typeInfo.DeclaredProperties;
			return typeInfo.DeclaredProperties.Concat(GetProperties(typeInfo.BaseType));
		}
	}
}
