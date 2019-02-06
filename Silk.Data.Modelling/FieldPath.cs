using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A path of fields.
	/// </summary>
	public interface IFieldPath
	{
		/// <summary>
		/// Gets the model the path is built on.
		/// </summary>
		IModel Model { get; }

		/// <summary>
		/// Gets the final field found at the end of the path.
		/// </summary>
		IField FinalField { get; }

		/// <summary>
		/// Gets an array of fields in the path.
		/// </summary>
		IReadOnlyList<IField> Fields { get; }
	}

	public interface IFieldPath<TField> : IFieldPath
		where TField : IField
	{
		/// <summary>
		/// Gets the final field found at the end of the path.
		/// </summary>
		new TField FinalField { get; }

		/// <summary>
		/// Gets an array of fields in the path.
		/// </summary>
		new IReadOnlyList<TField> Fields { get; }
	}

	public interface IFieldPath<TModel, TField> : IFieldPath<TField>
		where TField : IField
		where TModel : IModel<TField>
	{
		/// <summary>
		/// Gets the model the path is built on.
		/// </summary>
		new TModel Model { get; }
	}

	/// <summary>
	/// A path of fields.
	/// </summary>
	/// <typeparam name="TField"></typeparam>
	public class FieldPath<TModel, TField> : IFieldPath<TModel, TField>
		where TModel : IModel<TField>
		where TField : IField
	{
		/// <summary>
		/// Gets the model the path is built on.
		/// </summary>
		public TModel Model { get; }

		/// <summary>
		/// Gets the final field found at the end of the path.
		/// </summary>
		public TField FinalField { get; }

		/// <summary>
		/// Gets an array of fields in the path.
		/// </summary>
		public IReadOnlyList<TField> Fields { get; }

		IModel IFieldPath.Model => Model;

		IField IFieldPath.FinalField => FinalField;

		private readonly IField[] _fields;
		IReadOnlyList<IField> IFieldPath.Fields => _fields;

		public FieldPath(TModel model, TField finalField, TField[] fields)
		{
			Model = model;
			FinalField = finalField;
			Fields = fields ?? new TField[0];
			_fields = fields.OfType<IField>().ToArray();
		}

		/// <summary>
		/// Create a new FieldPath instance with a new field appended to the end of the path.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public FieldPath<TModel, TField> Concat(TField field)
		{
			return new FieldPath<TModel, TField>(
				Model, field,
				Fields.Concat(new[] { field }).ToArray()
				);
		}
	}
}
