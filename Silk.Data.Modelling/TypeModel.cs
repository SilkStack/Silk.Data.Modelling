using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data model structure of a CLR type.
	/// </summary>
	public abstract partial class TypeModel : IModel<PropertyInfoField>
	{
		public abstract IReadOnlyList<PropertyInfoField> Fields { get; }

		IReadOnlyList<IField> IModel.Fields => Fields;
	}

	/// <summary>
	/// Data model structure of a CLR type.
	/// </summary>
	public class TypeModel<T> : TypeModel
	{
		private readonly IReadOnlyList<PropertyInfoField> _fields;

		public override IReadOnlyList<PropertyInfoField> Fields => _fields;

		public TypeModel(IReadOnlyList<PropertyInfoField> fields)
		{
			_fields = fields;
		}
	}
}
