using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling
{
	internal static class ReflectionHelper
	{
		private readonly static object _typeHelpersLock = new object();
		private readonly static Dictionary<Type, TypeHelper> _typeHelperCache
			= new Dictionary<Type, TypeHelper>();

		public static PropertyHelper GetProperty(Type type, string propertyName)
		{
			if (_typeHelperCache.TryGetValue(type, out var typeHelper))
				return typeHelper.GetProperty(propertyName);

			lock (_typeHelpersLock)
			{
				if (_typeHelperCache.TryGetValue(type, out typeHelper))
					return typeHelper.GetProperty(propertyName);

				typeHelper = new TypeHelper(type);
				_typeHelperCache.Add(type, typeHelper);
				return typeHelper.GetProperty(propertyName);
			}
		}

		public class TypeHelper
		{
			private readonly Type _type;
			private readonly object _propertyLock = new object();
			private readonly Dictionary<string, PropertyHelper> _properties
				= new Dictionary<string, PropertyHelper>();

			public TypeHelper(Type type)
			{
				_type = type;
			}

			public PropertyHelper GetProperty(string propertyName)
			{
				if (_properties.TryGetValue(propertyName, out var propertyHelper))
					return propertyHelper;

				lock (_propertyLock)
				{
					if (_properties.TryGetValue(propertyName, out propertyHelper))
						return propertyHelper;

					var property = CreateProperty(propertyName);
					if (property == null)
						return null;

					_properties.Add(propertyName, property);
					return property;
				}
			}

			private PropertyHelper CreateProperty(string propertyName)
			{
				var propertyInfo = FindTypeProperty(propertyName);
				if (propertyInfo == null)
					return null;
				return new PropertyHelper(_type, propertyInfo);
			}

			private PropertyInfo FindTypeProperty(string propertyName)
			{
				return GetProperties(_type).FirstOrDefault(q => q.Name == propertyName);
			}

			private static IEnumerable<PropertyInfo> GetProperties(Type type)
			{
				var typeInfo = type.GetTypeInfo();
				if (typeInfo.BaseType == null)
					return typeInfo.DeclaredProperties;
				return typeInfo.DeclaredProperties.Concat(GetProperties(typeInfo.BaseType));
			}
		}

		public class PropertyHelper
		{
			public Type MemberOfType { get; }
			public PropertyInfo PropertyInfo { get; }
			public Type PropertyType => PropertyInfo.PropertyType;
			public bool CanRead { get; }
			public bool CanWrite { get; }

			public PropertyHelper(Type memberOfType, PropertyInfo propertyInfo)
			{
				MemberOfType = memberOfType;
				PropertyInfo = propertyInfo;
				CanRead = propertyInfo.CanRead;
				CanWrite = propertyInfo.CanWrite;
			}

			public void SetValue(object instance, object value)
			{
				//  todo: replace reflection with cached expressions
				PropertyInfo.SetValue(instance, value);
			}

			public object GetValue(object instance)
			{
				//  todo: replace reflection with cached expressions
				return PropertyInfo.GetValue(instance);
			}
		}
	}
}
