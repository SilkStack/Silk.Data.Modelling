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

		/// <summary>
		/// Gets an array of metadata on the field.
		/// </summary>
		public object[] Metadata { get; }

		public ModelField(string name, bool canRead, bool canWrite,
			object[] metadata)
		{
			Name = name;
			CanRead = canRead;
			CanWrite = canWrite;
			Metadata = metadata;
		}
	}
}
