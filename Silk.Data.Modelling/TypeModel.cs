using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A model of a <see cref="Type"/>.
	/// </summary>
	public abstract partial class TypeModel : ModelBase<IPropertyField>
	{
		public abstract Type Type { get; }
	}

	/// <summary>
	/// A model of a <see cref="Type"/>.
	/// </summary>
	public class TypeModel<T> : TypeModel
	{
		public override Type Type { get; } = typeof(T);

		public override IPropertyField[] Fields { get; }

		internal TypeModel(IPropertyField[] fields)
		{
			Fields = fields;
		}
	}
}
