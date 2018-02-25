using Silk.Data.Modelling.Conventions;
using Silk.Data.Modelling.ResourceLoaders;
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

		public Model SourceModel { get; }

		public Model TargetModel { get; }

		/// <summary>
		/// Gets the view conventions being used to build the view definition.
		/// </summary>
		public ViewConvention[] ViewConventions { get; }

		/// <summary>
		/// Gets a collection of resource loaders that will load resources before a mapping operation.
		/// </summary>
		public List<IResourceLoader> ResourceLoaders { get; } = new List<IResourceLoader>();

		/// <summary>
		/// User data that can be utilized by view conventions to store data.
		/// </summary>
		public List<object> UserData { get; } = new List<object>();

		public ViewDefinition(Model sourceModel, Model targetModel, ViewConvention[] viewConventions)
		{
			SourceModel = sourceModel;
			TargetModel = targetModel;
			Name = targetModel.Name;
			ViewConventions = viewConventions;
		}
	}
}
