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
		/// Gets the object the modelreadwriter is reading to and writing from.
		/// </summary>
		object Value { get; }

		/// <summary>
		/// Gets a modelreadwriter for the provided field.
		/// </summary>
		/// <param name="modelField"></param>
		/// <returns></returns>
		IModelReadWriter GetField(ModelField modelField);
	}
}
