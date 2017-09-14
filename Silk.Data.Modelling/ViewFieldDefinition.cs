using System;
using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Definition from which an IViewField can be constructed.
	/// </summary>
	public class ViewFieldDefinition
	{
		/// <summary>
		/// Gets or sets the name of the field.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the data type of the field.
		/// </summary>
		public Type DataType { get; set; }

		/// <summary>
		/// Gets the list of metadata for the field.
		/// </summary>
		public List<object> Metadata { get; } = new List<object>();

		public ViewFieldDefinition(string name)
		{
			Name = name;
		}
	}
}
