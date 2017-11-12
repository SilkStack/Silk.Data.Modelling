using Silk.Data.Modelling.ResourceLoaders;
using System;

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

		/// <summary>
		/// Gets required resource loaders.
		/// </summary>
		public IResourceLoader[] ResourceLoaders { get; }

		public ModelBinding(string[] modelFieldPath, string[] viewFieldPath)
		{
			ModelFieldPath = modelFieldPath;
			ViewFieldPath = viewFieldPath;
		}

		public ModelBinding(string[] modelFieldPath, string[] viewFieldPath,
			IResourceLoader[] resourceLoaders)
		{
			ModelFieldPath = modelFieldPath;
			ViewFieldPath = viewFieldPath;
			ResourceLoaders = resourceLoaders;
		}

		public T ReadValue<T>(IContainerReadWriter from)
		{
			if (from.ContainerType == ContainerType.Model)
				return from.ReadFromPath<T>(ModelFieldPath);
			return from.ReadFromPath<T>(ViewFieldPath);
		}

		public void WriteValue<T>(IContainerReadWriter to, T value)
		{
			if (to.ContainerType == ContainerType.Model)
				to.WriteToPath<object>(ModelFieldPath, value);
			else
				to.WriteToPath<object>(ViewFieldPath, value);
		}

		public virtual void CopyBindingValue(IContainerReadWriter from, IContainerReadWriter to, MappingContext mappingContext)
		{
			object value = ReadValue<object>(from);
			WriteValue<object>(to, value);
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
