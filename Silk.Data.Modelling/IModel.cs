using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data structure model.
	/// </summary>
	public interface IModel
	{
		/// <summary>
		/// Gets a collection of fields present in the data structure.
		/// </summary>
		IReadOnlyList<IField> Fields { get; }
	}

	/// <summary>
	/// Data structure model.
	/// </summary>
	public interface IModel<TField> : IModel
		where TField : IField
	{
		/// <summary>
		/// Gets a collection of fields present in the data structure.
		/// </summary>
		new IReadOnlyList<TField> Fields { get; }
	}
}
