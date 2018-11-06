using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class UseFactory<TFrom, TTo> : AssignmentBinding
	{
		private readonly Func<TFrom, TTo> _factory;

		public UseFactory(Func<TFrom, TTo> factory, IFieldReference to) :
			base(to)
		{
			_factory = factory;
		}

		public TTo CreateInstance(TFrom from)
		{
			return _factory(from);
		}

		public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(To, CreateInstance(from.ReadField<TFrom>(new[] { "." })));
		}
	}

	public class UseFactoryBinding<TFrom, TTo> : IAssignmentBindingFactory<Func<TFrom, TTo>>
	{
		public AssignmentBinding CreateBinding<T>(IFieldReference toField, Func<TFrom, TTo> bindingOption)
		{
			return new UseFactory<TFrom, TTo>(bindingOption, toField);
		}
	}
}
