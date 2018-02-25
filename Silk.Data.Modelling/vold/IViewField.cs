using Silk.Data.Modelling.Bindings;
using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A field on a view.
	/// </summary>
	public interface IViewField
	{
		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the data type of the field.
		/// </summary>
		Type DataType { get; }

		/// <summary>
		/// Gets an array of metadata for the field.
		/// </summary>
		object[] Metadata { get; }

		/// <summary>
		/// Gets the binding for the view field.
		/// </summary>
		ModelBinding ModelBinding { get; }
	}
}
