using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// A definition a view can be built from.
	/// </summary>
	public class ViewDefinition
	{
		/// <summary>
		/// Gets or sets the view's name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets a list of field definitions.
		/// </summary>
		public List<ViewFieldDefinition> FieldDefinitions { get; } = new List<ViewFieldDefinition>();

		public Model Model { get; }

		/// <summary>
		/// Gets a collection of resource loaders that will load resources before a mapping operation.
		/// </summary>
		public List<IResourceLoader> ResourceLoaders { get; } = new List<IResourceLoader>();

		public ViewDefinition(Model model)
		{
			Model = model;
			Name = model.Name;
		}
	}
}
