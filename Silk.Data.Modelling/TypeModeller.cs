using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Produces strongly typed models of CLR types.
	/// </summary>
	public static class TypeModeller
	{
		private static readonly Dictionary<Type, TypedModel> _typeModelCache = new Dictionary<Type, TypedModel>();

		/// <summary>
		/// Gets the strongly typed model for a type, T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static TypedModel<T> GetModelOf<T>()
		{
			return (TypedModel<T>)GetModelOf(typeof(T));
		}

		/// <summary>
		/// Gets the strongly typed model for a type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypedModel GetModelOf(Type type)
		{
			if (_typeModelCache.TryGetValue(type, out var model))
				return model;

			lock (_typeModelCache)
			{
				if (_typeModelCache.TryGetValue(type, out model))
					return model;

				model = CreateModel(type);
				_typeModelCache.Add(type, model);
				return model;
			}
		}

		private static TypedModel CreateModel(Type type)
		{
			var modelType = typeof(TypedModel<>).MakeGenericType(type);
			return Activator.CreateInstance(modelType, new object[] {
				type.Name, GetModelFields(type), GetModelMetadata(type)
			}) as TypedModel;
		}

		private static IEnumerable<ModelField> GetModelFields(Type type)
		{
			var fieldBaseType = typeof(TypedModelField<>);
			foreach (var property in type.GetProperties()
					.Where(q => (q.CanRead && !q.GetMethod.IsStatic) ||
						(q.CanWrite && !q.SetMethod.IsStatic)))
			{
				var (dataType, enumerableBaseType) = property.PropertyType.GetDataAndEnumerableType();
				var fieldType = fieldBaseType.MakeGenericType(dataType);
				yield return Activator.CreateInstance(fieldType, new object[]
				{
					property.Name,
					property.CanRead,
					property.CanWrite,
					property.GetCustomAttributes(false),
					enumerableBaseType
				}) as ModelField;
			}
		}

		private static IEnumerable<object> GetModelMetadata(Type type)
		{
			return type.GetCustomAttributes(false);
		}
	}
}
