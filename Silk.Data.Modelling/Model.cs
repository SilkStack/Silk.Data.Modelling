using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Represents a data structure.
	/// </summary>
	public class Model
	{
		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the fields present on the model.
		/// </summary>
		public ModelField[] Fields { get; }

		/// <summary>
		/// Gets metadata present on the model.
		/// </summary>
		public object[] Metadata { get; }

		public Model(string name, IEnumerable<ModelField> fields, IEnumerable<object> metadata)
		{
			Name = name;
			Fields = fields.ToArray();
			Metadata = metadata.ToArray();
			foreach (var field in Fields)
				field.ParentModel = this;
		}
	}
}
