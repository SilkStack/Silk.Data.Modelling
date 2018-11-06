using System;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class UseFactory<TFrom, TTo> : MappingBinding
	{
		private readonly Func<TFrom, TTo> _factory;

		public UseFactory(Func<TFrom, TTo> factory, IFieldReference from, IFieldReference to) :
			base(from, to)
		{
			_factory = factory;
		}

		public TTo CreateInstance(TFrom from)
		{
			return _factory(from);
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(To, CreateInstance(from.ReadField<TFrom>(From)));
		}
	}

	public class UseFactoryBinding<TFrom, TTo> : IMappingBindingFactory<Func<TFrom, TTo>>
	{
		public MappingBinding CreateBinding<TFrom1, TTo1>(IFieldReference fromField, IFieldReference toField, Func<TFrom, TTo> bindingOption)
		{
			return new UseFactory<TFrom1, TTo1>(bindingOption as Func<TFrom1, TTo1>, fromField, toField);
		}
	}
}
