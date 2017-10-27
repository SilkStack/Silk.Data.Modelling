using Silk.Data.Modelling.Bindings;
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
		/// Gets or sets the name of the field on the original model.
		/// </summary>
		public string ModelFieldName { get; set; }

		/// <summary>
		/// Gets or sets the data type of the field.
		/// </summary>
		public Type DataType { get; set; }

		/// <summary>
		/// Gets the list of metadata for the field.
		/// </summary>
		public List<object> Metadata { get; } = new List<object>();

		/// <summary>
		/// Gets or sets the binding for the view field.
		/// </summary>
		public ModelBinding ModelBinding { get; set; }

		public ViewFieldDefinition(string name, ModelBinding binding,
			string modelFieldName = null)
		{
			Name = name;
			ModelBinding = binding;
			ModelFieldName = modelFieldName;
		}
	}
}
