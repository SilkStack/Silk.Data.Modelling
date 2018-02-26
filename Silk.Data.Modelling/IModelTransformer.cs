namespace Silk.Data.Modelling
{
	/// <summary>
	/// Handles transforming model types.
	/// </summary>
	public interface IModelTransformer
	{
		void VisitModel<TField>(IModel<TField> model)
			where TField : IField;

		void VisitField<T>(IField<T> field);
	}
}
