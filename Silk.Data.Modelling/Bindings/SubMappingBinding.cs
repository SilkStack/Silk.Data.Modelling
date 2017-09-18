namespace Silk.Data.Modelling.Bindings
{
	public class SubMappingBinding : ModelBinding
	{
		public override BindingDirection Direction => BindingDirection.ModelToView;

		public SubMappingBinding(string[] modelFieldPath, string[] viewFieldPath)
			: base(modelFieldPath, viewFieldPath)
		{
		}

		public override object ReadFromModel(IModelReadWriter modelReadWriter, MappingContext mappingContext)
		{
			return mappingContext.Resources.Retrieve(modelReadWriter, $"subMapped:{string.Join(".", ModelFieldPath)}");
		}

		public override void WriteToModel(IModelReadWriter modelReadWriter, object value, MappingContext mappingContext)
		{
			//base.WriteToModel(modelReadWriter,
			//	mappingContext.Resources.Retrieve($"subMapping:{string.Join(".", ModelFieldPath)}"),
			//	mappingContext);
		}
	}
}
