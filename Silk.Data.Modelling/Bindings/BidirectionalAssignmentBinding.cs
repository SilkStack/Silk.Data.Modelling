using System;
using System.Linq;

namespace Silk.Data.Modelling.Bindings
{
	/// <summary>
	/// Copies values/references from models to views and vice-versa.
	/// </summary>
	public class BidirectionalAssignmentBinding : ModelBinding
	{
		public override BindingDirection Direction => BindingDirection.Bidirectional;

		public BidirectionalAssignmentBinding(string[] modelFieldPath, string[] viewFieldPath)
			: base(modelFieldPath, viewFieldPath)
		{
		}
	}
}
