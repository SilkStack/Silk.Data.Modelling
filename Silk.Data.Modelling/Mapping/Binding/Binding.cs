namespace Silk.Data.Modelling.Mapping.Binding
{
	public abstract class Binding
	{
		public abstract void PerformBinding(IModelReadWriter from, IModelReadWriter to);
	}

	/// <summary>
	/// Assigns a value in the result mapping graph.
	/// </summary>
	public abstract class AssignmentBinding : Binding
	{
		public string[] ToPath { get; }

		public AssignmentBinding(string[] toPath)
		{
			ToPath = toPath;
		}

		public override void PerformBinding(IModelReadWriter from, IModelReadWriter to)
		{
			AssignBindingValue(from, to);
		}

		public abstract void AssignBindingValue(IModelReadWriter from, IModelReadWriter to);
	}

	/// <summary>
	/// Binds a value between models.
	/// </summary>
	public abstract class MappingBinding : Binding
	{
		public string[] FromPath { get; }

		public string[] ToPath { get; }

		public MappingBinding(string[] fromPath, string[] toPath)
		{
			FromPath = fromPath;
			ToPath = toPath;
		}

		public override void PerformBinding(IModelReadWriter from, IModelReadWriter to)
		{
			CopyBindingValue(from, to);
		}

		/// <summary>
		/// Copies bound value from <see cref="from"/> to <see cref="to"/>.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public abstract void CopyBindingValue(IModelReadWriter from, IModelReadWriter to);
	}
}
