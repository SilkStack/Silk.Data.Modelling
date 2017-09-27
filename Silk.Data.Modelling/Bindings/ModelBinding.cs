using System;
using System.Linq;

namespace Silk.Data.Modelling.Bindings
{
	/// <summary>
	/// Binds a viewfield to a source.
	/// </summary>
	public abstract class ModelBinding
	{
		/// <summary>
		/// Gets the direction the binding applies in.
		/// </summary>
		public abstract BindingDirection Direction { get; }

		/// <summary>
		/// Gets the path to the bound value on the model.
		/// </summary>
		public string[] ModelFieldPath { get; }

		/// <summary>
		/// Gets the path fot he bound value on the view.
		/// </summary>
		public string[] ViewFieldPath { get; }

		public ModelBinding(string[] modelFieldPath, string[] viewFieldPath)
		{
			ModelFieldPath = modelFieldPath;
			ViewFieldPath = viewFieldPath;
		}

		public virtual void WriteToContainer(IContainer container, object value, MappingContext mappingContext)
		{
			container.SetValue(ViewFieldPath, value);
		}

		public virtual object ReadFromContainer(IContainer container, MappingContext mappingContext)
		{
			return container.GetValue(ViewFieldPath);
		}

		/// <summary>
		/// Reads the binding value from the provided modelreadwriter.
		/// </summary>
		/// <param name="modelReadWriter"></param>
		/// <returns></returns>
		public virtual object ReadFromModel(IModelReadWriter modelReadWriter, MappingContext mappingContext)
		{
			foreach (var pathComponent in ModelFieldPath)
			{
				var field = modelReadWriter.Model.Fields.FirstOrDefault(q => q.Name == pathComponent);
				if (field == null)
					throw new InvalidOperationException("Invalid field path.");
				modelReadWriter = modelReadWriter.GetField(field);
				if (modelReadWriter == null)
					return null;
			}
			return modelReadWriter.Value;
		}

		/// <summary>
		/// Writes the given value to the bound model field.
		/// </summary>
		/// <param name="modelReadWriter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual void WriteToModel(IModelReadWriter modelReadWriter, object value, MappingContext mappingContext)
		{
			foreach (var pathComponent in ModelFieldPath)
			{
				var field = modelReadWriter.Model.Fields.FirstOrDefault(q => q.Name == pathComponent);
				if (field == null)
					throw new InvalidOperationException("Invalid field path.");
				modelReadWriter = modelReadWriter.GetField(field);
				if (modelReadWriter == null)
					throw new InvalidOperationException($"Couldn't get field \"{field.Name}\".");
			}
			modelReadWriter.Value = value;
		}
	}

	[Flags]
	public enum BindingDirection
	{
		None = 0,
		ModelToView = 1,
		ViewToModel = 2,
		Bidirectional = 3
	}
}
