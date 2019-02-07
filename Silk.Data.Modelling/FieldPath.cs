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
		/// Gets a value indicating if the field path has a parent.
		/// </summary>
		bool HasParent { get; }

		/// <summary>
		/// Gets the field path's parent.
		/// </summary>
		IFieldPath Parent { get; }

		/// <summary>
		/// Gets the model the path is built on.
		/// </summary>
		IModel Model { get; }

		/// <summary>
		/// Gets the final field found at the end of the path.
		/// </summary>
		IField FinalField { get; }

		/// <summary>
		/// Gets a collection of fields in the path.
		/// </summary>
		IReadOnlyList<IField> Fields { get; }
	}

	public interface IFieldPath<TField> : IFieldPath
		where TField : IField
	{
		/// <summary>
		/// Gets the field path's parent.
		/// </summary>
		new IFieldPath<TField> Parent { get; }

		/// <summary>
		/// Gets the final field found at the end of the path.
		/// </summary>
		new TField FinalField { get; }

		/// <summary>
		/// Gets a collection of fields in the path.
		/// </summary>
		new IReadOnlyList<TField> Fields { get; }
	}

	public interface IFieldPath<TModel, TField> : IFieldPath<TField>
		where TField : IField
		where TModel : IModel<TField>
	{
		/// <summary>
		/// Gets the field path's parent.
		/// </summary>
		new IFieldPath<TModel, TField> Parent { get; }

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
		where TField : class, IField
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
		/// Gets a collection of fields in the path.
		/// </summary>
		public IReadOnlyList<TField> Fields { get; }

		IModel IFieldPath.Model => Model;

		IField IFieldPath.FinalField => FinalField;

		IReadOnlyList<IField> IFieldPath.Fields => Fields;

		private FieldPath<TModel, TField> _parent;

		public bool HasParent => _parent != null;

		public IFieldPath<TModel, TField> Parent => _parent;

		IFieldPath<TField> IFieldPath<TField>.Parent => _parent;

		IFieldPath IFieldPath.Parent => _parent;

		public FieldPath(TModel model, TField finalField, TField[] fields)
		{
			Model = model;
			FinalField = finalField;
			Fields = fields ?? new TField[0];
		}

		private FieldPath(FieldPath<TModel, TField> parent, TModel model, TField finalField, TField[] fields)
		{
			_parent = parent;
			Model = model;
			FinalField = finalField;
			Fields = fields ?? new TField[0];
		}

		/// <summary>
		/// Create a new FieldPath instance with a new field appended to the end of the path.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public FieldPath<TModel, TField> Child(TField field)
		{
			return new FieldPath<TModel, TField>(
				this, Model, field,
				Fields.Concat(new[] { field }).ToArray()
				);
		}
	}
}
