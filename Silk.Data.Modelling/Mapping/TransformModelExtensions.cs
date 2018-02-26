namespace Silk.Data.Modelling.Mapping
{
	public static class TransformModelExtensions
	{
		/// <summary>
		/// Transforms the model to a <see cref="SourceModel"/> for use in mapping creation.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static SourceModel TransformToSourceModel(this IModel model, string[] rootPath = null)
		{
			var transformer = new SourceModelTransformer(rootPath);
			model.Transform(transformer);
			return transformer.BuildSourceModel();
		}

		/// <summary>
		/// Transforms the model to a <see cref="TargetModel"/> for use in mapping creation.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static TargetModel TransformToTargetModel(this IModel model, string[] rootPath = null)
		{
			var transformer = new TargetModelTransformer(rootPath);
			model.Transform(transformer);
			return transformer.BuildTargetModel();
		}
	}
}
