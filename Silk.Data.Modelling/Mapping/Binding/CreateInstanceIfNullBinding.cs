using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CreateInstanceIfNull<T> : AssignmentBinding
	{
		private readonly Func<T> _createInstance;

		public CreateInstanceIfNull(ConstructorInfo constructorInfo, IFieldReference to) :
			base(to)
		{
			_createInstance = CreateCtorMethod(constructorInfo);
		}

		private Func<T> CreateCtorMethod(ConstructorInfo constructorInfo)
		{
			var lambda = Expression.Lambda<Func<T>>(
				Expression.New(constructorInfo)
				);
			return lambda.Compile();
		}

		public T CreateInstance()
		{
			return _createInstance();
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var nullCheck = to.ReadField<T>(To);
			if (nullCheck == null)
			{
				to.WriteField<T>(To, _createInstance());
			}
		}
	}

	public class CreateInstanceIfNull : IAssignmentBindingFactory<ConstructorInfo>
	{
		public AssignmentBinding CreateBinding<TTo>(IFieldReference toField, ConstructorInfo bindingOption)
		{
			return new CreateInstanceIfNull<TTo>(bindingOption, toField);
		}
	}
}
