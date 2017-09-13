namespace Silk.Data.Modelling
{
	/// <summary>
	/// A field member of a model.
	/// </summary>
	public class ModelField
	{
		/// <summary>
		/// Gets the model the field is present on.
		/// </summary>
		public Model ParentModel { get; internal set; }

		/// <summary>
		/// Gets a value indicating if the field is readable.
		/// </summary>
		public bool CanRead { get; }

		/// <summary>
		/// Gets a value indicating if the field is writeable.
		/// </summary>
		public bool CanWrite { get; }

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		public string Name { get; }

		public ModelField(string name, bool canRead, bool canWrite)
		{
			Name = name;
			CanRead = canRead;
			CanWrite = canWrite;
		}
	}
}
