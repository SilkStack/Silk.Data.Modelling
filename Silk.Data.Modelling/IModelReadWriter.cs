namespace Silk.Data.Modelling
{
	/// <summary>
	/// Reads and writes fields on a model.
	/// </summary>
	public interface IModelReadWriter
	{
		/// <summary>
		/// Gets the model the modelreadwriter is for.
		/// </summary>
		Model Model { get; }

		/// <summary>
		/// Gets or sets the field's value.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Gets a modelreadwriter for the provided field.
		/// </summary>
		/// <param name="modelField"></param>
		/// <returns></returns>
		IModelReadWriter GetField(ModelField modelField);
	}
}
