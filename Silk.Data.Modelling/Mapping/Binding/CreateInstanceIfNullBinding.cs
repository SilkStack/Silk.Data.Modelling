using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CreateInstanceIfNull<T> : AssignmentBinding
	{
		private readonly Func<T> _createInstance;

		public CreateInstanceIfNull(ConstructorInfo constructorInfo, string[] toPath) :
			base(toPath)
		{
			_createInstance = CreateCtorMethod(constructorInfo);
		}

		private Func<T> CreateCtorMethod(ConstructorInfo constructorInfo)
		{
			var method = new DynamicMethod("Ctor", typeof(T), new Type[0], true);
			var ilgen = method.GetILGenerator();
			ilgen.Emit(OpCodes.Newobj, constructorInfo);
			ilgen.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(Func<T>)) as Func<T>;
		}

		public T CreateInstance()
		{
			return _createInstance();
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var nullCheck = to.ReadField<T>(ToPath, 0);
			if (nullCheck == null)
			{
				to.WriteField<T>(ToPath, 0, _createInstance());
			}
		}
	}

	public class CreateInstanceIfNull : IAssignmentBindingFactory<ConstructorInfo>
	{
		public AssignmentBinding CreateBinding<TTo>(ITargetField toField, ConstructorInfo bindingOption)
		{
			return new CreateInstanceIfNull<TTo>(bindingOption, toField.FieldPath);
		}
	}
}
