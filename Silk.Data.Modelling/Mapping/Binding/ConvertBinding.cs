namespace Silk.Data.Modelling.Mapping.Binding
{
	public class ConvertBinding<TFrom, TTo> : MappingBinding
	{
		public Converter<TFrom, TTo> Converter { get; }

		public ConvertBinding(Converter<TFrom, TTo> converter, string[] fromPath, string[] toPath)
			: base(fromPath, toPath)
		{
			Converter = converter;
		}

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			to.WriteField<TTo>(ToPath, 0, Converter.Convert(from.ReadField<TFrom>(FromPath, 0)));
		}
	}
}
