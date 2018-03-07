using System;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class EnumerableBinding<TFrom, TTo> : MappingBinding
	{
		public EnumerableBinding(string[] fromPath, string[] toPath, MappingBinding elementBinding) : base(fromPath, toPath)
		{
			ElementBinding = elementBinding;
		}

		public MappingBinding ElementBinding { get; }

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			throw new NotImplementedException();
		}
	}

	public class EnumerableBindingFactory : IMappingBindingFactory<IMappingBindingFactory>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, IMappingBindingFactory bindingOption)
		{
			var elementBinding = typeof(IMappingBindingFactory).GetTypeInfo()
				.GetDeclaredMethod(nameof(IMappingBindingFactory.CreateBinding))
				.MakeGenericMethod(fromField.ElementType, toField.ElementType)
				.Invoke(bindingOption, new object[] { fromField, toField })
				as MappingBinding;
			return new EnumerableBinding<TFrom, TTo>(fromField.FieldPath, toField.FieldPath, elementBinding);
		}
	}
}
