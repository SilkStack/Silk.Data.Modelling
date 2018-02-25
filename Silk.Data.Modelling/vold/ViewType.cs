using System;

namespace Silk.Data.Modelling
{
	[Flags]
	public enum ViewType
	{
		None = 0,
		/// <summary>
		/// View is built against a specified model.
		/// </summary>
		ModelDriven = 1,
		/// <summary>
		/// Fields on the view are entirely derived from conventions, no model was used.
		/// </summary>
		ConventionDerived = 2,
		All = 3
	}
}
