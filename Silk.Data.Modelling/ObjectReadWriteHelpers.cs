using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Silk.Data.Modelling
{
	internal abstract class ObjectReadWriteHelpers
	{
		private static readonly Dictionary<Type, ObjectReadWriteHelpers> _cache
			= new Dictionary<Type, ObjectReadWriteHelpers>();
		private static readonly MethodInfo _createForTypeMethod = typeof(ObjectReadWriteHelpers).GetTypeInfo()
			.GetDeclaredMethod(nameof(CreateForType));

		public static ObjectReadWriteHelpers GetForType(Type type)
		{
			if (_cache.TryGetValue(type, out var readWriteHelpers))
				return readWriteHelpers;

			lock (_cache)
			{
				if (_cache.TryGetValue(type, out readWriteHelpers))
					return readWriteHelpers;

				readWriteHelpers = _createForTypeMethod.MakeGenericMethod(type).Invoke(null, null)
					as ObjectReadWriteHelpers;
				_cache.Add(type, readWriteHelpers);
				return readWriteHelpers;
			}
		}

		private static ObjectReadWriteHelpers CreateForType<T>()
		{
			var type = typeof(T);
			var typeInfo = type.GetTypeInfo();

			var properties = new Dictionary<string, ObjectProperty>();

			foreach (var property in GetProperties(type))
			{
				Delegate setter = null;
				Delegate getter = null;
				if (property.CanWrite && !property.SetMethod.IsStatic && property.GetIndexParameters().Length == 0)
				{
					setter = CreateWriteDelegate<T>(property);
				}
				if (property.CanRead && !property.GetMethod.IsStatic && property.GetIndexParameters().Length == 0)
				{
					getter = CreateReadDelegate<T>(property);
				}

				properties[property.Name] = Activator.CreateInstance(
					typeof(ObjectProperty<,>).MakeGenericType(type, property.PropertyType),
					new object[] { getter, setter }
					) as ObjectProperty;
			}

			return new ObjectReadWriteHelpers<T>(properties);
		}

		private static Delegate CreateReadDelegate<T>(PropertyInfo property)
		{
			var method = new DynamicMethod("Getter", property.PropertyType, new[] { typeof(T) }, true);
			var ilgen = method.GetILGenerator();
			ilgen.Emit(OpCodes.Ldarg_0);
			ilgen.Emit(OpCodes.Callvirt, property.GetMethod);
			ilgen.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType));
		}

		private static Delegate CreateWriteDelegate<T>(PropertyInfo property)
		{
			var method = new DynamicMethod("Setter", typeof(void), new[] { typeof(T), property.PropertyType }, true);
			var ilgen = method.GetILGenerator();
			ilgen.Emit(OpCodes.Ldarg_0);
			ilgen.Emit(OpCodes.Ldarg_1);
			ilgen.Emit(OpCodes.Callvirt, property.SetMethod);
			ilgen.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(Action<,>).MakeGenericType(typeof(T), property.PropertyType));
		}

		private static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.BaseType == null)
				return typeInfo.DeclaredProperties;
			return typeInfo.DeclaredProperties.Concat(GetProperties(typeInfo.BaseType));
		}

		public abstract void SetTypedValue<T>(object instance, string propertyName, T value);

		public abstract T GetTypedValue<T>(object instance, string propertyName);

		public abstract object GetValue(object instance, string propertyName);

		public abstract class ObjectProperty
		{
			public abstract object GetValue(object instance);
		}

		public class ObjectProperty<T, TValue> : ObjectProperty
		{
			private readonly Func<T, TValue> _getter;
			private readonly Action<T, TValue> _setter;

			public ObjectProperty(Delegate getter, Delegate setter)
			{
				if (getter != null)
					_getter = (Func<T, TValue>)getter;
				if (setter != null)
					_setter = (Action<T, TValue>)setter;
			}

			public void SetTypedValue(object instance, TValue value)
			{
				_setter((T)instance, value);
			}

			public TValue GetTypedValue(object instance)
			{
				return _getter((T)instance);
			}

			public override object GetValue(object instance)
			{
				return GetTypedValue(instance);
			}
		}
	}

	internal class ObjectReadWriteHelpers<T> : ObjectReadWriteHelpers
	{
		private readonly Dictionary<string, ObjectProperty> _properties;

		public ObjectReadWriteHelpers(Dictionary<string, ObjectProperty> properties)
		{
			_properties = properties;
		}

		public override void SetTypedValue<TValue>(object instance, string propertyName, TValue value)
		{
			((ObjectProperty<T, TValue>)_properties[propertyName]).SetTypedValue(instance, value);
		}

		public override TValue GetTypedValue<TValue>(object instance, string propertyName)
		{
			return ((ObjectProperty<T, TValue>)_properties[propertyName]).GetTypedValue(instance);
		}

		public override object GetValue(object instance, string propertyName)
		{
			return _properties[propertyName].GetValue(instance);
		}
	}
}