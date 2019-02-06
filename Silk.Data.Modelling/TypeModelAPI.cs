using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling
{
	public partial class TypeModel
	{
		private static readonly Dictionary<Type, TypeModel> _builtModels
			= new Dictionary<Type, TypeModel>();
		private static readonly TypeModelFactory _factory = new TypeModelFactory();

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

		public static TypeModel GetModelOf(Type type)
			=> GetOrCreateModel(type);

		public static TypeModel<T> GetModelOf<T>()
			=> GetOrCreateModel(typeof(T)) as TypeModel<T>;

		private class TypeModelFactory
		{
			private static readonly MethodInfo _createModelMethod = typeof(TypeModelFactory)
				.GetMethod(nameof(BuildTypeModelGeneric), BindingFlags.Instance | BindingFlags.NonPublic);

			public TypeModel BuildTypeModel(Type type)
				=> _createModelMethod.MakeGenericMethod(type).Invoke(this, null) as TypeModel;

			private TypeModel<T> BuildTypeModelGeneric<T>()
			{
				var fields = GetPropertyFields(typeof(T));
				return new TypeModel<T>(fields);
			}

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

			private IEnumerable<PropertyInfo> GetProperties(Type type)
			{
				return type.GetRuntimeProperties();
			}
		}
	}
}
