using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A model of a <see cref="Type"/>.
	/// </summary>
	public abstract partial class TypeModel : IModel<PropertyField>
	{
		public abstract Type Type { get; }
		public abstract PropertyField[] Fields { get; }

		IField[] IModel.Fields => Fields;
	}

	/// <summary>
	/// A model of a <see cref="Type"/>.
	/// </summary>
	public class TypeModel<T> : TypeModel
	{
		public override Type Type { get; } = typeof(T);

		public override PropertyField[] Fields { get; }

		internal TypeModel(PropertyField[] fields)
		{
			Fields = fields;
		}
	}
}
