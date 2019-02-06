using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling
{
	public partial class TypeModel
	{
		/// <summary>
		/// Collection of built models indexed by the Type they model.
		/// </summary>
		private static readonly Dictionary<Type, TypeModel> _builtModels
			= new Dictionary<Type, TypeModel>();
		/// <summary>
		/// TypeModel factory instance and lock object.
		/// </summary>
		private static readonly TypeModelFactory _factory = new TypeModelFactory();

		/// <summary>
		/// Gets a TypeModel from the collection of built models or creates one if needed.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static TypeModel GetOrCreateModel(Type type)
		{
			if (_builtModels.TryGetValue(type, out var model))
				return model;
			lock (_factory)
			{
				if (_builtModels.TryGetValue(type, out model))
					return model;

				model = _factory.BuildTypeModel(type);
				_builtModels.Add(type, model);
				return model;
			}
		}

		/// <summary>
		/// Gets the TypeModel of the provided Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypeModel GetModelOf(Type type)
			=> GetOrCreateModel(type);

		/// <summary>
		/// Gets the TypeModel of the provided generic type parameter.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static TypeModel<T> GetModelOf<T>()
			=> GetOrCreateModel(typeof(T)) as TypeModel<T>;

		/// <summary>
		/// Factory for producing TypeModel instances.
		/// </summary>
		private class TypeModelFactory
		{
			/// <summary>
			/// Cached reference to the generic build model method.
			/// </summary>
			private static readonly MethodInfo _createModelMethod = typeof(TypeModelFactory)
				.GetMethod(nameof(BuildTypeModelGeneric), BindingFlags.Instance | BindingFlags.NonPublic);

			/// <summary>
			/// Build the TypeModel for the provided Type.
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			public TypeModel BuildTypeModel(Type type)
				=> _createModelMethod.MakeGenericMethod(type).Invoke(this, null) as TypeModel;

			/// <summary>
			/// Build a TypeModel for the provided generic type parameter.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			private TypeModel<T> BuildTypeModelGeneric<T>()
			{
				var fields = GetPropertyFields(typeof(T));
				return new TypeModel<T>(fields);
			}

			/// <summary>
			/// Gets all eligable fields for the type model of the provided Type.
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			private IReadOnlyList<PropertyInfoField> GetPropertyFields(Type type)
			{
				var ret = new List<PropertyInfoField>();
				foreach (var property in GetProperties(type)
						.Where(q => (q.CanRead && !q.GetMethod.IsStatic) ||
							(q.CanWrite && !q.SetMethod.IsStatic)))
				{
					ret.Add(PropertyInfoField.CreateFromPropertyInfo(property));
				}
				return ret.ToArray();
			}

			/// <summary>
			/// Gets candidate properties from the provided Type.
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			private IEnumerable<PropertyInfo> GetProperties(Type type)
			{
				return type.GetRuntimeProperties();
			}
		}
	}
}
